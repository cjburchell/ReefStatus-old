// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataService.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Core
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Windows.Forms;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.WebServer;

    using Reminder = RedPoint.ReefStatus.Common.ProfiLux.Reminder;

    /// <summary>
    /// Data Service
    /// </summary>
    public class DataService : ICommandReply
    {
        #region Constants and Fields

        /// <summary>
        /// The timer log.
        /// </summary>
        private readonly Timer timerLog = new Timer();

        /// <summary>
        /// The web server.
        /// </summary>
        private WebInterface webServer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="DataService"/> class. 
        /// </summary>
        static DataService()
        {
            Instance = new DataService();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DataService"/> class from being created. 
        /// </summary>
        private DataService()
        {
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The log item deligate.
        /// </summary>
        /// <param name="now">
        /// The now.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public delegate void LogItemDeligate(DateTime now, Controller Controller);

        /// <summary>
        /// The update display text deligate.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="viewText">The view text.</param>
        /// <param name="statusDisplay">The status display.</param>
        /// <param name="Controller">The Controller.</param>
        public delegate void UpdateDisplayTextDeligate(string status, string viewText, Image statusDisplay , Controller Controller);

        /// <summary>
        /// The update status deligate.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public delegate void UpdateStatusDeligate(Controller Controller);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DataService Instance { get; private set; }

        /// <summary>
        /// Gets or sets the on log item.
        /// </summary>
        /// <value>The on log item.</value>
        public LogItemDeligate OnLogItem { get; set; }

        /// <summary>
        /// Gets or sets the on update display text.
        /// </summary>
        /// <value>The on update display text.</value>
        public UpdateDisplayTextDeligate OnUpdateDisplayText { get; set; }

        /// <summary>
        /// Gets or sets the on update status.
        /// </summary>
        /// <value>The on update status.</value>
        public UpdateStatusDeligate OnUpdateStatus { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the status email.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public static void SendStatusEmail(string message)
        {
            foreach (var Controller in ReefStatusSettings.Instance.Controllers)
            {
                string htmlBody = string.Empty;
                string body = string.Empty;

                if (!string.IsNullOrEmpty(message))
                {
                    htmlBody += message;
                    body += message;
                }

                if (Controller.Info.Alarm == CurrentState.On)
                {
                    string reason = FindAlarmReason(Controller);

                    htmlBody += "<h1>" + Language.GetResource("strAlarmDetected") + DateTime.Now + "</h1>";
                    htmlBody += "<br/>" + Language.GetResource("strReason") + "<br/>" + reason.Replace("\n", "<br/>") +
                                "<br/>";

                    body += Language.GetResource("strAlarmDetected") + DateTime.Now + "\n";
                    body += "\n" + Language.GetResource("strReason") + "\n" + reason + "\n";
                }

                string subject = Language.GetResource("strStatusUpdate") + " " + Controller.Name;

                htmlBody += HtmlStatusTable(Controller);
                body += CreateTextStatusTable(Controller);

                Collection<Attachment> attatchments = CreateGraphAttachments(Controller);

                htmlBody = attatchments.Aggregate(
                    htmlBody, (current, attachment) => current + ("<img src='cid:" + attachment.ContentId + "'>"));

                ReefStatusSettings.Instance.Mail.SendMailMessage(subject, htmlBody, body, attatchments);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.timerLog.Tick += TimerLogTick;
            this.timerLog.Interval = ReefStatusSettings.Instance.Logging.LogInterval * 60000;

            if (ReefStatusSettings.Instance.Web.Enable)
            {
                this.StartWebServer();
            }

            this.timerLog.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.timerLog.Stop();
            this.StopWebServer();
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        public void UpdateSettings()
        {
            this.timerLog.Interval = ReefStatusSettings.Instance.Logging.LogInterval * 60000;
            this.UpdateWebInterface();
        }

        #endregion

        #region Implemented Interfaces

        #region ICommandReply

        /// <summary>
        /// Logs the data.
        /// </summary>
        /// <param name="loggedTime">The logged time.</param>
        /// <param name="Controller">The Controller.</param>
        /// <param name="callback">The callback.</param>
        public void LogData(DateTime loggedTime, Controller Controller, IUpdateProgress callback)
        {
            try
            {
                using (IDataAccess access = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    if (access != null)
                    {
                        access.AddLog(Controller.Items, loggedTime, Controller.Id, callback);

                        if (ReefStatusSettings.Instance.Logging.LimitDatabase)
                        {
                            DateTime from;

                            switch (ReefStatusSettings.Instance.Logging.LimitDatabaseMode)
                            {
                                case DateRangeMode.Days:
                                    from =
                                        ReefStatusSettings.Instance.Logging.LastArchiveTime.AddDays(
                                            -ReefStatusSettings.Instance.Logging.LimitDatabaseDuration);
                                    break;
                                case DateRangeMode.Weeks:
                                    from =
                                        ReefStatusSettings.Instance.Logging.LastArchiveTime.AddDays(
                                            -ReefStatusSettings.Instance.Logging.LimitDatabaseDuration * 7);
                                    break;
                                case DateRangeMode.Months:
                                    from =
                                        ReefStatusSettings.Instance.Logging.LastArchiveTime.AddMonths(
                                            -ReefStatusSettings.Instance.Logging.LimitDatabaseDuration);
                                    break;
                                case DateRangeMode.Hours:
                                    from =
                                        ReefStatusSettings.Instance.Logging.LastArchiveTime.AddHours(
                                            -ReefStatusSettings.Instance.Logging.LimitDatabaseDuration);
                                    break;
                                default:
                                    throw new ReefStatusException(
                                        1001,
                                        "Unknown Limit Mode " + ReefStatusSettings.Instance.Logging.LimitDatabaseMode);
                            }

                            access.RemoveLogs(from);
                        }
                    }

                }

                if (ReefStatusSettings.Instance.Logging.ArchiveDatabase &&
                    GetNextTime(
                        ReefStatusSettings.Instance.Logging.ArchiveDatabaseMode, 
                        ReefStatusSettings.Instance.Logging.ArchiveDatabaseDuration, 
                        ReefStatusSettings.Instance.Logging.LastArchiveTime) < DateTime.Now)
                {
                    string archiveLocation = ReefStatusSettings.AppDataDir + "\\ReefStatus\\Archive";

                    if (!Directory.Exists(archiveLocation))
                    {
                        Directory.CreateDirectory(archiveLocation);
                    }

                    ReefStatusSettings.Instance.Logging.Connection.Backup(archiveLocation);

                    ReefStatusSettings.Instance.Logging.LastArchiveTime = DateTime.Now;
                }

                ReefStatusSettings.Instance.Save();
            }
            catch (IOException ex)
            {
                Logger.Instance.Log(new LogMessage(1001, "Error Archiving Data") { Exception = ex });
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            if (this.OnLogItem != null)
            {
                this.OnLogItem(loggedTime, Controller);
            }
        }

        /// <summary>
        /// Called when [error in connection].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="isError">if set to <c>true</c> [is error].</param>
        public void OnErrorInConnection(Exception exception, bool isError)
        {
            try
            {
                if (ReefStatusSettings.Instance.Mail.SendOnConnectionLost)
                {
                    if (isError)
                    {
                        string errorMessage = "Reef Status has lost connection to the Controller! : " + exception.Message;
                        ReefStatusSettings.Instance.Mail.SendMailMessage(
                            "Lost Connection", errorMessage, errorMessage, null);
                    }
                    else
                    {
                        string errorMessage = "Reef Status has regained connection to the Controller!";
                        ReefStatusSettings.Instance.Mail.SendMailMessage(
                            "Connection Established", errorMessage, errorMessage, null);
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(
                    new LogMessage(1003, "Unable to send mail for lost connection") { Exception = ex });
            }
        }

        /// <summary>
        /// Updates the display text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="viewText">The view text.</param>
        /// <param name="statusDisplay">The status display.</param>
        /// <param name="Controller">The Controller.</param>
        public void UpdateDisplayText(string status, string viewText, Image statusDisplay ,Controller Controller)
        {
            if (this.OnUpdateDisplayText != null)
            {
                this.OnUpdateDisplayText(status, viewText, statusDisplay, Controller);
            }
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public void UpdateStatus(Controller Controller)
        {
            CheckAlarm(Controller);

            if (this.OnUpdateStatus != null)
            {
                this.OnUpdateStatus(Controller);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks the alarm.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private static void CheckAlarm(Controller Controller)
        {
            try
            {
                if (ReefStatusSettings.Instance.Mail.SendOnAlarm)
                {
                    CheckReminders(Controller.Info);

                    if (Controller.Info.Alarm == CurrentState.On)
                    {
                        if (!Controller.SentAlarmEmail)
                        {
                            SendAlarmEmail(Controller, false);
                        }

                        Controller.SentAlarmEmail = true;
                    }
                    else
                    {
                        if (Controller.SentAlarmEmail)
                        {
                            SendAlarmEmail(Controller, true);
                        }

                        Controller.SentAlarmEmail = false;
                    }
                }

                if (ReefStatusSettings.Instance.Mail.SendStatus &&
                    GetNextTime(
                        ReefStatusSettings.Instance.Mail.SendStatusMode, 
                        ReefStatusSettings.Instance.Mail.SendStatusDuration, 
                        ReefStatusSettings.Instance.Mail.LastStatusTime) < DateTime.Now)
                {
                    SendStatusEmail(string.Empty);
                    ReefStatusSettings.Instance.Mail.LastStatusTime = DateTime.Now;
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(new LogMessage(1002, "Error in Checking Alarm") { Exception = ex });
            }
        }

        /// <summary>
        /// Checks the reminders.
        /// </summary>
        /// <param name="info">
        /// The controller info.
        /// </param>
        private static void CheckReminders(Info info)
        {
            foreach (Reminder reminder in info.Reminders)
            {
                if (!reminder.SentMail && reminder.IsOverdue && ReefStatusSettings.Instance.Mail.SendOnReminder)
                {
                    string subject = Language.GetResource("strReminder") + reminder.Text;
                    string body = string.Format(
                        CultureInfo.CurrentCulture, Language.GetResource("strReminderEmailMessage"), reminder.Text);
                    ReefStatusSettings.Instance.Mail.SendMailMessage(subject, body, body, null);
                    reminder.SentMail = true;
                }
            }
        }

        /// <summary>
        /// Creates the graph attachments.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of attachments
        /// </returns>
        private static Collection<Attachment> CreateGraphAttachments(Controller Controller)
        {
            var attatchments = new Collection<Attachment>();

            foreach (var settings in Controller.Items.OfType<Probe>())
            {
                var datapoints = GetDatapoints(settings.Range, settings, Controller);
                foreach (DataPoint dataPoint in datapoints)
                {
                    dataPoint.Value = settings.ConvertValue(dataPoint.Value);
                }
            }

            return attatchments;
        }

        /// <summary>
        /// Creates the text status table.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// the text version of the status table
        /// </returns>
        private static string CreateTextStatusTable(Controller Controller)
        {
            string body = Controller.Items.OfType<SensorInfo>().Aggregate(
                string.Empty, 
                (current, settings) =>
                current +
                string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\n", settings.DisplayName, settings.ValueWithUnits));

            if (!ReefStatusSettings.Instance.Mail.SendShortMessage)
            {
                body = Controller.Items.OfType<LevelSensor>().Aggregate(
                    body, 
                    (current, settings) =>
                    current +
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        "{0}\t{1}\t{2}\n", 
                        settings.DisplayName, 
                        settings.OpertationMode,
                        settings.ConvertedValue));

                body = Controller.Items.OfType<SPort>().Aggregate(
                    body, 
                    (current, settings) =>
                    current +
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        "{0}\t{1}\t{2}\n", 
                        settings.DisplayName, 
                        settings.Units, 
                        settings.Value));

                body = Controller.Items.OfType<LPort>().Aggregate(
                    body, 
                    (current, settings) =>
                    current +
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        "{0}\t{1}\t{2}\n", 
                        settings.DisplayName, 
                        settings.Mode, 
                        settings.ValueWithUnits));

                body = Controller.Items.OfType<UserInfo>().Aggregate(
                    body, 
                    (current, settings) =>
                    current +
                    string.Format(
                        CultureInfo.CurrentCulture, 
                        "{0}\t{1}\t{2}\n", 
                        settings.DisplayName, 
                        settings.Units, 
                        settings.Value));
            }

            return body;
        }

        /// <summary>
        /// Finds the alarm reason.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// the string representation of the alarm
        /// </returns>
        private static string FindAlarmReason(Controller Controller)
        {
            string reason = string.Empty;

            foreach (var probe in Controller.Items.OfType<Probe>())
            {
                if (probe.IsAlarmOn == CurrentState.On && probe.AlarmEnable)
                {
                    if ((double)probe.Value > (probe.NominalValue + probe.AlarmDeviation))
                    {
                        reason += string.Format(
                            CultureInfo.CurrentCulture, Language.GetResource("strtooHigh") + "\n", probe.DisplayName);
                    }
                    else if ((double)probe.Value < (probe.NominalValue - probe.AlarmDeviation))
                    {
                        reason += string.Format(
                            CultureInfo.CurrentCulture, Language.GetResource("strtooLow") + "\n", probe.DisplayName);
                    }
                    else
                    {
                        reason += string.Format(CultureInfo.CurrentCulture, "Alarm on {0}" + "\n", probe.DisplayName);
                    }
                }
            }

            reason =
                Controller.Items.OfType<LevelSensor>().Where(sensor => sensor.IsAlarmOn == CurrentState.On).
                    Aggregate(
                        reason, 
                        (current, sensor) =>
                        current +
                        string.Format(
                            CultureInfo.CurrentCulture, Language.GetResource("strTimeout") + "\n", sensor.DisplayName));

            if (string.IsNullOrEmpty(reason))
            {
                reason = "Unknown";
            }

            return reason;
        }

        /// <summary>
        /// Gets the datapoints.
        /// </summary>
        /// <param name="graphRange">
        /// The graph range.
        /// </param>
        /// <param name="item">
        /// The type of data point.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of data points for the graph
        /// </returns>
        private static Collection<DataPoint> GetDatapoints(GraphRange graphRange, BaseInfo item, Controller Controller)
        {
            var points = new Collection<DataPoint>();

            try
            {
                using (IDataAccess data = ReefStatusSettings.Instance.Logging.Connection.Create())
                {
                    switch (graphRange)
                    {
                        case GraphRange.All:
                            points = data.GetDataPoints(item.GraphId, false, Controller.Id);
                            break;
                        case GraphRange.Year:
                            points = data.GetDataPoints(item.GraphId, DateTime.Now.AddYears(-1), false, Controller.Id);
                            break;
                        case GraphRange.Month:
                            points = data.GetDataPoints(item.GraphId, DateTime.Now.AddMonths(-1), false, Controller.Id);
                            break;
                        case GraphRange.Week:
                            points = data.GetDataPoints(item.GraphId, DateTime.Now.AddDays(-7), false, Controller.Id);
                            break;
                        case GraphRange.Day:
                            points = data.GetDataPoints(item.GraphId, DateTime.Now.AddDays(-1), false, Controller.Id);
                            break;
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }

            return points;
        }

        /// <summary>
        /// Gets the next acrhive time.
        /// </summary>
        /// <param name="mode">
        /// The mode of the time.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <param name="lastTime">
        /// The last time.
        /// </param>
        /// <returns>
        /// The next archive time
        /// </returns>
        private static DateTime GetNextTime(DateRangeMode mode, int duration, DateTime lastTime)
        {
            switch (mode)
            {
                case DateRangeMode.Days:
                    return lastTime.AddDays(duration);
                case DateRangeMode.Weeks:
                    return lastTime.AddDays(duration * 7);
                case DateRangeMode.Months:
                    return lastTime.AddMonths(duration);
                case DateRangeMode.Hours:
                    return lastTime.AddHours(duration);
                default:
                    throw new ReefStatusException(1002, "Unknown Mode " + mode);
            }
        }

        /// <summary>
        /// HTMLs the status table.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// the HTML Text
        /// </returns>
        private static string HtmlStatusTable(Controller Controller)
        {
            string htmlBody = "<table>";
            htmlBody += string.Format(CultureInfo.CurrentCulture, "<tr><td>Name</td><td>Mode</td><td>Value</td></tr>");

            htmlBody = Controller.Items.OfType<Probe>().Aggregate(
                htmlBody, 
                (current, probe) =>
                current +
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", 
                    probe.DisplayName, 
                    probe.Units, 
                    probe.ConvertedValue));

            htmlBody = Controller.Items.OfType<LevelSensor>().Aggregate(
                htmlBody, 
                (current, probe) =>
                current +
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", 
                    probe.DisplayName, 
                    probe.Units, 
                    probe.Value));

            htmlBody = Controller.Items.OfType<SPort>().Where(device => !device.IsConstant).Aggregate(
                htmlBody, 
                (current, device) =>
                current +
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "<tr><td>{0}</td><td>{1} ({2})</td><td>{3}</td></tr>", 
                    device.DisplayName, 
                    device.Mode, 
                    device.Units, 
                    device.Value));

            htmlBody = Controller.Items.OfType<LPort>().Where(device => !device.IsConstant).Aggregate(
                htmlBody, 
                (current, device) =>
                current +
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "<tr><td>{0}</td><td>{1} ({2})</td><td>{3}</td></tr>", 
                    device.DisplayName, 
                    device.Mode, 
                    device.Units, 
                    device.Value));

            htmlBody = Controller.Items.OfType<UserInfo>().Aggregate(
                htmlBody, 
                (current, userValue) =>
                current +
                string.Format(
                    CultureInfo.CurrentCulture, 
                    "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", 
                    userValue.DisplayName, 
                    userValue.Units, 
                    userValue.Value));

            htmlBody += "</table>";
            return htmlBody;
        }

        /// <summary>
        /// Sends the alarm email.
        /// </summary>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <param name="cleared">
        /// if set to <c>true</c> [cleared].
        /// </param>
        private static void SendAlarmEmail(Controller controller, bool cleared)
        {
            string subject;
            string htmlBody;
            string body;

            if (!cleared)
            {
                subject = Language.GetResource("strAlarm") + " " + controller.Name;
                htmlBody = "<h1>" + Language.GetResource("strAlarmDetected") + DateTime.Now + "</h1>";
                body = Language.GetResource("strAlarmDetected") + DateTime.Now + "\n";

                string reason = FindAlarmReason(controller);
                htmlBody += "<br/>" + Language.GetResource("strReason") + "<br/>" + reason.Replace("\n", "<br/>") +
                            "<br/>";
                body += "\n" + Language.GetResource("strReason") + "\n" + reason + "\n";
            }
            else
            {
                subject = Language.GetResource("strAlarmCleared") + " " + controller.Name;
                htmlBody = "<h1>" + Language.GetResource("strAlarmCleared") + " " + DateTime.Now + "</h1>";
                body = Language.GetResource("strAlarmCleared") + " " + DateTime.Now + "\n";
            }

            htmlBody += HtmlStatusTable(controller);
            body += CreateTextStatusTable(controller);

            Collection<Attachment> attatchments = CreateGraphAttachments(controller);

            htmlBody = attatchments.Aggregate(
                htmlBody, (current, attachment) => current + ("<img src='cid:" + attachment.ContentId + "'>"));

            ReefStatusSettings.Instance.Mail.SendMailMessage(subject, htmlBody, body, attatchments);
        }

        /// <summary>
        /// Handles the Tick event of the timerLog control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void TimerLogTick(object sender, EventArgs e)
        {
            foreach (var Controller in ReefStatusSettings.Instance.Controllers)
            {
                Controller.Commands.LogParameters();
            }
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        private void StartWebServer()
        {
            this.StopWebServer();
            try
            {
                this.webServer = new WebInterface();
            }
            catch (ReefStatusException ex)
            {
                
                Logger.Instance.LogError(ex);
            } 
        }

        /// <summary>
        /// Stops the web server.
        /// </summary>
        private void StopWebServer()
        {
            if (this.webServer != null)
            {
                this.webServer.Stop();
                this.webServer = null;
            }
        }

        /// <summary>
        /// Updates the web interface.
        /// </summary>
        private void UpdateWebInterface()
        {
            if (ReefStatusSettings.Instance.Web.Enable)
            {
                this.StartWebServer();
            }
            else
            {
                this.StopWebServer();
            }
        }

        #endregion
    }
}