using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KlarfViewer.Model;

namespace KlarfViewer.Service
{

    public class KlarfParsingService
    {
        /// <summary>
        /// 지정된 경로의 Klarf 파일을 파싱하여 KlarfData 객체로 반환.
        /// </summary>
        /// <param name="filePath">파싱할 .klarf 파일의 전체 경로</param>
        /// <returns>파싱된 데이터가 담긴 KlarfData 객체</returns>
        public KlarfData Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Klarf 파일을 찾을 수 없습니다.", filePath);
            }
            // initialize KlarfData instance and read klarf file
            KlarfData klarfData = new KlarfData();
            var lines = File.ReadAllLines(filePath);

            // Line index 
            int lineIndex = 0;

            while (lineIndex < lines.Length)            {
                var line = lines[lineIndex].Trim(); // trimming for tokenization                
                var tokens = line.TrimEnd(';').Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // tokenization

                // Jump Empty Lines
                if (tokens.Length == 0)
                {
                    lineIndex++;
                    continue; 
                }

                var keyword = tokens[0];

                try
                {
                    switch (keyword)
                    {
                        case "WaferID":
                            klarfData.Wafer.WaferID = tokens[1].Trim('"');
                            break;
                        case "LotID":
                            klarfData.Wafer.LotID = tokens[1].Trim('"');
                            break;
                        case "Slot":
                            klarfData.Wafer.Slot = int.Parse(tokens[1]);
                            break;
                        case "FileTimestamp":
                            klarfData.Wafer.FileTimestamp = DateTime.ParseExact(tokens[1] + " " + tokens[2], "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture); // Data Pharsing the form "08-18-2023 16:19:42"
                            break;
                        case "TiffFilename":
                            klarfData.Wafer.TiffFilename = tokens[1];
                            break;
                        case "DiePitch":
                            // Parsing the exponential like 3.963000e+003 to double 
                            klarfData.Wafer.DiePitch = new DieSize
                            {
                                Width = double.Parse(tokens[1], CultureInfo.InvariantCulture),
                                Height = double.Parse(tokens[2], CultureInfo.InvariantCulture)
                            };
                            break;

                        // --- 여러 줄에 걸쳐 데이터를 읽어야 하는 특별한 케이스 ---
                        case "SampleTestPlan":
                            int dieCount = int.Parse(tokens[1]);
                            // 다음 줄부터 dieCount 만큼의 줄을 읽어 Die 리스트 
                            lineIndex = ParseSampleTestPlan(lines, lineIndex + 1, dieCount, klarfData.Dies);
                            continue; // lineIndex가 이미 업데이트되었으므로 continue

                        case "DefectRecordSpec":
                            // DefectList의 컬럼 순서를 정의하는 헤더를 파싱.
                            var defectHeaders = tokens.Skip(2).ToList();
                            // DefectList를 파싱하기 위해 다음 줄로 넘어감.
                            lineIndex = ParseDefectList(lines, lineIndex + 2, defectHeaders, klarfData.Defects);
                            continue; // lineIndex가 이미 업데이트되었으므로 continue;

                        case "EndOfFile;":
                            // 파일의 끝에 도달했으므로 루프를 종료
                            lineIndex = lines.Length;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var log = $"Error parsing line {lineIndex + 1}: '{lines[lineIndex]}'. Error: {ex.Message}";
                    // 파싱 중 오류가 발생하면 콘솔에 기록하거나 로그 출력
                    Console.WriteLine(log);
                    throw new FileNotFoundException("Klarf 파싱중 에러가 발생하였습니다.", log);
                }

                lineIndex++;
            }

            // 파싱 후처리: 각 Die에 불량 여부(IsDefective)를 마킹
            LinkDefectsToDies(klarfData);

            return klarfData;
        }

        private int ParseSampleTestPlan(string[] lines, int startIndex, int count, List<DieInfo> dies)
        {
            for (int i = 0; i < count; i++)
            {
                var currentLineIndex = startIndex + i;
                if (currentLineIndex >= lines.Length) break;

                var tokens = lines[currentLineIndex].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    dies.Add(new DieInfo
                    {
                        XIndex = int.Parse(tokens[0]),
                        YIndex = int.Parse(tokens[1])
                    });
                }
            }
            // 읽은 마지막 줄의 인덱스를 반환.
            return startIndex + count;
        }

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

                // IMAGELIST는 여러 개일 수 있음, 여기서는 첫 번째(대표) ID만 가져옴
                defect.ImageId = GetValue<int>(tokens, headerMap, "IMAGECOUNT") > 0 ? GetValue<int>(tokens, headerMap, "IMAGELIST") : -1;

                defects.Add(defect);
                currentIndex++;
            }
            return currentIndex;
        }

        // HelperMethod for parse safety, clean by use dictionary and generic
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
                if (dieMap.TryGetValue(key, out DieInfo die))
                {
                    die.IsDefective = true;
                }
            }
        }
    }
}
