// <copyright file="Logger.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common
{
    using System;

    using log4net;

    /// <summary>
    /// The logger.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// the instance of the logger
        /// </summary>
        private static Logger instance;

        private readonly ILog log = LogManager.GetLogger("ReefStatus");

        /// <summary>
        /// Prevents a default instance of the <see cref="Logger"/> class from being created.
        /// </summary>
        private Logger()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Logger Instance => instance ?? (instance = new Logger());

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
                if (message.Exception != null)
                {
                    this.log.Error(message, message.Exception);
                }
                else
                {
                    this.log.Error(message);
                }
            }
            else
            {
                this.log.Debug(message);
            }
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

        public void LogError(Exception ex)
        {
            this.Log(new LogMessage(404, ex.Message)
            {
                Exception = ex
            });
        }
    }
}
