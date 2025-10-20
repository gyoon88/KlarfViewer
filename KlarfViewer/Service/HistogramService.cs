using KlarfViewer.ViewModel;
using System.Windows.Media.Imaging;

namespace KlarfViewer.Service
{
    public class HistogramService
    {
        public void ShowHistogram(BitmapSource image)
        {
            if (image == null) return;

            HistogramViewModel histogramViewModel = new HistogramViewModel(image);
            View.HistogramWindow histogramWindow = new View.HistogramWindow
            {
                DataContext = histogramViewModel
            };
            histogramWindow.Show();
        }
    }
}
