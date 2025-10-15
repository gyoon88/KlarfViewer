using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KlarfViewer.ViewModel
{
    public class DefectImageViewModel : BaseViewModel
    {
        private BitmapSource defectImage;
        private string imageLoadingError;
        private bool isInMeasurementMode;
        private double distance;
        private double zoomLevel;

        public BitmapSource DefectImage
        {
            get => defectImage;
            private set
            {
                defectImage = value;
                OnPropertyChanged(nameof(DefectImage));
            }
        }

        public string ImageLoadingError
        {
            get => imageLoadingError;
            private set => SetProperty(ref imageLoadingError, value);
        }

        public bool IsInMeasurementMode
        {
            get => isInMeasurementMode;
            set => SetProperty(ref isInMeasurementMode, value);
        }

        public double Distance
        {
            get => distance;
            set => SetProperty(ref distance, value);
        }

        public double ZoomLevel
        {
            get => zoomLevel;
            set => SetProperty(ref zoomLevel, value);
        }

        public ICommand ToggleMeasurementModeCommand { get; }

        public DefectImageViewModel()
        {
            ToggleMeasurementModeCommand = new RelayCommand(() => IsInMeasurementMode = !IsInMeasurementMode);
            ZoomLevel = 100.0;
        }

        public void UpdateImage(string tiffFilePath, int imageId)
        {
            try
            {
                ImageLoadingError = null;
                DefectImage = null;
                Distance = 0;
                ZoomLevel = 100.0;

                if (!File.Exists(tiffFilePath))
                {
                    ImageLoadingError = $"TIF 파일을 찾을 수 없습니다: {tiffFilePath}";
                    return;
                }

                var decoder = new TiffBitmapDecoder(
                    new Uri(tiffFilePath, UriKind.Absolute),
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad
                );

                int frameIndex = imageId;
                if (frameIndex >= 0 && frameIndex < decoder.Frames.Count)
                {
                    DefectImage = decoder.Frames[frameIndex];
                }
                else
                {
                    ImageLoadingError = $"TIF 파일에 해당 이미지가 없습니다. (요청 ID: {imageId}, 최대 프레임: {decoder.Frames.Count})";
                }
            }
            catch (Exception ex)
            {
                ImageLoadingError = $"이미지 로딩 오류: {ex.Message}";
                DefectImage = null;
            }
        }
    }
}
