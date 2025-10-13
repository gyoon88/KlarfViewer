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
    }
}