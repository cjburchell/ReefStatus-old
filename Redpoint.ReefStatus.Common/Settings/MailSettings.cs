// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using RedPoint.ReefStatus.Common.Database;

    /// <summary>
    /// Mail Settings
    /// </summary>
    public class MailSettings : CouchDocument
    {
        /// <summary>
        /// Gets or sets from time.
        /// </summary>
        /// <value>From time.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Gets or sets a value indicating whether [send on alarm].
        /// </summary>
        /// <value><c>true</c> if [send on alarm]; otherwise, <c>false</c>.</value>
        public bool SendOnAlarm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send on connection lost].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [send on connection lost]; otherwise, <c>false</c>.
        /// </value>
        public bool SendOnConnectionLost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send on reminder].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send on reminder]; otherwise, <c>false</c>.
        /// </value>
        public bool SendOnReminder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send short message].
        /// </summary>
        /// <value><c>true</c> if [send short message]; otherwise, <c>false</c>.</value>
        public bool SendShortMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [send status].
        /// </summary>
        /// <value><c>true</c> if [send status]; otherwise, <c>false</c>.</value>
        public bool SendStatus { get; set; }

        /// <summary>
        /// Gets or sets the duration of the send status.
        /// </summary>
        /// <value>The duration of the send status.</value>
        public int SendStatusDuration { get; set; } = 1;

        /// <summary>
        /// Gets or sets the send status mode.
        /// </summary>
        /// <value>The send status mode.</value>
        public DateRangeMode SendStatusMode { get; set; } = DateRangeMode.Days;

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the To time.
        /// </summary>
        /// <value>To time.</value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use password].
        /// </summary>
        /// <value><c>true</c> if [use password]; otherwise, <c>false</c>.</value>
        public bool UsePassword { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSsl { get; set; }
    }
}