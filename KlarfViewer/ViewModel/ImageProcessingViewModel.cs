using KlarfViewer.utils;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using KlarfViewer.Command;
using KlarfViewer.Service;

namespace KlarfViewer.ViewModel
{
    public class ImageProcessingViewModel : BaseViewModel
    {
        private BitmapSource originalImage;
        private BitmapSource processedImage;

        private int brightness;
        private double contrast;
        private int blurRadius;

        public ImageProcessingViewModel(BitmapSource image)
        {
            originalImage = image;
            processedImage = image;

            ApplyChangesCommand = new RelayCommand(ApplyChanges);
            ShowHistogramCommand = new RelayCommand(ShowHistogram);
        }

        public BitmapSource ProcessedImage
        {
            get => processedImage;
            set => SetProperty(ref processedImage, value);
        }

        public int Brightness
        {
            get => brightness;
            set
            {
                if (SetProperty(ref brightness, value))
                {
                    ApplyChanges(); // Apply changes automatically
                }
            }
        }

        public double Contrast
        {
            get => contrast;
            set
            {
                if (SetProperty(ref contrast, value))
                {
                    ApplyChanges(); // Apply changes automatically
                }
            }
        }

        public int BlurRadius
        {
            get => blurRadius;
            set
            {
                if (SetProperty(ref blurRadius, value))
                {
                    ApplyChanges(); // Apply changes automatically
                }
            }
        }

        public ICommand ApplyChangesCommand { get; }
        public ICommand ShowHistogramCommand { get; }

        private void ApplyChanges()
        {
            BitmapSource tempImage = originalImage;

            // Apply Brightness and Contrast together
            if (Brightness != 0 || Contrast != 0)
            {
                tempImage = CSharpImageProcessor.ApplyBrightnessContrast(tempImage, Brightness, Contrast);
            }

            // Then apply blur
            if (BlurRadius > 0)
            {
                tempImage = CSharpImageProcessor.ApplyGaussianBlur(tempImage, BlurRadius);
            }

            ProcessedImage = tempImage;
        }

        private void ShowHistogram()
        {
            var histogramService = new HistogramService();
            histogramService.ShowHistogram(ProcessedImage);
        }
    }
}