using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Yaio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>


    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Views.MainWindow mainWindow = new Views.MainWindow();
            mainWindow.DataContext = new ViewModels.MainWindowViewModel();
            MainWindow.Show();
        }
    }
}
