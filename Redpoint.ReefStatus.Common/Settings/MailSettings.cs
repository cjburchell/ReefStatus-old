// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Windows.Input;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    /// <summary>
    /// Mail Settings
    /// </summary>
    public class MailSettings : BindableBase
    {
        #region Constants and Fields

        /// <summary>
        /// The from.
        /// </summary>
        private string from;

        /// <summary>
        /// The last status time.
        /// </summary>
        private DateTime lastStatusTime;

        /// <summary>
        /// The password.
        /// </summary>
        private string password;

        /// <summary>
        /// The port.
        /// </summary>
        private int port;

        /// <summary>
        /// The send on alarm.
        /// </summary>
        private bool sendOnAlarm;

        /// <summary>
        /// The send on connection lost.
        /// </summary>
        private bool sendOnConnectionLost;

        /// <summary>
        /// The send on reminder.
        /// </summary>
        private bool sendOnReminder;

        /// <summary>
        /// The send short message.
        /// </summary>
        private bool sendShortMessage;

        /// <summary>
        /// The send status.
        /// </summary>
        private bool sendStatus;

        /// <summary>
        /// The send status duration.
        /// </summary>
        private int sendStatusDuration;

        /// <summary>
        /// The send status mode.
        /// </summary>
        private DateRangeMode sendStatusMode;

        /// <summary>
        /// The server.
        /// </summary>
        private string server;

        /// <summary>
        /// The test email command.
        /// </summary>
        private ICommand testEmailCommand;

        /// <summary>
        /// The to.
        /// </summary>
        private string to;

        /// <summary>
        /// The use password.
        /// </summary>
        private bool usePassword;

        /// <summary>
        /// The user name.
        /// </summary>
        private string userName;

        private bool enableSsl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MailSettings"/> class.
        /// </summary>
        public MailSettings()
        {
            this.SendOnAlarm = false;
            this.Port = 25;
            this.UsePassword = false;
            this.SendStatus = false;
            this.SendStatusDuration = 1;
            this.SendStatusMode = DateRangeMode.Days;
            this.LastStatusTime = DateTime.Now;
            this.SendShortMessage = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets from time.
        /// </summary>
        /// <value>From time.</value>
        public string From
        {
            get
            {
                return this.from;
            }

            set
            {
                if (this.from == value)
                {
                    return;
                }

                this.from = value;
                this.OnPropertyChanged(() => this.From);
            }
        }

        /// <summary>
        /// Gets or sets the last status time.
        /// </summary>
        /// <value>The last status time.</value>
        public DateTime LastStatusTime
        {
            get
            {
                return this.lastStatusTime;
            }

            set
            {
                if (this.lastStatusTime == value)
                {
                    return;
                }

                this.lastStatusTime = value;
                this.OnPropertyChanged(() => this.LastStatusTime);
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password == value)
                {
                    return;
                }

                this.password = value;
                this.OnPropertyChanged(() => this.Password);
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (this.port == value)
                {
                    return;
                }

                this.port = value;
                this.OnPropertyChanged(() => this.Port);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send on alarm].
        /// </summary>
        /// <value><c>true</c> if [send on alarm]; otherwise, <c>false</c>.</value>
        public bool SendOnAlarm
        {
            get
            {
                return this.sendOnAlarm;
            }

            set
            {
                if (this.sendOnAlarm != value)
                {
                    this.sendOnAlarm = value;
                    this.OnPropertyChanged(() => this.SendOnAlarm);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send on connection lost].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [send on connection lost]; otherwise, <c>false</c>.
        /// </value>
        public bool SendOnConnectionLost
        {
            get
            {
                return this.sendOnConnectionLost;
            }

            set
            {
                if (this.sendOnConnectionLost == value)
                {
                    return;
                }

                this.sendOnConnectionLost = value;
                this.OnPropertyChanged(() => this.SendOnConnectionLost);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send on reminder].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [send on reminder]; otherwise, <c>false</c>.
        /// </value>
        public bool SendOnReminder
        {
            get
            {
                return this.sendOnReminder;
            }

            set
            {
                if (this.sendOnReminder == value)
                {
                    return;
                }

                this.sendOnReminder = value;
                this.OnPropertyChanged(() => this.SendOnReminder);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send short message].
        /// </summary>
        /// <value><c>true</c> if [send short message]; otherwise, <c>false</c>.</value>
        public bool SendShortMessage
        {
            get
            {
                return this.sendShortMessage;
            }

            set
            {
                if (this.sendShortMessage != value)
                {
                    this.sendShortMessage = value;
                    this.OnPropertyChanged(() => this.SendShortMessage);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [send status].
        /// </summary>
        /// <value><c>true</c> if [send status]; otherwise, <c>false</c>.</value>
        public bool SendStatus
        {
            get
            {
                return this.sendStatus;
            }

            set
            {
                if (this.sendStatus != value)
                {
                    this.sendStatus = value;
                    this.OnPropertyChanged(() => this.SendStatus);
                }
            }
        }

        /// <summary>
        /// Gets or sets the duration of the send status.
        /// </summary>
        /// <value>The duration of the send status.</value>
        public int SendStatusDuration
        {
            get
            {
                return this.sendStatusDuration;
            }

            set
            {
                if (this.sendStatusDuration != value)
                {
                    this.sendStatusDuration = value;
                    this.OnPropertyChanged(() => this.SendStatusDuration);
                }
            }
        }

        /// <summary>
        /// Gets or sets the send status mode.
        /// </summary>
        /// <value>The send status mode.</value>
        public DateRangeMode SendStatusMode
        {
            get
            {
                return this.sendStatusMode;
            }

            set
            {
                if (this.sendStatusMode == value)
                {
                    return;
                }

                this.sendStatusMode = value;
                this.OnPropertyChanged(() => this.SendStatusMode);
            }
        }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string Server
        {
            get
            {
                return this.server;
            }

            set
            {
                if (this.server == value)
                {
                    return;
                }

                this.server = value;
                this.OnPropertyChanged(() => this.Server);
            }
        }

        /// <summary>
        /// Gets TestEmailCommand.
        /// </summary>
        [XmlIgnore]
        public ICommand TestEmailCommand
        {
            get
            {
                return this.testEmailCommand ?? (this.testEmailCommand = new DelegateCommand(this.TestEmail));
            }
        }

        /// <summary>
        /// Gets or sets the To time.
        /// </summary>
        /// <value>To time.</value>
        public string To
        {
            get
            {
                return this.to;
            }

            set
            {
                if (this.to == value)
                {
                    return;
                }

                this.to = value;
                this.OnPropertyChanged(() => this.To);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use password].
        /// </summary>
        /// <value><c>true</c> if [use password]; otherwise, <c>false</c>.</value>
        public bool UsePassword
        {
            get
            {
                return this.usePassword;
            }

            set
            {
                if (this.usePassword == value)
                {
                    return;
                }

                this.usePassword = value;
                this.OnPropertyChanged(() => this.UsePassword);
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                if (this.userName == value)
                {
                    return;
                }

                this.userName = value;
                this.OnPropertyChanged(() => this.UserName);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the mail message.
        /// </summary>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="htmlBody">
        /// The HTML body.
        /// </param>
        /// <param name="textBody">
        /// The text body.
        /// </param>
        /// <param name="attachments">
        /// The attachments.
        /// </param>
        public void SendMailMessage(
            string subject, string htmlBody, string textBody, Collection<Attachment> attachments)
        {
            try
            {
                var mailClient = new SmtpClient { Host = this.Server, Port = this.Port };

                if (this.UsePassword)
                {
                    mailClient.Credentials = new NetworkCredential(this.UserName, this.Password);
                }

                mailClient.EnableSsl = this.EnableSsl;

                var message = new MailMessage
                    {
                       From = new MailAddress(this.From, Language.GetResource("strReefStatus")) 
                    };

                foreach (string address in this.To.Split(new[] { ',', ';' }))
                {
                    message.To.Add(address.Trim());
                }

                message.Subject = subject;

                if (this.SendShortMessage)
                {
                    message.Body = textBody;
                }
                else
                {
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(
                        textBody, null, MediaTypeNames.Text.Plain);
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                        htmlBody, null, MediaTypeNames.Text.Html);

                    if (attachments != null)
                    {
                        foreach (Attachment item in attachments)
                        {
                            message.Attachments.Add(item);
                        }
                    }

                    message.AlternateViews.Add(plainView);
                    message.AlternateViews.Add(htmlView);
                }

                mailClient.Send(message);
            }
            catch (ArgumentException ex)
            {
                throw new ReefStatusException(5000, "Error Sending E-mail " + subject, ex);
            }
            catch (FormatException ex)
            {
                throw new ReefStatusException(5001, "Error Sending E-mail" + subject, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new ReefStatusException(5002, "Error Sending E-mail" + subject, ex);
            }
            catch (SmtpException ex)
            {
                throw new ReefStatusException(5003, "Error Sending E-mail" + subject, ex);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSsl
        {
            get
            {
                return enableSsl;
            }
            set
            {
                if (enableSsl == value)
                {
                    return;
                }

                enableSsl = value;
                this.OnPropertyChanged(() => this.EnableSsl);
            }
        }

        /// <summary>
        /// Tests this instance.
        /// </summary>
        public void TestEmail()
        {
            try
            {
                this.SendMailMessage(
                    "Test Reef Status E-mail",
                    "This is a test of the Reef status e-mail",
                    "This is a test of the Reef status e-mail",
                    null);
            }
            catch (ReefStatusException ex)
            {
                Trace.WriteLine(ex);
            }
        }

        #endregion
    }
}