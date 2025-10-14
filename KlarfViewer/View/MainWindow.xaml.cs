using System.Windows;
using System.Windows.Controls;
using KlarfViewer.ViewModel;

namespace KlarfViewer.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            WaferMapContainerGrid.SizeChanged += WaferMapContainerGrid_SizeChanged;
        }

        private void WaferMapContainerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                mainViewModel.WaferMapVM.UpdateMapSize(e.NewSize.Width, e.NewSize.Height);
            }
        }
    }
}