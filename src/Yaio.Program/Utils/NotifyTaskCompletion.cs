namespace Yaio.Utils
{
    //https://msdn.microsoft.com/en-us/magazine/dn605875.aspx

    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    /// <code>
    /// using System;
    ///using System.Net.Http;
    ///using System.Threading.Tasks;
    ///public static class MyStaticService
    ///{
    ///    public static async Task<int> CountBytesInUrlAsync(string url)
    ///    {
    ///        // Artificial delay to show responsiveness.
    ///        await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
    ///        // Download the actual data and count it.
    ///        using (var client = new HttpClient())
    ///        {
    ///            var data = await client.GetByteArrayAsync(url).ConfigureAwait(false);
    ///            return data.Length;
    ///        }
    ///    }
    ///}
    /// </code>
    /// <code>
    /// public class MainViewModel
    ///{
    ///  public MainViewModel()
    ///    {
    ///        UrlByteCount = new NotifyTaskCompletion<int>(
    ///          MyStaticService.CountBytesInUrlAsync("http://www.example.com"));
    ///    }
    ///    public NotifyTaskCompletion<int> UrlByteCount { get; private set; }
    ///}
    /// </code>
    /// <code>
    /// <Window x:Class="MainWindow"
    ///        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    ///  <Grid>
    ///    <Label Content = "{Binding UrlByteCount.Result}" />
    ///  </ Grid >
    ///</ Window >
    ///</code>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
            else
            {
                NotifyProperties();
            }
        }

        private void NotifyProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
            if (Task.IsCanceled)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (Task.IsFaulted)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
                PropertyChanged?.Invoke(this,
                  new PropertyChangedEventArgs(nameof(InnerException)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                PropertyChanged?.Invoke(this,
                  new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
            }
            NotifyProperties();
        }
        public Task<TResult> Task { get; private set; }
        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default(TResult);
            }
        }
        public TaskStatus Status { get { return Task.Status; } }
        public bool IsRunning { get {
                switch (Task.Status)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingToRun:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForChildrenToComplete:
                        return true;
                    case TaskStatus.RanToCompletion:
                    case TaskStatus.Canceled:
                    case TaskStatus.Faulted:
                    default:
                        return false;
                }
            } }
        public bool IsCompleted { get { return Task.IsCompleted; } }
        public bool IsNotCompleted { get { return !Task.IsCompleted; } }
        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }
        public bool IsCanceled { get { return Task.IsCanceled; } }
        public bool IsFaulted { get { return Task.IsFaulted; } }
        public AggregateException Exception { get { return Task.Exception; } }
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
