using KlarfViewer.ViewModel;
using System.Windows.Input;

namespace KlarfViewer.Command
{
    public class ImageProcessingWindowCommand
    {
        private readonly MainViewModel vm;

        public ICommand OpenImageProcessingWindowCommand { get; }

        public ImageProcessingWindowCommand(MainViewModel viewModel)
        {
            vm = viewModel;
            OpenImageProcessingWindowCommand = new RelayCommand(OpenImageProcessingWindow, CanOpenImageProcessingWindow);
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
