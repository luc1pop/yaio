using Yaio.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yaio.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }





        protected override void OnClosing(CancelEventArgs e)
        {
            MainWindowViewModel viewModel = this.DataContext as MainWindowViewModel;


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = this.DataContext as MainWindowViewModel;
        }

        private void FolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            if (sender == FromFolderBrowseButton)
            {
                dialog.SelectedPath = viewModel.FromFolderPath;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                viewModel.FromFolderPath = dialog.SelectedPath;
            }
            if (sender == ToFolderBrowseButton)
            {
                dialog.SelectedPath = viewModel.ToFolderPath;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                viewModel.ToFolderPath = dialog.SelectedPath;
            }
        }

        private void StartProcess_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            try
            {
                viewModel.StartProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void StartExtractingProcess_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            try
            {
                viewModel.StartExtractingProcess();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void StopProcess_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            try
            {
                viewModel.CancelProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}

