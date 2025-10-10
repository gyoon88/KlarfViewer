using System.Configuration;
using System.Data;
using System.Windows;
using KlarfViewer.View; // 추가

namespace KlarfViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}