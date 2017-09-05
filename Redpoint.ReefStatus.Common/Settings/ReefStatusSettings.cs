// <copyright file="ReefStatusSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

using RedPoint.ReefStatus.Common.Database;

namespace RedPoint.ReefStatus.Common.Settings
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class ReefStatusSettings : IReefStatusSettings
    {
        private CouchDataAccess dataAccess;

        public ReefStatusSettings(CouchDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;

            this.Connection = this.dataAccess.LoadConnectionSettings();
            this.Logging = this.dataAccess.LoadLoggingSettings();
            this.Mail = this.dataAccess.LoadMailSettings();
        }

        /// <summary>
        /// Gets the logging.
        /// </summary>
        /// <value>The logging.</value>
        public LoggingSettings Logging { get; }

        /// <summary>
        /// Gets the mail.
        /// </summary>
        /// <value>The mail.</value>
        public MailSettings Mail { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public ConnectionSettings Connection { get; }
    }
}
