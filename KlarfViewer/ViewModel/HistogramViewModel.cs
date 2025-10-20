using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media;
using KlarfViewer.utils;

namespace KlarfViewer.ViewModel
{
    public class HistogramViewModel : BaseViewModel
    {
        #region Public Properties for View Binding

        public int[]? R_HistogramData { get; private set; }
        public int[]? G_HistogramData { get; private set; }
        public int[]? B_HistogramData { get; private set; }
        public int[]? GrayscaleHistogramData { get; private set; }

        private int maxHistogramValue;
        public int MaxHistogramValue
        {
            get => maxHistogramValue;
            private set => SetProperty(ref maxHistogramValue, value);
        }

        private bool isColorImage;
        public bool IsColorImage
        {
            get => isColorImage;
            private set => SetProperty(ref isColorImage, value);
        }

        #endregion

        #region Public Properties for Statistics
        private double? mean;
        private double? std;
        private int? median;
        private int? mode;
        private int? max;
        private int? min;
        private int? range;

        public double? Mean { get => mean; private set => SetProperty(ref mean, value); }
        public double? Std { get => std; private set => SetProperty(ref std, value); }
        public int? Median { get => median; private set => SetProperty(ref median, value); }
        public int? Mode { get => mode; private set => SetProperty(ref mode, value); }
        public int? Max { get => max; private set => SetProperty(ref max, value); }
        public int? Min { get => min; private set => SetProperty(ref min, value); }
        public int? Range { get => range; private set => SetProperty(ref range, value); }
        #endregion

        public HistogramViewModel(BitmapSource imageSource)
        {
            UpdateAllHistograms(imageSource);
        }

        private void UpdateAllHistograms(BitmapSource imageSource)
        {
            if (imageSource == null) return;

            if (imageSource.Format == PixelFormats.Gray8)
            {
                IsColorImage = false;
                GrayscaleHistogramData = CSharpImageProcessor.CalculateGrayscaleHistogram(imageSource);
                R_HistogramData = G_HistogramData = B_HistogramData = null;
                MaxHistogramValue = GrayscaleHistogramData.Any() ? GrayscaleHistogramData.Max() : 0;

                CalculateAndSetStatistics(GrayscaleHistogramData);
            }
            else
            {
                IsColorImage = true;
                var colorHistograms = CSharpImageProcessor.CalculateColorHistograms(imageSource);

                R_HistogramData = colorHistograms.R;
                G_HistogramData = colorHistograms.G;
                B_HistogramData = colorHistograms.B;
                GrayscaleHistogramData = null;

                int maxR = R_HistogramData?.Max() ?? 0;
                int maxG = G_HistogramData?.Max() ?? 0;
                int maxB = B_HistogramData?.Max() ?? 0;
                MaxHistogramValue = Math.Max(maxR, Math.Max(maxG, maxB));

                var statsHistogram = CSharpImageProcessor.CalculateGrayscaleHistogram(imageSource);
                CalculateAndSetStatistics(statsHistogram);
            }

            OnPropertyChanged(nameof(IsColorImage));
            OnPropertyChanged(nameof(R_HistogramData));
            OnPropertyChanged(nameof(G_HistogramData));
            OnPropertyChanged(nameof(B_HistogramData));
            OnPropertyChanged(nameof(GrayscaleHistogramData));
        }

        private void ClearStatistics()
        {
            Mean = null;
            Std = null;
            Median = null;
            Mode = null;
            Max = null;
            Min = null;
            Range = null;
        }

        private void CalculateAndSetStatistics(int[]? histogram)
        {
            if (histogram == null || histogram.Length != 256 || histogram.Sum() == 0)
            {
                ClearStatistics();
                return;
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
                ClearStatistics();
                return;
            }

            double meanValue = (double)sumOfIntensities / totalPixels;
            Mean = meanValue;

            double sumOfSquaredDifferences = 0;
            for (int i = 0; i < 256; i++)
            {
                sumOfSquaredDifferences += Math.Pow(i - meanValue, 2) * histogram[i];
            }
            Std = Math.Sqrt(sumOfSquaredDifferences / totalPixels);

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
            Median = medianValue;

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
            Mode = modeValue;

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
            Min = minValue != -1 ? minValue : (int?)null;
            Max = maxValue != -1 ? maxValue : (int?)null;

            if (Min.HasValue && Max.HasValue)
            {
                Range = Max.Value - Min.Value;
            }
            else
            {
                Range = null;
            }
        }
    }
}