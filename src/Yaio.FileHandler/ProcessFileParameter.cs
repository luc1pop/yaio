using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yaio.FileHandler
{


    public sealed class ProcessFileParameter
    {
        public enum LogSeverity
        {
            Error,
            Warning,
            Info
        }
        public sealed class LogEntry
        {
            public LogSeverity Severity { get; private set; }
            public string Message { get; private set; }
            public string SourceFile { get; private set; }
            public string TargetFile { get; private set; }

            public LogEntry(LogSeverity severity, string message, string sourceFile, string targetFile)
            {
                Severity = severity;
                Message = message;
                SourceFile = sourceFile;
                TargetFile = targetFile;
            }

            public override string ToString()
            {
                var retValue = new StringBuilder();
                retValue.Append(Severity.ToString());
                retValue.Append("\t");
                retValue.Append(Message);
                retValue.Append("\t");
                retValue.Append(SourceFile);
                retValue.Append("\t");
                retValue.Append(TargetFile);
                return retValue.ToString();
            }
        }
        public string PrefixHtml { get; set; }
        public System.Threading.CancellationToken CancelationToken { get; private set; }
        public int MaxWithHeight { get; set; }

        public string LogFileName { get; private set; }

        public string ToDirectoryPath { get; set; }
        public string FromDirectoryPath { get; private set; }

        public IList<string> FileFilter { get; set; }

        public bool Recursive { get; set; }
        public bool DeleteDuplicateFilesFromProcessFolder { get; set; }
        public bool SearchForDuplicatesRecursivelyInYearFolder { get; set; }

        public List<LogEntry> LogList { get; private set; }

        public bool CreateYearDirectory { get; set; }
        public bool CreateMonthDirectory { get; set; }
        public string FolderSufix { get; set; }

        public ProcessFileParameter(string fromPath, string toPath,
             System.Threading.CancellationToken cts)
        {
            if (cts == null) { throw new ArgumentNullException(nameof(cts)); }
            FromDirectoryPath = fromPath;
            ToDirectoryPath = toPath;
            Recursive = true;
            DeleteDuplicateFilesFromProcessFolder = false;
            FileFilter = new List<string>() { ".jpg" };
            LogList = new List<LogEntry>();
            CancelationToken = cts;
            LogFileName = Path.Combine(ToDirectoryPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
            this.Log(LogSeverity.Info, $"from '{FromDirectoryPath}' to '{ToDirectoryPath}'","","",true);
            MaxWithHeight = 500;

        }

        public void Log(LogSeverity severity, string message, string sourceFile, string targetFile, bool throwExIfFail = false)
        {
            var tmpEntry = new LogEntry(severity, message, sourceFile, targetFile);
            try
            {
                using (System.IO.StreamWriter file =
               new System.IO.StreamWriter(LogFileName,true))
                {
                    file.WriteLine(tmpEntry.ToString());
                }

            }
            catch (Exception)
            {
                if(throwExIfFail)
                    throw;
            }

            LogList.Add(tmpEntry);
            if (this.AddLog != null)
                this.AddLog.Invoke(tmpEntry);
        }

        public void Log(string message, string sourceFile, string targetFile)
        {
            Log(LogSeverity.Info, message, sourceFile, targetFile);
        }

        public void Log(Exception ex, string message, string sourceFile, string targetFile, LogSeverity severity = LogSeverity.Error)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(message);
            stringBuilder.Append(": ");
            stringBuilder.Append(ex.ToString());
            Log(severity, stringBuilder.ToString(), sourceFile, targetFile);
        }

        public void SetNrOfFiles(int nr)
        {
            if (NrOfFilesToProcess != null)
                NrOfFilesToProcess.Invoke(nr);
        }

        public void ProcessingFile(int fileIndex)
        {
            if (FileIndexProcess != null)
                FileIndexProcess.Invoke(fileIndex);

        }

        public event AddLogDelegate AddLog;
        public event NrOfFilesToProcessDelegate NrOfFilesToProcess;
        public event FileIndexProcessDelegate FileIndexProcess;

        public delegate void AddLogDelegate(LogEntry entry);
        public delegate void NrOfFilesToProcessDelegate(int nrOfFiles);
        public delegate void FileIndexProcessDelegate(int fileIndex);
    }


}