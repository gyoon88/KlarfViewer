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

        // Grid 사이즈에 맞춰 웨이퍼 맵 크기를 바꿔주기 위한 코드 비하인드
        private void WaferMapContainerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel mainViewModel && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                mainViewModel.WaferMapVM.UpdateMapSize(e.NewSize.Width, e.NewSize.Height);
            }
        }
    }
}