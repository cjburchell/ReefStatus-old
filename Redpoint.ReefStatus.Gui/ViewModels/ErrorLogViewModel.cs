namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Threading;

    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    public class ErrorLogViewModel : BindableBase
    {
        private readonly Dispatcher dispatcher;


        /// <summary>
        /// Initializes the <see cref="ErrorLogViewModel"/> class.
        /// </summary>
        static ErrorLogViewModel()
        {
            Instance = new ErrorLogViewModel();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ErrorLogViewModel"/> class from being created.
        /// </summary>
        private ErrorLogViewModel()
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.ErrorLogContent = new ObservableCollection<LogMessage>();
            Logger.Instance.OnError += this.LoggerOnError;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ErrorLogViewModel Instance { get; private set; }

        /// <summary>
        /// Gets the content of the error log.
        /// </summary>
        /// <value>
        /// The content of the error log.
        /// </value>
        public ObservableCollection<LogMessage> ErrorLogContent { get; private set; }

        /// <summary>
        /// Starups this instance.
        /// </summary>
        public static void Starup()
        {
        }

        /// <summary>
        /// Loggers the on error.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LoggerOnError(LogMessage message)
        {
            if (message.Code != 1)
            {
                if (!this.dispatcher.CheckAccess())
                {
                    this.dispatcher.BeginInvoke(new Action(() => this.LoggerOnError(message)));
                }
                else
                {
                    this.ErrorLogContent.Add(message);
                    if (this.ErrorLogContent.Count > 200)
                    {
                        this.ErrorLogContent.RemoveAt(0);
                    }
                }
            }
        }
    }
}
