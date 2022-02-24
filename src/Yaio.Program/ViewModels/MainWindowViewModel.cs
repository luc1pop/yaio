using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Yaio.FileHandler;
using Yaio.Utils;

namespace Yaio.ViewModels
{
    class MainWindowViewModel : BaseClassViewModel
    {

        CancellationTokenSource cancelationTokenForProcess;

        public MainWindowViewModel()
        {
            ToFolderPath = Yaio.Properties.Settings.Default.ToFolderPath;
            FromFolderPath = Yaio.Properties.Settings.Default.FromFolderPath;
            FileExtensions = Yaio.Properties.Settings.Default.FileExtensions;
            CreateYearDirectory = Yaio.Properties.Settings.Default.CreateYearDirectory;
            CreateMonthDirectory = Yaio.Properties.Settings.Default.CreateMonthDirectory;
            Recursive = Yaio.Properties.Settings.Default.Recursive;
            DeleteDuplicateFilesFromProcessFolder = Yaio.Properties.Settings.Default.DeleteDuplicateFilesFromProcessFolder;
            SearchForDuplicatesRecursivelyInYearFolder = Yaio.Properties.Settings.Default.SearchForDuplicatesRecursivelyInYearFolder;
            _Log = new ObservableCollection<LogViewModel>();
        }

        #region properties
        bool _CreateYearDirectory;
        public bool CreateYearDirectory
        {
            get { return _CreateYearDirectory; }
            set
            {
                if (value != _CreateYearDirectory)
                {
                    _CreateYearDirectory = value;
                    Yaio.Properties.Settings.Default.CreateYearDirectory = _CreateYearDirectory;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(CreateYearDirectory));
                }
            }
        }


        bool _CreateMonthDirectory;
        public bool CreateMonthDirectory
        {
            get { return _CreateMonthDirectory; }
            set
            {
                if (value != _CreateMonthDirectory)
                {
                    _CreateMonthDirectory = value;
                    Yaio.Properties.Settings.Default.CreateMonthDirectory = _CreateMonthDirectory;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(CreateMonthDirectory));
                }
            }
        }

        bool _Recursive;
        public bool Recursive
        {
            get { return _Recursive; }
            set
            {
                if (value != _Recursive)
                {
                    _Recursive = value;
                    Yaio.Properties.Settings.Default.Recursive = _Recursive;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(Recursive));
                }
            }
        }


        


        bool _DeleteDuplicateFilesFromProcessFolder;
        public bool DeleteDuplicateFilesFromProcessFolder
        {
            get { return _DeleteDuplicateFilesFromProcessFolder; }
            set
            {
                if (value != _DeleteDuplicateFilesFromProcessFolder)
                {
                    _DeleteDuplicateFilesFromProcessFolder = value;
                    Yaio.Properties.Settings.Default.DeleteDuplicateFilesFromProcessFolder = _DeleteDuplicateFilesFromProcessFolder;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(DeleteDuplicateFilesFromProcessFolder));
                }
            }
        }

        
        bool _SearchForDuplicatesRecursivelyInYearFolder;
        public bool SearchForDuplicatesRecursivelyInYearFolder
        {
            get { return _SearchForDuplicatesRecursivelyInYearFolder; }
            set
            {
                if (value != _SearchForDuplicatesRecursivelyInYearFolder)
                {
                    _SearchForDuplicatesRecursivelyInYearFolder = value;
                    Yaio.Properties.Settings.Default.SearchForDuplicatesRecursivelyInYearFolder = _SearchForDuplicatesRecursivelyInYearFolder;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(SearchForDuplicatesRecursivelyInYearFolder));
                }
            }
        }

        string _ToFolderPath;
        public string ToFolderPath
        {
            get { return _ToFolderPath; }
            set
            {
                if (value != _ToFolderPath)
                {
                    _ToFolderPath = value;
                    Yaio.Properties.Settings.Default.ToFolderPath = _ToFolderPath;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(ToFolderPath));
                }
            }
        }

        


        string _FolderSufix;
        public string FolderSufix
        {
            get { return _FolderSufix; }
            set
            {
                if (value != _FolderSufix)
                {
                    _FolderSufix = value;
                    NotifyPropertyChanged(nameof(FolderSufix));
                }
            }
        }

        string _FromFolderPath;
        public string FromFolderPath
        {
            get { return _FromFolderPath; }
            set
            {
                if (value != _FromFolderPath)
                {
                    _FromFolderPath = value;
                    Yaio.Properties.Settings.Default.FromFolderPath = _FromFolderPath;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(FromFolderPath));
                }
            }
        }

        string _FileExtensions;
        public string FileExtensions
        {
            get { return _FileExtensions; }
            set
            {
                if (value != _FileExtensions)
                {
                    _FileExtensions = value;
                    Yaio.Properties.Settings.Default.FileExtensions = _FileExtensions;
                    Yaio.Properties.Settings.Default.Save();
                    NotifyPropertyChanged(nameof(FileExtensions));
                }
            }
        }




        int _ProgressValue;
        public int ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                if (value != _ProgressValue)
                {
                    _ProgressValue = value;
                    NotifyPropertyChanged(nameof(ProgressValue));
                }
            }
        }

        int _ProgressMaximum;
        public int ProgressMaximum
        {
            get { return _ProgressMaximum; }
            set
            {
                if (value != _ProgressMaximum)
                {
                    _ProgressMaximum = value;
                    NotifyPropertyChanged(nameof(ProgressMaximum));
                }
            }
        }

        ObservableCollection<LogViewModel> _Log;
        public ObservableCollection<LogViewModel> Log
        {
            get { return _Log; }
            set
            {
                if (value != _Log)
                {
                    _Log = value;
                    NotifyPropertyChanged(nameof(Log));
                }
            }
        }

        NotifyTaskCompletion<ProcessResult> _ProcessViewModel;
        public NotifyTaskCompletion<ProcessResult> ProcessViewModel
        {
            get { return _ProcessViewModel; }
            set
            {
                if (value != _ProcessViewModel)
                {
                    _ProcessViewModel = value;
                    _ProcessViewModel.PropertyChanged += _ProcessViewModel_PropertyChanged;
                    NotifyPropertyChanged(nameof(ProcessViewModel));
                }
            }
        }

        private void _ProcessViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
            }
        }


        #endregion

        public void CancelProcess()
        {

            if (cancelationTokenForProcess != null && _ProcessViewModel != null 
                && _ProcessViewModel.IsRunning)
            {
                cancelationTokenForProcess.Cancel();
                cancelationTokenForProcess.Dispose();
                cancelationTokenForProcess = null;
            }
        }

        internal void StartExtractingProcess()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                Log.Clear();
                NotifyPropertyChanged(nameof(Log));
            });
            ProcessViewModel = new NotifyTaskCompletion<ProcessResult>(
               ExtractInfoFromFiles.ExtractInfo(GetProcessParams()));

        }

        

        private ProcessFileParameter GetProcessParams()
        {
            cancelationTokenForProcess = new CancellationTokenSource();
            var param = new ProcessFileParameter(FromFolderPath, ToFolderPath,
                cancelationTokenForProcess.Token);

            param.FileFilter = FileExtensions.Split(',').ToList();
            param.AddLog += Param_AddLog;
            param.NrOfFilesToProcess += Param_NrOfFilesToProcess;
            param.FileIndexProcess += Param_FileIndexProcess;
            param.CreateYearDirectory = this.CreateYearDirectory;
            param.CreateMonthDirectory = this.CreateMonthDirectory;
            param.Recursive = this.Recursive;
            param.DeleteDuplicateFilesFromProcessFolder = this.DeleteDuplicateFilesFromProcessFolder;
            param.SearchForDuplicatesRecursivelyInYearFolder = this.SearchForDuplicatesRecursivelyInYearFolder;
            param.PrefixHtml = Yaio.Properties.Settings.Default.PrefixHtml;
            param.FolderSufix = this.FolderSufix;
            return param;
        }

        internal void StartProcess()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                Log.Clear();
                NotifyPropertyChanged(nameof(Log));
            });
            ProcessViewModel = new NotifyTaskCompletion<ProcessResult>(
                ArrangeFilesS.ArrangeFiles(GetProcessParams()));

            //ProcessFiles.ProcessFile(fileInfo, param);
        }

        private void Param_FileIndexProcess(int fileIndex)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.ProgressValue = fileIndex;
            });
        }

        private void Param_NrOfFilesToProcess(int nrOfFiles)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                this.ProgressMaximum = nrOfFiles;
            });
        }

        private void Param_AddLog(ProcessFileParameter.LogEntry entry)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                Log.Add(new LogViewModel(entry));
                NotifyPropertyChanged(nameof(Log));
            });
        }
    }



}
