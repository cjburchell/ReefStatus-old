// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandController.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using HttpServer;
    using HttpServer.Exceptions;
    using HttpServer.MVC.Controllers;

    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;
    using RedPoint.ReefStatus.Common.Service;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    ///     Command Controller
    /// </summary>
    public class CommandController : RequestController
    {
        private readonly CommandThread commandThread;

        private readonly Controller controller;

        private readonly IReefStatusSettings settings;

        public CommandController(IReefStatusSettings settings, Controller controller, CommandThread commandThread)
        {
            this.settings = settings;
            this.controller = controller;
            this.commandThread = commandThread;
        }

        /// <summary>
        ///     Clears the level alarm.
        /// </summary>
        /// <returns>
        ///     OK if succeded
        /// </returns>
        public string ClearLevelAlarm()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var infoItem = this.controller.LevelSensors.FirstOrDefault(item => item.Id == this.Id);
                if (infoItem == null)
                {
                    throw new BadRequestException("Id not found");
                }

                this.commandThread.SendClearLevelAlarm(infoItem);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Make a clone of this controller
        /// </summary>
        /// <returns>
        ///     a new controller with the same base information as this one.
        /// </returns>
        public override object Clone()
        {
            return new CommandController(this.settings, this.controller, this.commandThread);
        }

        /// <summary>
        ///     Feeds the pasue.
        /// </summary>
        /// <returns>
        ///     Ok if succeded
        /// </returns>
        public string FeedPasue()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                var enable = this.GetBody<bool>();
                this.commandThread.SendFeed(enable);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
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

        /// <summary>
        ///     Keys this instance.
        /// </summary>
        /// <returns>
        ///     empty string
        /// </returns>
        public string Key()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing key");
            }

            try
            {
                switch (this.Id)
                {
                    case "up":
                        this.commandThread.KeyCommand(FaceplateKey.Up);
                        break;
                    case "down":
                        this.commandThread.KeyCommand(FaceplateKey.Down);
                        break;
                    case "right":
                        this.commandThread.KeyCommand(FaceplateKey.Right);
                        break;
                    case "left":
                        this.commandThread.KeyCommand(FaceplateKey.Left);
                        break;
                    case "esc":
                        this.commandThread.KeyCommand(FaceplateKey.Esc);
                        break;
                    case "enter":
                        this.commandThread.KeyCommand(FaceplateKey.Enter);
                        break;
                    default:
                        throw new BadRequestException($"Unknown key {this.Id}");
                }

                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        public string Test()
        {
            return "Test";
        }

        /// <summary>
        ///     Maintenances this instance.
        /// </summary>
        /// <returns>
        ///     Ok if succeded
        /// </returns>
        public string Maintenance()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                var enable = this.GetBody<bool>();
                this.commandThread.SendMaintenance(enable, this.controller.Info.Maintenance[0]);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Manuals the lights.
        /// </summary>
        /// <returns>
        ///     Ok if succreded
        /// </returns>
        public string ManualLights()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                    var enable = this.GetBody<bool>();

                    this.commandThread.SendOperationMode(enable ? OperationMode.ManualIllumination : OperationMode.Normal);
                    return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     enables/disables Manual sockets.
        /// </summary>
        /// <returns>
        ///     ok if succeded
        /// </returns>
        public string ManualSockets()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                var enable = this.GetBody<bool>();
                this.commandThread.SendOperationMode(enable ? OperationMode.ManualSockets : OperationMode.Normal);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Refreshes this instance.
        /// </summary>
        /// <returns>
        ///     if the commnd is ok
        /// </returns>
        public string Refresh()
        {
            this.commandThread.UpdateAll();
            return this.FormatResult(true);
        }

        /// <summary>
        ///     Resets the reminder.
        /// </summary>
        /// <returns>
        ///     Ok if the reminder was reset
        /// </returns>
        public string ResetReminder()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var reminder = this.controller.Info.Reminders.FirstOrDefault(item => item.Index.ToString() == this.Id);
                if (reminder == null)
                {
                    throw new BadRequestException("Id not found");
                }

                this.commandThread.SendResetReminder(reminder);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Sets the socket.
        /// </summary>
        /// <returns>
        ///     OK if succeded
        /// </returns>
        public string SetLight()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var infoItem = this.controller.Lights.FirstOrDefault(item => item.Id == this.Id);
                if (infoItem == null)
                {
                    throw new BadRequestException("Id not found");
                }

                var value = this.GetBody<int>();
                this.commandThread.SendLightState(infoItem, value);

                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Sets the socket.
        /// </summary>
        /// <returns>
        ///     OK if succeded
        /// </returns>
        public string SetSocket()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var infoItem = this.controller.SPorts.FirstOrDefault(item => item.Id == this.Id);
                if (infoItem == null)
                {
                    throw new BadRequestException("Id not found");
                }

                var enable = this.GetBody<bool>();
                this.commandThread.SendSocketState(infoItem, enable);

                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Starts the water change.
        /// </summary>
        /// <returns>
        ///     OK if succeded
        /// </returns>
        public string StartWaterChange()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var infoItem = this.controller.LevelSensors.FirstOrDefault(item => item.Id == this.Id);
                if (infoItem == null)
                {
                    throw new BadRequestException("Id not found");
                }

                this.commandThread.SendStartWaterChange(infoItem);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Thunderstorms this instance.
        /// </summary>
        /// <returns>
        ///     Ok if succeded
        /// </returns>
        public string StatusEmail()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                var message = this.GetBody<string>();

                AlertService.SendStatusEmail(message, this.controller, this.settings.Mail);

                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Thunderstorms this instance.
        /// </summary>
        /// <returns>
        ///     Ok if succeded
        /// </returns>
        public string Thunderstorm()
        {
            if (this.Request.Method != Method.Post)
            {
                throw new BadRequestException("Only post accepted");
            }

            try
            {
                var duration = this.GetBody<int>();

                this.commandThread.ThunderStorm(duration);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Updates the dousing value.
        /// </summary>
        /// <returns>
        ///     true or false
        /// </returns>
        public string UpdateDousingValue()
        {
            if (this.Request.Method != Method.Put)
            {
                throw new BadRequestException("Only put accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var dousingValue = this.GetBody<DousingValue>();
                var infoItem = this.controller.DosingPumps.FirstOrDefault(item => item.Id == this.Id);
                if (infoItem == null)
                {
                    throw new BadRequestException("Id not found");
                }

                this.commandThread.SendUpdateDosingRate(infoItem, dousingValue.Rate, dousingValue.PerDay);
                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Updates the user value.
        /// </summary>
        /// <returns>
        ///     True or false
        /// </returns>
        public string UpdateUserValue()
        {
            if (this.Request.Method != Method.Put)
            {
                throw new BadRequestException("Only put accepted");
            }

            if (string.IsNullOrEmpty(this.Id))
            {
                throw new BadRequestException("Missing Id");
            }

            try
            {
                var value = this.GetBody<double>();

                var userInfo = this.controller.UserInfo.FirstOrDefault(item => item.Id == this.Id);
                if (userInfo == null)
                {
                    throw new BadRequestException("Id not found");
                }
                userInfo.Time = DateTime.Now;
                userInfo.Value = value;

                using (var access = DatabaseConnectionFactory.Create())
                {
                    access.InsertItem((double)userInfo.Value, userInfo.Time, userInfo.Id, false, null);
                }

                return this.FormatResult(true);
            }
            catch (ReefStatusException ex)
            {
                Logger.Instance.LogError(ex);
                throw new InternalServerException(ex.Message);
            }
        }

        /// <summary>
        ///     Formats the result.
        /// </summary>
        /// <param name="result">
        ///     if set to <c>true</c> [result].
        /// </param>
        /// <returns>
        ///     the result in string format
        /// </returns>
        private string FormatResult(bool result)
        {
            this.Response.ContentType = "text/javascript";
            var data = JsonConvert.SerializeObject(result, Formatting.None);
            return data;
        }
    }
}