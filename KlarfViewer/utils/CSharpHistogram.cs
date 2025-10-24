using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
        public static (int[] R, int[] G, int[] B) CalculateColorHistograms(BitmapSource imageSource)
        {
            if (imageSource.Format != PixelFormats.Bgra32)
            {
                imageSource = new FormatConvertedBitmap(imageSource, PixelFormats.Bgra32, null, 0);
            }

            int width = imageSource.PixelWidth;
            int height = imageSource.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            imageSource.CopyPixels(pixels, stride, 0);

            int[] rHist = new int[256];
            int[] gHist = new int[256];
            int[] bHist = new int[256];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                bHist[pixels[i]]++;
                gHist[pixels[i + 1]]++;
                rHist[pixels[i + 2]]++;
            }

            return (rHist, gHist, bHist);
        }

        public static int[] CalculateGrayscaleHistogram(BitmapSource imageSource)
        {
            if (imageSource.Format != PixelFormats.Gray8)
            {
                imageSource = new FormatConvertedBitmap(imageSource, PixelFormats.Gray8, null, 0);
            }

            int width = imageSource.PixelWidth;
            int height = imageSource.PixelHeight;
            int stride = width;
            byte[] pixels = new byte[height * stride];
            imageSource.CopyPixels(pixels, stride, 0);

            int[] histogram = new int[256];
            foreach (byte pixel in pixels)
            {
                histogram[pixel]++;
            }

            return histogram;
        }

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
