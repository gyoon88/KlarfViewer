using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace KlarfViewer.ViewModel
{
    public class DefectImageViewModel : BaseViewModel
    {
        private BitmapSource _defectImage;
        public BitmapSource DefectImage
        {
            get => _defectImage;
            private set
            {
                _defectImage = value;
                OnPropertyChanged(nameof(DefectImage));
            }
        }

        public DefectImageViewModel() { }

        public void UpdateImage(string tiffFilePath, int imageId)
        {
            try
            {
                if (!File.Exists(tiffFilePath))
                {
                    // Handle file not found
                    DefectImage = null;
                    return;
                }

                var decoder = new TiffBitmapDecoder(new Uri(tiffFilePath, UriKind.Absolute), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                if (imageId >= 0 && imageId < decoder.Frames.Count)
                {
                    DefectImage = decoder.Frames[imageId];
                }
                else
                {
                    // Handle invalid imageId
                    DefectImage = null;
                }
            }
            catch (Exception)
            {
                // Handle other exceptions, e.g., file format issues
                DefectImage = null;
            }
        }
    }
}
