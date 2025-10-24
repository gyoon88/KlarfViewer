using System;
using System.Linq;

namespace KlarfViewer.utils
{
    public struct HistogramStatistics
    {
        public double? Mean { get; set; }
        public double? Std { get; set; }
        public int? Median { get; set; }
        public int? Mode { get; set; }
        public int? Max { get; set; }
        public int? Min { get; set; }
        public int? Range { get; set; }
    }

    public static class CSharpHistogram
    {
        public static HistogramStatistics CalculateStatistics(int[]? histogram)
        {
            HistogramStatistics stats = new HistogramStatistics();

            if (histogram == null || histogram.Length != 256 || histogram.Sum() == 0)
            {
                return stats; // Returns a struct with all nulls
            }

            long totalPixels = 0;
            long sumOfIntensities = 0;
            for (int i = 0; i < 256; i++)
            {
                totalPixels += histogram[i];
                sumOfIntensities += (long)i * histogram[i];
            }

            if (totalPixels == 0)
            {
                return stats; // Returns a struct with all nulls
            }

            double meanValue = (double)sumOfIntensities / totalPixels;
            stats.Mean = meanValue;

            double sumOfSquaredDifferences = 0;
            for (int i = 0; i < 256; i++)
            {
                sumOfSquaredDifferences += Math.Pow(i - meanValue, 2) * histogram[i];
            }
            stats.Std = Math.Sqrt(sumOfSquaredDifferences / totalPixels);

            long cumulativeFrequency = 0;
            long medianThreshold = totalPixels / 2;
            int medianValue = 0;
            for (int i = 0; i < 256; i++)
            {
                cumulativeFrequency += histogram[i];
                if (cumulativeFrequency >= medianThreshold)
                {
                    medianValue = i;
                    break;
                }
            }
            stats.Median = medianValue;

            int maxFrequency = 0;
            int modeValue = 0;
            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > maxFrequency)
                {
                    maxFrequency = histogram[i];
                    modeValue = i;
                }
            }
            stats.Mode = modeValue;

            int minValue = -1;
            int maxValue = -1;
            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > 0)
                {
                    if (minValue == -1)
                    {
                        minValue = i;
                    }
                    maxValue = i;
                }
            }
            stats.Min = minValue != -1 ? minValue : (int?)null;
            stats.Max = maxValue != -1 ? maxValue : (int?)null;

            if (stats.Min.HasValue && stats.Max.HasValue)
            {
                stats.Range = stats.Max.Value - stats.Min.Value;
            }
            else
            {
                stats.Range = null;
            }
            return stats;
        }
    }
}
