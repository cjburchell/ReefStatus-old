// <copyright file="Logger.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System.Diagnostics;
    using System.IO;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// The logger.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The name of the error log file
        /// </summary>
        private const string LogFileName = "log.txt";

        /// <summary>
        /// the instance of the logger
        /// </summary>
        private static Logger instance;

        /// <summary>
        /// The error log location.
        /// </summary>
        private static string errorLogLocation;

        /// <summary>
        /// Prevents a default instance of the <see cref="Logger"/> class from being created.
        /// </summary>
        private Logger()
        {
        }

        /// <summary>
        /// The on error delagate.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public delegate void OnErrorDelagate(LogMessage message);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Logger Instance
        {
            get
            {
                return instance ?? (instance = new Logger());
            }
        }

        /// <summary>
        /// Gets the error log location.
        /// </summary>
        /// <value>The error log location.</value>
        public static string ErrorLogLocation
        {
            get
            {
                if (string.IsNullOrEmpty(errorLogLocation))
                {
                    errorLogLocation = ReefStatusSettings.AppDataDir + "\\ReefStatus\\" + LogFileName;
                }

                return errorLogLocation;
            }
        }

        /// <summary>
        /// Gets or sets the on error.
        /// </summary>
        /// <value>The on error.</value>
        public OnErrorDelagate OnError { get; set; }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        public void Log(LogMessage message)
        {
            if (message.Code != 1)
            {
                try
                {
                    using (var logFile = File.AppendText(ErrorLogLocation))
                    {
                        logFile.WriteLine(message.ToString());
                        if (message.Exception != null)
                        {
                            logFile.WriteLine(message.Exception);
                        }
                    }
                }
                catch (IOException)
                {
                }
            }

            if (this.OnError != null)
            {
                this.OnError(message);
            }

            Trace.WriteLine(message);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public void LogError(ReefStatusException ex)
        {
            this.Log(new LogMessage(ex.Code, ex.Message)
                         {
                             Exception = ex
                         });
        }
    }
}
