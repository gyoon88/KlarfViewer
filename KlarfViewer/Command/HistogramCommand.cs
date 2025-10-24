using KlarfViewer.Service;
using KlarfViewer.View;
using KlarfViewer.ViewModel;
using KlarfViewer.Service;
using System.Windows.Input;

namespace KlarfViewer.Command
{
    public class HistogramCommand
    {
        private MainViewModel mainVM;
        public ICommand ShowHistogramCommand { get; set; }

        public HistogramCommand( MainViewModel mainViewModel) 
        {
            mainVM = mainViewModel;
            ShowHistogramCommand = new RelayCommand(ExecuteHistogram, CanExecuteHistogram);             
        }
        private bool CanExecuteHistogram()
        {
            bool canExecuteHistogram = mainVM.DefectImageVM.DefectImage != null;                   
            return canExecuteHistogram;
        }
        private void ExecuteHistogram()
        {
            var histogramService = new HistogramService();
            histogramService.ShowHistogram(mainVM.DefectImageVM.DefectImage);
        }
    }
}
