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
        }

        private void FileTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel mainViewModel && e.NewValue is FileSystemObjectViewModel selectedItem && selectedItem.IsDirectory)
            {
                mainViewModel.FileListVM.SelectedDirectory = selectedItem;
            }
        }
    }
}