using KlarfViewer.utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KlarfViewer.ViewModel
{
    public class HistogramViewModel : BaseViewModel
    {
        #region Public Properties for View Binding

        private BitmapSource? image;
        public BitmapSource? Image
        {
            get => image;
            set
            {
                if (SetProperty(ref image, value))
                {
                    UpdateAllHistograms(Image);
                }
            }
               
        }
        private MainViewModel MainVM { get; set; }
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

        public HistogramViewModel(MainViewModel mainVM)
        {
            MainVM = mainVM;

            Image = MainVM.DefectImageVM.DefectImage;
            MainVM.DefectImageVM.PropertyChanged += OnDefectImageChanged;
            MainVM.ImageProcessingVM.PropertyChanged += OnImageProcessChanged;

            UpdateAllHistograms(Image);
        }


        private void OnDefectImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefectImageViewModel.DefectImage))
            {
                Image = MainVM.DefectImageVM.DefectImage;
                UpdateAllHistograms(Image);
            }
        }
        private void OnImageProcessChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageProcessingViewModel.ProcessedImage))
            {
                Image = MainVM.ImageProcessingVM.ProcessedImage;
                UpdateAllHistograms(Image);
            }
        }
        private void UpdateAllHistograms(BitmapSource imageSource)
        {
            if (imageSource == null) return;

            if (imageSource.Format == PixelFormats.Gray8)
            {
                IsColorImage = false;
                GrayscaleHistogramData = CSharpHistogram.CalculateGrayscaleHistogram(imageSource);
                R_HistogramData = G_HistogramData = B_HistogramData = null;
                MaxHistogramValue = GrayscaleHistogramData.Any() ? GrayscaleHistogramData.Max() : 0;

                var stats = CSharpHistogram.CalculateStatistics(GrayscaleHistogramData);
                SetStatistics(stats);
            }
            else
            {
                IsColorImage = true;
                var colorHistograms = CSharpHistogram.CalculateColorHistograms(imageSource);

                R_HistogramData = colorHistograms.R;
                G_HistogramData = colorHistograms.G;
                B_HistogramData = colorHistograms.B;
                GrayscaleHistogramData = null;

                int maxR = R_HistogramData?.Max() ?? 0;
                int maxG = G_HistogramData?.Max() ?? 0;
                int maxB = B_HistogramData?.Max() ?? 0;
                MaxHistogramValue = Math.Max(maxR, Math.Max(maxG, maxB));

                var statsHistogram = CSharpHistogram.CalculateGrayscaleHistogram(imageSource);
                var stats = CSharpHistogram.CalculateStatistics(statsHistogram);
                SetStatistics(stats);
            }

            OnPropertyChanged(nameof(IsColorImage));
            OnPropertyChanged(nameof(R_HistogramData));
            OnPropertyChanged(nameof(G_HistogramData));
            OnPropertyChanged(nameof(B_HistogramData));
            OnPropertyChanged(nameof(GrayscaleHistogramData));
        }                

        private void SetStatistics(HistogramStatistics stats)
        {
            Mean = stats.Mean;
            Std = stats.Std;
            Median = stats.Median;
            Mode = stats.Mode;
            Max = stats.Max;
            Min = stats.Min;
            Range = stats.Range;
        }
    }
}