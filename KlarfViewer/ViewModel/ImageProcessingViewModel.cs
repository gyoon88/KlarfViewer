using KlarfViewer.utils;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using KlarfViewer.Command;
using KlarfViewer.Service;

namespace KlarfViewer.ViewModel
{
    public class ImageProcessingViewModel : BaseViewModel
    {
        private BitmapSource _originalImage;
        private BitmapSource _processedImage;

        private int _brightness;
        private double _contrast;
        private int _blurRadius;

        public ImageProcessingViewModel(BitmapSource image)
        {
            _originalImage = image;
            _processedImage = image;

            ApplyChangesCommand = new RelayCommand(ApplyChanges);
            ShowHistogramCommand = new RelayCommand(ShowHistogram);
        }

        public BitmapSource ProcessedImage
        {
            get => _processedImage;
            set
            {
                _processedImage = value;
                OnPropertyChanged();
            }
        }

        public int Brightness
        {
            get => _brightness;
            set
            {
                _brightness = value;
                OnPropertyChanged();
                ApplyChanges(); // Apply changes automatically
            }
        }

        public double Contrast
        {
            get => _contrast;
            set
            {
                _contrast = value;
                OnPropertyChanged();
                ApplyChanges(); // Apply changes automatically
            }
        }

        public int BlurRadius
        {
            get => _blurRadius;
            set
            {
                _blurRadius = value;
                OnPropertyChanged();
                ApplyChanges(); // Apply changes automatically
            }
        }

        public ICommand ApplyChangesCommand { get; }
        public ICommand ShowHistogramCommand { get; }

        private void ApplyChanges()
        {
            BitmapSource tempImage = _originalImage;

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