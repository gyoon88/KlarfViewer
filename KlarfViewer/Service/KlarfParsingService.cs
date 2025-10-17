using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using KlarfViewer.Model;

using System.Threading.Tasks;

namespace KlarfViewer.Service
{

    public class KlarfParsingService
    {
        public Task<KlarfData> ParseAsync(string filePath, IProgress<double> progress)
        {
            return Task.Run(() =>
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Klarf 파일을 찾을 수 없습니다.", filePath);
                }

                KlarfData CurrentklarfData = new KlarfData { FilePath = filePath };
                var lines = File.ReadAllLines(filePath);
                int lineIndex = 0;
                int totalLines = lines.Length;

                while (lineIndex < totalLines)
                {
                    progress?.Report((double)lineIndex / totalLines * 100);

                    var line = lines[lineIndex].Trim();

                    if (string.IsNullOrEmpty(line))
                    {
                        lineIndex++;
                        continue;
                    }

                    string keyword;
                    string[] values;

                    int firstSpace = line.IndexOf(' ');
                    if (firstSpace == -1)
                    {
                        keyword = line.TrimEnd(';');
                        values = Array.Empty<string>();
                    }
                    else
                    {
                        keyword = line.Substring(0, firstSpace);
                        var remaining = line.Substring(firstSpace + 1).Trim().TrimEnd(';');
                        values = remaining.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    try
                    {
                        switch (keyword.ToUpper())
                        {
                            case "WAFERID":
                                CurrentklarfData.Wafer.WaferID = values[0].Trim('"');
                                break;
                            case "LOTID":
                                CurrentklarfData.Wafer.LotID = values[0].Trim('"');
                                break;
                            case "SLOT":
                                CurrentklarfData.Wafer.Slot = values[0].Trim('"');
                                break;
                            case "INSPECTIONSTATIONID":
                                CurrentklarfData.Wafer.DeviceID = string.Join(" - ", values.Select(
                                    value => value.Trim('"')).Where(
                                    trimmedValue => !string.IsNullOrWhiteSpace(trimmedValue)));
                                break;
                            case "FILETIMESTAMP":
                                string timestamp = values[0] + " " + values[1];
                                CurrentklarfData.Wafer.FileTimestamp = DateTime.ParseExact(timestamp, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                break;
                            case "SAMPLECENTERLOCATION":
                                CurrentklarfData.Wafer.SampleCenterLocation = new SampleCenter
                                {
                                    XLoc = double.Parse(values[0], CultureInfo.InvariantCulture),
                                    YLoc = double.Parse(values[1], CultureInfo.InvariantCulture)
                                };
                                break;
                            case "TIFFFILENAME":
                                CurrentklarfData.Wafer.TiffFilename = values[0];
                                break;
                            case "DIEPITCH":
                                CurrentklarfData.Wafer.DiePitch = new DieSize
                                {
                                    Width = double.Parse(values[0], CultureInfo.InvariantCulture),
                                    Height = double.Parse(values[1], CultureInfo.InvariantCulture)
                                };
                                break;
                            case "SAMPLETESTPLAN":
                                int dieCount = int.Parse(values[0]);
                                CurrentklarfData.Wafer.TotalDies = int.Parse(values[0]);
                                lineIndex = ParseSampleTestPlan(lines, lineIndex + 1, dieCount, CurrentklarfData.Dies);
                                continue;
                            case "DEFECTRECORDSPEC":
                                var defectHeaders = values.Skip(1).ToList();
                                lineIndex = ParseDefectList(lines, lineIndex + 2, defectHeaders, CurrentklarfData.Defects);
                                continue;
                            case "ENDOFILE;":
                            case "ENDOFFILE":
                                lineIndex = lines.Length;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        var log = $"Error parsing line {lineIndex + 1}: '{lines[lineIndex]}'. Error: {ex.Message}";
                        Console.WriteLine(log);
                        MessageBox.Show("Klarf 파싱중 에러가 발생하였습니다.", log);
                    }
                    lineIndex++;
                }

                progress?.Report(100);
                ValidateParsedData(CurrentklarfData);
                LinkDefectsToDies(CurrentklarfData);
                return CurrentklarfData;
            });
        }
        private void ValidateParsedData(KlarfData klarfData)
        {
            var missingFields = new List<string>();           

            if (string.IsNullOrEmpty(klarfData.Wafer.WaferID))
            {
                missingFields.Add("WaferID");
            }
            if (string.IsNullOrEmpty(klarfData.Wafer.LotID))
            {
                missingFields.Add("LotID");
            }
            if (string.IsNullOrEmpty(klarfData.Wafer.TiffFilename))
            {
                missingFields.Add("TiffFilename");
            }
            if (klarfData.Wafer.FileTimestamp == DateTime.MinValue)
            {
                missingFields.Add("FileTimestamp");
            }
            if (klarfData.Dies == null || klarfData.Dies.Count == 0)
            {
                missingFields.Add("SampleTestPlan (Die list)");
            }
            if (klarfData.Defects == null || klarfData.Defects.Count == 0)
            {
                missingFields.Add("Defect list");
            }
            if (missingFields.Count > 0)
            {
                string message = "파싱이 완료되었지만, 일부 필수 데이터가 누락되었습니다:\n\n" + string.Join("\n", missingFields);
                MessageBox.Show(message, "파싱 경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private int ParseSampleTestPlan(string[] lines, int startIndex, int count, List<DieInfo> dies)
        {
            for (int i = 0; i < count; i++)
            {
                var currentLineIndex = startIndex + i;
                if (currentLineIndex >= lines.Length) break;

                var line = lines[currentLineIndex].Trim();
                var tokens = line.TrimEnd(';').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    dies.Add(new DieInfo
                    {
                        XIndex = int.Parse(tokens[0]),
                        YIndex = int.Parse(tokens[1]),
                        DieID = i + 1
                    });
                }
            }
            // return the index of line 
            return startIndex + count;
        }

        // Call from parse when the keyword is DefectList 
        private int ParseDefectList(string[] lines, int startIndex, List<string> headers, List<DefectInfo> defects)
        {
            int currentIndex = startIndex;
            // 헤더 이름과 인덱스를 매핑하여 쉽게 값을 찾을 수 있도록 딕셔너리생성
            var headerMap = headers.Select((name, index) => new { name, index }).ToDictionary(p => p.name, p => p.index);

            while (currentIndex < lines.Length && !lines[currentIndex].Contains("};") && !lines[currentIndex].Contains("SummarySpec"))
            {
                var line = lines[currentIndex].TrimEnd(';');
                var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < headers.Count)
                {
                    currentIndex++;
                    continue; // IMAGELIST 등으로 인해 여러 줄로 나뉜 경우 일단은 Jump~
                }

                var defect = new DefectInfo();
                // Assign the value safety by use dictionary
                defect.Id = GetValue<int>(tokens, headerMap, "DEFECTID");
                defect.XRel = GetValue<double>(tokens, headerMap, "XREL");
                defect.YRel = GetValue<double>(tokens, headerMap, "YREL");
                defect.XIndex = GetValue<int>(tokens, headerMap, "XINDEX");
                defect.YIndex = GetValue<int>(tokens, headerMap, "YINDEX");
                defect.XSize = GetValue<double>(tokens, headerMap, "XSIZE");
                defect.YSize = GetValue<double>(tokens, headerMap, "YSIZE");
                defect.DefectArea = GetValue<double>(tokens, headerMap, "DEFECTAREA");
                defect.DSize = GetValue<int>(tokens, headerMap, "DSIZE");
                defect.ImageCount = GetValue<int>(tokens, headerMap, "IMAGECOUNT");
                defect.ImageList = GetValue<int>(tokens, headerMap, "IMAGELIST");
                defect.ImageId = GetValue<int>(tokens, headerMap, "IMAGEID");

                defects.Add(defect);
                currentIndex++;
            }
            return currentIndex;
        }

        // HelperMethod for parse safety, clean by use dictionary and generic
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
        private T GetValue<T>(string[] tokens, Dictionary<string, int> map, string key)
        {
            if (map.TryGetValue(key, out int index) && index < tokens.Length)
            {
                var value = tokens[index];
                // TryParse를 사용하여 변환 실패 시 기본값을 반환
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
                catch { return default(T); }
            }
            return default(T); // return default value when key does not exist or range over 
        }

        private void LinkDefectsToDies(KlarfData data)
        {
            // Die들을 딕셔너리로 변환 (Key: "X_Y", Value: DieData 객체)
            var dieMap = data.Dies.ToDictionary(d => $"{d.XIndex}_{d.YIndex}");

            foreach (var defect in data.Defects)
            {
                var key = $"{defect.XIndex}_{defect.YIndex}";
                if (dieMap.TryGetValue(key, out DieInfo? die))
                {
                    die.DefectCount++;
                    die.IsDefective = true;
                    defect.DefectIdInDie = die.DefectCount;
                }
            }
            foreach (var defect in data.Defects)
            {
                var key = $"{defect.XIndex}_{defect.YIndex}";
                if (dieMap.TryGetValue(key, out DieInfo? die))
                {
                    // 최종 Defect 개수를 할당
                    defect.TotalDefectsInDie = die.DefectCount;
                }
            }
        }
    }
}
