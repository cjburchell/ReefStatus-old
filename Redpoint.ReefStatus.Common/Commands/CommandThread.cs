// <copyright file="CommandThread.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Commands
{
    using System;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI;

    /// <summary>
    /// The command thread
    /// </summary>
    public class CommandThread : IDisposable, IProtocolCommands
    {
        #region Constants

        /// <summary>
        /// The max time for a connection
        /// </summary>
        private const int ConnectonTimeout = 1000;

        #endregion

        #region Fields

        /// <summary>
        /// The protocol lock
        /// </summary>
        private readonly object protocolLock = new object();

        /// <summary>
        /// The reply
        /// </summary>
        private readonly ICommandReply reply;

        /// <summary>
        /// The connection timer
        /// </summary>
        private Timer connectionTimer;

        /// <summary>
        /// The in log.
        /// </summary>
        private bool inLog;

        /// <summary>
        /// The in update display text.
        /// </summary>
        private bool inUpdateDisplayText;

        /// <summary>
        /// The is connection error.
        /// </summary>
        private bool isConnectionError;

        /// <summary>
        /// The is sending light command.
        /// </summary>
        private bool isSendingLightCommand;

        /// <summary>
        /// The is sending light time command.
        /// </summary>
        private bool isSendingLightTimeCommand;

        /// <summary>
        /// The new light test time value.
        /// </summary>
        private int newLightTestTimeValue;

        /// <summary>
        /// The protocol
        /// </summary>
        private IProfilux protocol;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandThread"/> class.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public CommandThread(ICommandReply reply, Controller Controller)
        {
            this.reply = reply;
            this.Controller = Controller;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Controller.
        /// </summary>
        /// <value>The Controller.</value>
        public Controller Controller { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether in update all.
        /// </summary>
        public bool InUpdateAll { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.protocol != null && this.protocol.IsConnected;
            }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public IUpdateProgress Progress { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Stop();
            if (this.protocol != null)
            {
                this.protocol.Disconnect();
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// List of Data Points
        /// </returns>
        public Collection<ItemDataRow> GetDataPoints(IProgressCallback callback)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.GetDataPoints(callback, this.Controller.Items.OfType<Probe>());
            }
        }

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <returns>The display String</returns>
        public string[] GetDisplay()
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.Display;
            }
        }

        /// <summary>
        /// Gets the display string.
        /// </summary>
        /// <returns>The display String</returns>
        public string GetDisplayString()
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.DisplayText;
            }
        }

        /// <summary>
        /// Gets the view string.
        /// </summary>
        /// <returns>The View string</returns>
        public string GetViewString()
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.ViewText;
            }
        }

        /// <summary>
        /// Gets the view text.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public string[] GetViewText()
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.View;
            }
        }

        /// <summary>
        /// Keys the command.
        /// </summary>
        /// <param name="facePlateKey">
        /// The face plate key.
        /// </param>
        public void KeyCommand(FaceplateKey facePlateKey)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                this.protocol.KeyCommand(facePlateKey);
            }
        }

        /// <summary>
        /// Logs the parameters.
        /// </summary>
        public void LogParameters()
        {
            new Thread(
                () =>
                    {
                        if (this.inLog)
                        {
                            return;
                        }

                        this.inLog = true;
                        try
                        {
                            var processedCommand = false;
                            ReefStatusException lastErrorMessage = null;
                            var retryCount = 0;

                            // try the command 5 times until it fails
                            while (!processedCommand && retryCount < 5)
                            {
                                retryCount++;
                                var updated = false;
                                lock (this.protocolLock)
                                {
                                    try
                                    {
                                        this.Connect();
                                        updated = this.Controller.UpdateValues(this.protocol, this.Progress);

                                        processedCommand = true;
                                    }
                                    catch (ReefStatusException ex)
                                    {
                                        lastErrorMessage = ex;
                                        try
                                        {
                                            if (this.protocol != null && this.protocol.IsConnected)
                                            {
                                                this.Disconnect();
                                                this.Connect();
                                            }
                                        }
                                        catch (ReefStatusException excpt)
                                        {
                                            lastErrorMessage = excpt;
                                            this.Disconnect();
                                        }

                                        Thread.Sleep(1000);
                                    }
                                }

                                if (updated)
                                {
                                    try
                                    {
                                        this.reply.UpdateStatus(this.Controller);
                                        this.reply.LogData(DateTime.Now, this.Controller, this.Progress);
                                    }
                                    catch (ReefStatusException ex)
                                    {
                                        this.HandelException(ex);
                                    }
                                }
                            }

                            if (!processedCommand)
                            {
                                if (lastErrorMessage != null)
                                {
                                    Logger.Instance.LogError(lastErrorMessage);
                                    if (!this.isConnectionError)
                                    {
                                        this.reply.OnErrorInConnection(lastErrorMessage, true);
                                        this.isConnectionError = true;
                                    }
                                }
                            }
                            else
                            {
                                if (this.isConnectionError)
                                {
                                    this.reply.OnErrorInConnection(null, false);
                                    this.isConnectionError = false;
                                }
                            }
                        }
                        finally
                        {
                            this.inLog = false;
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the clear level alarm.
        /// </summary>
        /// <param name="levelSensor">
        /// The level sensor.
        /// </param>
        public void SendClearLevelAlarm(LevelSensor levelSensor)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.ClearLevelAlarm(this.protocol, levelSensor);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        Language.GetResource("strLevelSensorAlarm") + levelSensor.DisplayName
                                        + Language.GetResource("strCleared")));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the feed.
        /// </summary>
        /// <param name="enable">
        /// The enable.
        /// </param>
        public void SendFeed(bool enable)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.protocol.FeedPause(enable);
                                Logger.Instance.Log(new LogMessage(0, Language.GetResource("strFeedPauseSet")));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the key command.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public void SendKeyCommand(FaceplateKey key)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.KeyCommand(key);
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the state of the light.
        /// </summary>
        /// <param name="light">
        /// The light.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="force">
        /// if set to <c>true</c> [force].
        /// </param>
        public void SendLightState(Light light, double value, bool force = false)
        {
            new Thread(
                () =>
                    {
                        if (this.isSendingLightCommand && !force)
                        {
                            return;
                        }

                        this.isSendingLightCommand = true;
                        try
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    this.Controller.SetLightState(this.protocol, light, value);
                                    this.Controller.UpdateLightValue(this.protocol, light);
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }
                        finally
                        {
                            this.isSendingLightCommand = false;
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the state of the light.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SendLightTestTime(int value)
        {
            this.newLightTestTimeValue = value;
            new Thread(
                () =>
                    {
                        if (this.isSendingLightTimeCommand)
                        {
                            return;
                        }

                        this.isSendingLightTimeCommand = true;
                        try
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    this.Controller.SetLightTestTime(this.protocol, this.newLightTestTimeValue);
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }
                        finally
                        {
                            this.isSendingLightTimeCommand = false;
                        }
                    }).Start();
        }

        /// <summary>
        /// The send light text.
        /// </summary>
        /// <param name="light">
        /// The light.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void SendLightText(Light light, string text)
        {
            if (this.Controller.Info.IsP3)
            {
                new Thread(
                    () =>
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();

                                    this.protocol.SetLightName(light.Channel, text);
                                    var lightName = this.protocol.GetLightName(light.Channel);
                                    if (!string.IsNullOrEmpty(lightName))
                                    {
                                        light.DisplayName = lightName;
                                    }
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }).Start();
            }
        }

        /// <summary>
        /// Sends the maintenance.
        /// </summary>
        /// <param name="enable">
        /// The enable.
        /// </param>
        /// <param name="maintenance">
        /// The maintenance.
        /// </param>
        public void SendMaintenance(bool enable, Maintenance maintenance)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.protocol.Maintenace(enable, maintenance.Index);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        Language.GetResource("strMaintenanceMode") + " " + maintenance.Index + " "
                                        + (enable
                                               ? Language.GetResource("strEnabled")
                                               : Language.GetResource("strDisabled"))));
                                this.Controller.UpdateOperationMode(this.protocol.OpMode, null);
                                this.Controller.UpdateMaintianceMode(this.protocol, null, maintenance.Index);
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the operation mode.
        /// </summary>
        /// <param name="mode">
        /// The mode.
        /// </param>
        public void SendOperationMode(OperationMode mode)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                var lastMode = this.protocol.OpMode;
                                if (lastMode != mode)
                                {
                                    this.protocol.OpMode = mode;
                                    Logger.Instance.Log(
                                        new LogMessage(
                                            0, 
                                            Language.GetResource("strSetOperationMode") + Language.GetFrendyName(mode)));
                                    this.Controller.UpdateOperationMode(this.protocol.OpMode, null);
                                    if (lastMode == OperationMode.ManualSockets)
                                    {
                                        this.Controller.UpdateSPortsValues(this.protocol, null);
                                    }
                                    else if (lastMode == OperationMode.ManualIllumination)
                                    {
                                        this.Controller.UpdateLPortValues(this.protocol, null);
                                        this.Controller.UpdateLightsValues(this.protocol, null);
                                    }
                                }
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// The send probe text.
        /// </summary>
        /// <param name="probe">
        /// The probe.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void SendProbeText(Probe probe, string value)
        {
            if (this.Controller.Info.IsP3)
            {
                new Thread(
                    () =>
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    this.protocol.SetProbeName(probe.Index, value);
                                    var probeName = this.protocol.GetProbeName(probe.Index);
                                    if (!string.IsNullOrEmpty(probeName))
                                    {
                                        probe.DisplayName = probeName;
                                    }
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }).Start();
            }
        }

        /// <summary>
        /// The send reset operational hours.
        /// </summary>
        /// <param name="baseInfo">
        /// The base info.
        /// </param>
        public void SendResetOperationalHours(BaseInfo baseInfo)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.ResetOperationalHours(this.protocol, baseInfo);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        string.Format(
                                            Language.GetResource("strResetOperationalHoursEvent"), 
                                            baseInfo.DisplayName)));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Resets the reminder.
        /// </summary>
        /// <param name="reminder">
        /// The reminder.
        /// </param>
        public void SendResetReminder(Reminder reminder)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.ResetReminder(this.protocol, reminder);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        Language.GetResource("strReminder") + " " + reminder.Text + " "
                                        + Language.GetResource("strReset")));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// The send s port text.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void SendSPortText(SPort port, string text)
        {
            if (this.Controller.Info.IsP3)
            {
                new Thread(
                    () =>
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    this.protocol.SetSPortName(port.PortNumber, text);
                                    var lightName = this.protocol.GetSPortName(port.PortNumber);
                                    if (!string.IsNullOrEmpty(lightName))
                                    {
                                        port.DisplayName = lightName;
                                    }
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }).Start();
            }
        }

        /// <summary>
        /// Sends the state of the socket.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="value">
        /// if set to <c>true</c> [value].
        /// </param>
        public void SendSocketState(SPort port, bool value)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.SetSocketState(this.protocol, port, value);
                                this.Controller.UpdateSPortValue(this.protocol, port);
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the start water change.
        /// </summary>
        /// <param name="levelSensor">
        /// The level sensor.
        /// </param>
        public void SendStartWaterChange(LevelSensor levelSensor)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.WaterChange(this.protocol, levelSensor);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        Language.GetResource("strWaterChangefor") + levelSensor.DisplayName
                                        + Language.GetResource("strStarted")));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Sends the update dosing rate.
        /// </summary>
        /// <param name="pump">
        /// The pump.
        /// </param>
        /// <param name="rate">
        /// The rate.
        /// </param>
        /// <param name="perDay">
        /// the rate per day
        /// </param>
        public void SendUpdateDosingRate(DosingPump pump, int rate, int perDay)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.UpdateDosingRate(this.protocol, pump, rate, perDay);
                                Logger.Instance.Log(
                                    new LogMessage(
                                        0, 
                                        Language.GetResource("strUpdated") + " " + pump.DisplayName + " "
                                        + Language.GetResource("strDosingRateTo") + (rate * perDay) + pump.Units));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (this.Progress != null)
            {
                this.Progress.StopProcessing = true;
            }

            this.Disconnect();
        }

        /// <summary>
        /// Starts a Thunder Storm.
        /// </summary>
        /// <param name="duration">
        /// The duration.
        /// </param>
        public void ThunderStorm(int duration)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.protocol.Thunderstorm(duration);
                                Logger.Instance.Log(
                                    new LogMessage(0, Language.GetResource("strThunderstormStarted") + duration));
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        public void UpdateAll()
        {
            new Thread(
                () =>
                    {
                        if (this.InUpdateAll)
                        {
                            return;
                        }

                        this.InUpdateAll = true;
                        try
                        {
                            var update = false;
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    update = this.Controller.UpdateAll(this.protocol, this.Progress, false);
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }

                            try
                            {
                                if (update)
                                {
                                    this.reply.UpdateStatus(this.Controller);
                                    this.reply.LogData(DateTime.Now, this.Controller, this.Progress);
                                }
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                        finally
                        {
                            this.InUpdateAll = false;
                        }
                    }).Start();
        }

        /// <summary>
        /// Updates the digital input.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateDigitalInput(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateDigitalInputs(this.protocol, null);
                }
                else
                {
                    this.Controller.UpdateDigitalInputValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <param name="getViewText">
        /// The get View Text.
        /// </param>
        public void UpdateDisplayText(bool getViewText = true)
        {
            new Thread(
                () =>
                    {
                        if (this.inUpdateDisplayText)
                        {
                            return;
                        }

                        this.inUpdateDisplayText = true;
                        try
                        {
                            lock (this.protocolLock)
                            {
                                try
                                {
                                    this.Connect();
                                    if (this.Controller.Info.IsP3)
                                    {
                                        this.reply.UpdateDisplayText(
                                            string.Empty, 
                                            getViewText ? this.GetViewString() : string.Empty, 
                                            this.GetDisplayImage(3), 
                                            this.Controller);
                                    }
                                    else
                                    {
                                        this.reply.UpdateDisplayText(
                                            this.GetDisplayString(), 
                                            getViewText ? this.GetViewString() : string.Empty, 
                                            null, 
                                            this.Controller);
                                    }
                                }
                                catch (ReefStatusException ex)
                                {
                                    this.HandelException(ex);
                                }
                            }
                        }
                        finally
                        {
                            this.inUpdateDisplayText = false;
                        }
                    }).Start();
        }

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateInfo(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateInfo(this.protocol, null);
                }
                else
                {
                    this.Controller.UpdateInfoValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        /// Gets the sockets.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateLPorts(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateLPorts(this.protocol, null);
                }
                else
                {
                    this.Controller.UpdateLPortValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateLevelSensors(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateLevelSensors(this.protocol, null);
                }
                else
                {
                    this.Controller.UpdateLevelSensorsValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        /// The update maintenance.
        /// </summary>
        /// <param name="maintenance">
        /// The maintenance.
        /// </param>
        public void UpdateMaintenance(Maintenance maintenance)
        {
            new Thread(
                () =>
                    {
                        lock (this.protocolLock)
                        {
                            try
                            {
                                this.Connect();
                                this.Controller.UpdateOperationMode(this.protocol.OpMode, null);
                                this.Controller.UpdateMaintianceMode(this.protocol, null, maintenance.Index);
                            }
                            catch (ReefStatusException ex)
                            {
                                this.HandelException(ex);
                            }
                        }
                    }).Start();
        }

        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateProbes(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateProbes(this.protocol, null, false);
                }
                else
                {
                    this.Controller.UpdateProbeValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        /// Gets the sockets.
        /// </summary>
        /// <param name="fullUpdate">
        /// if set to <c>true</c> [full update].
        /// </param>
        public void UpdateSPorts(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.Controller.UpdateSPorts(this.protocol, null);
                }
                else
                {
                    this.Controller.UpdateSPortsValues(this.protocol, null);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Connects this instance.
        /// </summary>
        private void Connect()
        {
            lock (this.protocolLock)
            {
                if (this.protocol == null || !this.protocol.IsConnected)
                {
                    this.protocol = ProfiluxController.Connect(this.Controller.Connection);
                }

                if (this.connectionTimer != null)
                {
                    this.connectionTimer.Change(ConnectonTimeout, Timeout.Infinite);
                }
                else
                {
                    this.connectionTimer = new Timer(this.ConnectionTimerTick, null, ConnectonTimeout, Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the connectionTimer control.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void ConnectionTimerTick(object state)
        {
            this.Disconnect();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        private void Disconnect()
        {
            lock (this.protocolLock)
            {
                if (this.protocol != null)
                {
                    try
                    {
                        this.protocol.Disconnect();
                    }
                    catch (ReefStatusException ex)
                    {
                        Logger.Instance.LogError(ex);
                    }

                    this.protocol = null;
                }

                if (this.connectionTimer != null)
                {
                    this.connectionTimer.Dispose();
                    this.connectionTimer = null;
                }
            }
        }

        /// <summary>
        /// Gets the display image.
        /// </summary>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <returns>
        /// The <see cref="Image"/>.
        /// </returns>
        private Image GetDisplayImage(int size)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.GetDisplayImage(size);
            }
        }

        /// <summary>
        /// Handels the exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void HandelException(ReefStatusException ex)
        {
            try
            {
                if (this.protocol != null && this.protocol.IsConnected)
                {
                    this.Disconnect();
                    this.Connect();
                }
            }
            catch (ReefStatusException)
            {
                this.Disconnect();
            }

            Logger.Instance.LogError(ex);
        }

        #endregion
    }
}