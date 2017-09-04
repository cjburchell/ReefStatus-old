

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System.IO;
    using System.Text;

    using HttpServer;
    using HttpServer.Exceptions;
    using HttpServer.MVC.Controllers;

    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.Settings;

    public class SettingsController : RequestController
    {
        private readonly IReefStatusSettings settings;

        private readonly IDataAccess dataAccess;

        public SettingsController(IReefStatusSettings settings, IDataAccess dataAccess)
        {
            this.settings = settings;
            this.dataAccess = dataAccess;
        }

        /// <summary>
        /// Make a clone of this controller
        /// </summary>
        /// <returns>a new controller with the same base information as this one.</returns>
        public override object Clone()
        {
            return new SettingsController(this.settings, this.dataAccess);
        }

        public string Connection()
        {
            switch (this.Request.Method)
            {
                case Method.Put:
                    var newSettings = this.GetBody<ConnectionSettings>();

                    this.settings.Connection.ConnectionType = newSettings.ConnectionType;
                    this.settings.Connection.BaudRate = newSettings.BaudRate;
                    this.settings.Connection.Timeout = newSettings.Timeout;
                    this.settings.Connection.Address = newSettings.Address;
                    this.settings.Connection.Port = newSettings.Port;
                    this.settings.Connection.ControllerAddress = newSettings.ControllerAddress;
                    this.settings.Connection.ComPort = newSettings.ComPort;
                    this.settings.Connection.Password = newSettings.Password;
                    this.settings.Connection.GetAll = newSettings.GetAll;
                    this.settings.Connection.UserName = newSettings.UserName;

                    this.dataAccess.SaveSettings(this.settings.Connection);

                    return this.FormatResult(true);
                case Method.Get:
                    return this.FormatResult(this.settings.Connection);
                default:
                    throw new BadRequestException("Only post accepted");
            }
        }

        private string FormatResult<T>(T result)
        {
            this.Response.ContentType = "application/json";
            var data = JsonConvert.SerializeObject(result, Formatting.None);
            return data;
        }

        private T GetBody<T>()
        {
            using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }
                catch (JsonException ex)
                {
                    throw new BadRequestException(ex.Message);
                }
            }
        }

        public string Logging()
        {
            switch (this.Request.Method)
            {
                case Method.Put:
                    var newSettings = this.GetBody<LoggingSettings>();

                    this.settings.Logging.LogInterval = newSettings.LogInterval;

                    this.dataAccess.SaveSettings(this.settings.Logging);

                    return this.FormatResult(true);
                case Method.Get:
                    return this.FormatResult(this.settings.Logging);
                default:
                    throw new BadRequestException("Only post accepted");
            }
        }

        public string Mail()
        {
            switch (this.Request.Method)
            {
                case Method.Put:
                    var newSettings = this.GetBody<MailSettings>();

                    this.settings.Mail.From = newSettings.From;
                    this.settings.Mail.Password = newSettings.Password;
                    this.settings.Mail.Port = newSettings.Port;
                    this.settings.Mail.SendOnAlarm = newSettings.SendOnAlarm;
                    this.settings.Mail.SendOnConnectionLost = newSettings.SendOnConnectionLost;
                    this.settings.Mail.SendOnReminder = newSettings.SendOnReminder;
                    this.settings.Mail.SendShortMessage = newSettings.SendShortMessage;
                    this.settings.Mail.SendStatus = newSettings.SendStatus;
                    this.settings.Mail.SendStatusDuration = newSettings.SendStatusDuration;
                    this.settings.Mail.SendStatusMode = newSettings.SendStatusMode;
                    this.settings.Mail.Server = newSettings.Server;
                    this.settings.Mail.To = newSettings.To;
                    this.settings.Mail.UsePassword = newSettings.UsePassword;
                    this.settings.Mail.UserName = newSettings.UserName;
                    this.settings.Mail.EnableSsl = newSettings.EnableSsl;

                    this.dataAccess.SaveSettings(this.settings.Mail);

                    return this.FormatResult(true);
                case Method.Get:
                    return this.FormatResult(this.settings.Mail);
                default:
                    throw new BadRequestException("Only post accepted");
            }
        }
    }
}
