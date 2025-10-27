using KlarfViewer.Command;
using KlarfViewer.Service;
using KlarfViewer.utils;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KlarfViewer.ViewModel
{
    public class ImageProcessingViewModel : BaseViewModel
    {
        private BitmapSource originalImage;
        private BitmapSource processedImage;

        private int brightness;
        private double contrast;
        private int blurRadius;
        private double sigma = 1.0;

        private DefectImageViewModel DefectImageViewer;


        private MainViewModel MainVM { get; set; }

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

        public double Sigma
        {
            get => sigma;
            set
            {
                if (SetProperty(ref sigma, value))
                {
                    ApplyChanges();
                }
            }
        }

        public ICommand ApplyChangesCommand { get; }
        public ICommand ApplyToSourceCommand { get; }

        public ImageProcessingViewModel(DefectImageViewModel DefectImageVM, MainViewModel mainVM)
        {
            MainVM = mainVM;
            DefectImageViewer = DefectImageVM;
            originalImage = DefectImageViewer.DefectImage;
            processedImage = DefectImageViewer.DefectImage;
            DefectImageViewer.PropertyChanged += OnDefectImageChanged;
            ApplyChangesCommand = new RelayCommand(ApplyChanges);
            ApplyToSourceCommand = new RelayCommand(ApplyToSource);
        }

        public void ApplyToSource()
        {
            if (MainVM.DefectListVM.SelectedDefect != null)
            {
                DefectImageViewer.DefectImage = ProcessedImage;
                MainVM.AddModifiedImage(MainVM.DefectListVM.SelectedDefect.Id, ProcessedImage);
            }
        }

        private void OnDefectImageChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefectImageViewModel.DefectImage))
            {
                originalImage = DefectImageViewer.DefectImage;
                ApplyChanges();
            }
        }
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
                tempImage = CSharpImageProcessor.ApplyGaussianBlur(tempImage, BlurRadius, Sigma);
            }

            ProcessedImage = tempImage;
        }


    }
}