namespace RedPoint.ReefStatus.Common.Service
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Net.Mime;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;
    using RedPoint.ReefStatus.Common.Settings;

    public class AlertService
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [sent alarm email].
        /// </summary>
        /// <value><c>true</c> if [sent alarm email]; otherwise, <c>false</c>.</value>
        public bool SentAlarmEmail { get; set; }

        /// <summary>
        ///     Gets or sets the last status time.
        /// </summary>
        /// <value>The last status time.</value>
        public DateTime LastStatusTime { get; set; }

        /// <summary>
        ///     Checks the alarm.
        /// </summary>
        /// <param name="controller">
        ///     The Controller.
        /// </param>
        public void CheckAlarm(Controller controller, MailSettings settings)
        {
            try
            {
                if (settings.SendOnAlarm)
                {
                    CheckReminders(controller.Info, settings);

                    if (controller.Info.Alarm == CurrentState.On)
                    {
                        if (!this.SentAlarmEmail)
                        {
                            SendAlarmEmail(controller, false, settings);
                        }

                        this.SentAlarmEmail = true;
                    }
                    else
                    {
                        if (this.SentAlarmEmail)
                        {
                            SendAlarmEmail(controller, true, settings);
                        }

                        this.SentAlarmEmail = false;
                    }
                }

                if (settings.SendStatus && GetNextTime(settings.SendStatusMode, settings.SendStatusDuration, this.LastStatusTime) < DateTime.Now)
                {
                    SendStatusEmail(string.Empty, controller, settings);
                    this.LastStatusTime = DateTime.Now;
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
        ///     Sends the status email.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        public static void SendStatusEmail(string message, Controller controller, MailSettings settings)
        {
            {
                var htmlBody = string.Empty;
                var body = string.Empty;

                if (!string.IsNullOrEmpty(message))
                {
                    htmlBody += message;
                    body += message;
                }

                if (controller.Info.Alarm == CurrentState.On)
                {
                    var reason = FindAlarmReason(controller);

                    htmlBody += "<h1>" + "Alarm Detected" + DateTime.Now + "</h1>";
                    htmlBody += "<br/>" + "Reason" + "<br/>" + reason.Replace("\n", "<br/>") + "<br/>";

                    body += "Alarm Detected" + DateTime.Now + "\n";
                    body += "\n" + "Reason" + "\n" + reason + "\n";
                }

                var subject = "Status Update" + " " + "Controler";

                htmlBody += HtmlStatusTable(controller);
                body += CreateTextStatusTable(controller, settings);

                SendMailMessage(settings, subject, htmlBody, body);
            }
        }

        /// <summary>
        ///     Sends the mail message.
        /// </summary>
        /// <param name="subject">
        ///     The subject.
        /// </param>
        /// <param name="htmlBody">
        ///     The HTML body.
        /// </param>
        /// <param name="textBody">
        ///     The text body.
        /// </param>
        /// <param name="attachments">
        ///     The attachments.
        /// </param>
        private static void SendMailMessage(MailSettings settings, string subject, string htmlBody, string textBody, Collection<Attachment> attachments = null)
        {
            try
            {
                var mailClient = new SmtpClient { Host = settings.Server, Port = settings.Port };

                if (settings.UsePassword)
                {
                    mailClient.Credentials = new NetworkCredential(settings.UserName, settings.Password);
                }

                mailClient.EnableSsl = settings.EnableSsl;

                var message = new MailMessage { From = new MailAddress(settings.From, "ReefStatus") };

                foreach (var address in settings.To.Split(',', ';'))
                {
                    message.To.Add(address.Trim());
                }

                message.Subject = subject;

                if (settings.SendShortMessage)
                {
                    message.Body = textBody;
                }
                else
                {
                    var plainView = AlternateView.CreateAlternateViewFromString(textBody, null, MediaTypeNames.Text.Plain);
                    var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                    if (attachments != null)
                    {
                        foreach (var item in attachments)
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
        ///     Sends the alarm email.
        /// </summary>
        /// <param name="controller">
        ///     The Controller.
        /// </param>
        /// <param name="cleared">
        ///     if set to <c>true</c> [cleared].
        /// </param>
        private static void SendAlarmEmail(Controller controller, bool cleared, MailSettings settings)
        {
            string subject;
            string htmlBody;
            string body;

            if (!cleared)
            {
                subject = "Alarm";
                htmlBody = "<h1>" + "Alarm Detected" + DateTime.Now + "</h1>";
                body = "Alarm Detected" + DateTime.Now + "\n";

                var reason = FindAlarmReason(controller);
                htmlBody += "<br/>" + "Reason" + "<br/>" + reason.Replace("\n", "<br/>") + "<br/>";
                body += "\n" + "Reason" + "\n" + reason + "\n";
            }
            else
            {
                subject = "Alarm Cleared";
                htmlBody = "<h1>" + "Alarm Cleared" + " " + DateTime.Now + "</h1>";
                body = "Alarm Cleared" + " " + DateTime.Now + "\n";
            }

            htmlBody += HtmlStatusTable(controller);
            body += CreateTextStatusTable(controller, settings);

            SendMailMessage(settings, subject, htmlBody, body);
        }

        /// <summary>
        ///     HTMLs the status table.
        /// </summary>
        /// <param name="Controller">
        ///     The Controller.
        /// </param>
        /// <returns>
        ///     the HTML Text
        /// </returns>
        private static string HtmlStatusTable(Controller Controller)
        {
            var htmlBody = "<table>";
            htmlBody += string.Format(CultureInfo.CurrentCulture, "<tr><td>Name</td><td>Mode</td><td>Value</td></tr>");

            htmlBody = Controller.Probes.Aggregate(htmlBody, (current, probe) => current + string.Format(CultureInfo.CurrentCulture, "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", probe.DisplayName, probe.Units, probe.ConvertedValue));

            htmlBody = Controller.LevelSensors.Aggregate(htmlBody, (current, probe) => current + string.Format(CultureInfo.CurrentCulture, "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", probe.DisplayName, probe.Units, probe.Value));

            htmlBody = Controller.SPorts.Where(device => !device.IsConstant).Aggregate(htmlBody, (current, device) => current + string.Format(CultureInfo.CurrentCulture, "<tr><td>{0}</td><td>{1} ({2})</td><td>{3}</td></tr>", device.DisplayName, device.DeviceMode, device.Units, device.Value));

            htmlBody = Controller.LPorts.Where(device => !device.IsConstant).Aggregate(htmlBody, (current, device) => current + string.Format(CultureInfo.CurrentCulture, "<tr><td>{0}</td><td>{1} ({2})</td><td>{3}</td></tr>", device.DisplayName, device.DeviceMode, device.Units, device.Value));

            htmlBody = Controller.UserInfo.Aggregate(htmlBody, (current, userValue) => current + string.Format(CultureInfo.CurrentCulture, "<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", userValue.DisplayName, userValue.Units, userValue.Value));

            htmlBody += "</table>";
            return htmlBody;
        }

        /// <summary>
        ///     Creates the text status table.
        /// </summary>
        /// <param name="Controller">
        ///     The Controller.
        /// </param>
        /// <returns>
        ///     the text version of the status table
        /// </returns>
        private static string CreateTextStatusTable(Controller Controller, MailSettings reefStatusSettings)
        {
            var body = Controller.Probes.Aggregate(string.Empty, (current, settings) => current + string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\n", settings.DisplayName, settings.ConvertedValue));

            if (!reefStatusSettings.SendShortMessage)
            {
                body = Controller.LevelSensors.Aggregate(body, (current, settings) => current + string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\t{2}\n", settings.DisplayName, settings.OpertationMode, settings.Value));

                body = Controller.SPorts.Aggregate(body, (current, settings) => current + string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\t{2}\n", settings.DisplayName, settings.Units, settings.Value));

                body = Controller.LPorts.Aggregate(body, (current, settings) => current + string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\t{2}\n", settings.DisplayName, settings.DeviceMode, settings.Value));

                body = Controller.UserInfo.Aggregate(body, (current, settings) => current + string.Format(CultureInfo.CurrentCulture, "{0}\t{1}\t{2}\n", settings.DisplayName, settings.Units, settings.Value));
            }

            return body;
        }

        /// <summary>
        ///     Finds the alarm reason.
        /// </summary>
        /// <param name="Controller">
        ///     The Controller.
        /// </param>
        /// <returns>
        ///     the string representation of the alarm
        /// </returns>
        private static string FindAlarmReason(Controller Controller)
        {
            var reason = string.Empty;

            foreach (var probe in Controller.Probes)
            {
                if (probe.IsAlarmOn == CurrentState.On && probe.AlarmEnable)
                {
                    if (probe.Value > probe.NominalValue + probe.AlarmDeviation)
                    {
                        reason += $"{probe.DisplayName} is too high" + "\n";
                    }
                    else if (probe.Value < probe.NominalValue - probe.AlarmDeviation)
                    {
                        reason += $"{probe.DisplayName} is too low" + "\n";
                    }
                    else
                    {
                        reason += $"Alarm on {probe.DisplayName}" + "\n";
                    }
                }
            }

            reason = Controller.LevelSensors.Where(sensor => sensor.IsAlarmOn == CurrentState.On).Aggregate(reason, (current, sensor) => current + string.Format(CultureInfo.CurrentCulture, "Timeout" + "\n", sensor.DisplayName));

            if (string.IsNullOrEmpty(reason))
            {
                reason = "Unknown";
            }

            return reason;
        }

        /// <summary>
        ///     Called when [error in connection].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="isError">if set to <c>true</c> [is error].</param>
        public void OnErrorInConnection(Exception exception, bool isError, IReefStatusSettings settings)
        {
            try
            {
                if (settings.Mail.SendOnConnectionLost)
                {
                    if (isError)
                    {
                        var errorMessage = "Reef Status has lost connection to the Controller! : " + exception.Message;
                        SendMailMessage(settings.Mail, "Lost Connection", errorMessage, errorMessage, null);
                    }
                    else
                    {
                        var errorMessage = "Reef Status has regained connection to the Controller!";
                        SendMailMessage(settings.Mail, "Connection Established", errorMessage, errorMessage, null);
                    }
                }
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(new LogMessage(1003, "Unable to send mail for lost connection") { Exception = ex });
            }
        }

        /// <summary>
        ///     Gets the next acrhive time.
        /// </summary>
        /// <param name="mode">
        ///     The mode of the time.
        /// </param>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        /// <param name="lastTime">
        ///     The last time.
        /// </param>
        /// <returns>
        ///     The next archive time
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
        ///     Checks the reminders.
        /// </summary>
        /// <param name="info">
        ///     The controller info.
        /// </param>
        private static void CheckReminders(Info info, MailSettings settings)
        {
            foreach (var reminder in info.Reminders)
            {
                if (!reminder.SentMail && reminder.IsOverdue && settings.SendOnReminder)
                {
                    var subject = "Reminder" + reminder.Text;
                    var body = string.Format(CultureInfo.CurrentCulture, "Reminder", reminder.Text);
                    SendMailMessage(settings, subject, body, body);
                    reminder.SentMail = true;
                }
            }
        }
    }
}