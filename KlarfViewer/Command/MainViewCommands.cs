using KlarfViewer.ViewModel;
using System.Windows.Input;

namespace KlarfViewer.Command
{
    public class MainViewCommands
    {
        private readonly MainViewModel vm;

        public ICommand OpenImageProcessingWindowCommand { get; }
        public ExportCsvCommand CsvCommand { get; }

        public MainViewCommands(MainViewModel viewModel)
        {
            vm = viewModel;
            OpenImageProcessingWindowCommand = new RelayCommand(OpenImageProcessingWindow, CanOpenImageProcessingWindow);
            CsvCommand = new ExportCsvCommand(vm);
        }

        private bool CanOpenImageProcessingWindow()
        {
            return vm.DefectImageVM.DefectImage != null;
        }

        private void OpenImageProcessingWindow()
        {
            var imageProcessingViewModel = new ImageProcessingViewModel(vm.DefectImageVM.DefectImage);
            var imageProcessingWindow = new KlarfViewer.View.ImageProcessingWindow
            {
                DataContext = imageProcessingViewModel
            };
            imageProcessingWindow.Show();
        }
    }
}
