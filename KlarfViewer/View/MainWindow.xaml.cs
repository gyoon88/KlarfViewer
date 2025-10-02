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
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.FileViewerVM.SelectedItem = e.NewValue as FileSystemObjectViewModel;
            }
        }
    }
}