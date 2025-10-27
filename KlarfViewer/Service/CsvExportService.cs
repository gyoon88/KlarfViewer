using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace KlarfViewer.Service
{
    public class CsvExportService
    {
        public CsvExportService(){}
        public static void Export(string filePath, IEnumerable<string[]> data, string[] headers)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", headers.Select(h => EscapeCsvField(h))));

            foreach (var line in data)
            {
                sb.AppendLine(string.Join(",", line.Select(l => EscapeCsvField(l))));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        public static void ExportImages(string filePath, List<BitmapSource> images)
        {
            if (images == null || !images.Any())
            {
                return;
            }

            var encoder = new TiffBitmapEncoder();

            foreach (var image in images)
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return "";
            }

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            {
                return $"\"{field.Replace("\"", "\"")}\"";
            }
            return field;
        }
    }
}
