using System.Text;
using Yaio.FileHandler;

namespace Yaio.ViewModels
{
    internal class LogViewModel : BaseClassViewModel
    {
        ProcessFileParameter.LogEntry _entry;
        public LogViewModel(ProcessFileParameter.LogEntry entry)
        {
            _entry = entry;
        }

        public string Severity
        {
            get { return _entry.Severity.ToString() ; }
        }

        public string Message
       {
            get { return _entry.Message; }
        }

        public override string ToString()
        {
            var retValue = new StringBuilder();
            retValue.Append(Severity);
            retValue.Append(" ");
            retValue.Append(Message);
            retValue.Append("\t");
            retValue.Append(_entry.SourceFile);
            retValue.Append("\t");
            retValue.Append(_entry.TargetFile);
            return retValue.ToString();

        }
    }
}