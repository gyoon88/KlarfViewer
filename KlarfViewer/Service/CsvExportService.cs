﻿using System.IO;

using System.Text;


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
