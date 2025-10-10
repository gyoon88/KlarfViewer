using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace KlarfViewer.ViewModel
{
    public class DefectImageViewModel : BaseViewModel
    {
        private BitmapSource defectImage;
        private string imageLoadingError;

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
            private set
            {
                imageLoadingError = value;
                OnPropertyChanged(nameof(ImageLoadingError));
            }
        }

        // constructor
        public DefectImageViewModel() { }

        // method
        public void UpdateImage(string tiffFilePath, int imageId)
        {
            try
            {
                ImageLoadingError = null; // 오류 메시지 초기화

                if (!File.Exists(tiffFilePath))
                {
                    ImageLoadingError = $"TIF 파일을 찾을 수 없습니다: {tiffFilePath}";
                    DefectImage = null;
                    return;
                }

                var decoder = new TiffBitmapDecoder(new Uri(tiffFilePath, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                int frameIndex = imageId - 1; // Klarf의 ID는 1-based, 디코더는 0-based
                if (frameIndex >= 0 && frameIndex < decoder.Frames.Count)
                {
                    DefectImage = decoder.Frames[frameIndex];
                }
                else
                {
                    ImageLoadingError = $"TIF 파일에 해당 이미지가 없습니다. (요청 ID: {imageId}, 최대 프레임: {decoder.Frames.Count})";
                    DefectImage = null;
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
