// <copyright file="CommandThread.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Commands
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;
    using RedPoint.ReefStatus.Common.ProfiLux.Protocol;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    ///     The command thread
    /// </summary>
    public class CommandThread : IDisposable, IProtocolCommands
    {
        /// <summary>
        ///     The max time for a connection
        /// </summary>
        private const int ConnectonTimeout = 1000;

        private readonly ConnectionSettings connectionSettings;

        /// <summary>
        ///     Gets or sets the Controller.
        /// </summary>
        /// <value>The Controller.</value>
        private readonly IController controller;

        /// <summary>
        ///     The protocol lock
        /// </summary>
        private readonly object protocolLock = new object();

        /// <summary>
        ///     The connection timer
        /// </summary>
        private Timer connectionTimer;

        /// <summary>
        ///     The in log.
        /// </summary>
        private bool inLog;

        /// <summary>
        ///     The is connection error.
        /// </summary>
        private bool isConnectionError;

        /// <summary>
        ///     The is sending light command.
        /// </summary>
        private bool isSendingLightCommand;

        /// <summary>
        ///     The is sending light time command.
        /// </summary>
        private bool isSendingLightTimeCommand;

        /// <summary>
        ///     The new light test time value.
        /// </summary>
        private int newLightTestTimeValue;

        /// <summary>
        ///     The protocol
        /// </summary>
        private IProfilux protocol;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandThread" /> class.
        /// </summary>
        /// <param name="controller">
        ///     The Controller.
        /// </param>
        /// <param name="connectionSettings">connection settings</param>
        public CommandThread(IController controller, ConnectionSettings connectionSettings)
        {
            this.controller = controller;
            this.connectionSettings = connectionSettings;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether in update all.
        /// </summary>
        public bool InUpdateAll { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.protocol?.IsConnected ?? false;

        /// <summary>
        ///     Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public IUpdateProgress Progress { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Stop();
            this.protocol?.Disconnect();
        }

        /// <summary>
        ///     Gets the display string.
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
        ///     Gets the view string.
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
        ///     Logs the parameters.
        /// </summary>
        public void LogParameters()
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
                    lock (this.protocolLock)
                    {
                        try
                        {
                            this.Connect();
                            this.UpdateValues(this.Progress);

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
                }

                if (!processedCommand)
                {
                    if (lastErrorMessage != null)
                    {
                        Logger.Instance.LogError(lastErrorMessage);
                        if (!this.isConnectionError)
                        {
                            ////this.reply.OnErrorInConnection(lastErrorMessage, true);
                            this.isConnectionError = true;
                        }
                    }
                }
                else
                {
                    if (this.isConnectionError)
                    {
                        ////this.reply.OnErrorInConnection(null, false);
                        this.isConnectionError = false;
                    }
                }
            }
            finally
            {
                this.inLog = false;
            }
        }

        /// <summary>
        ///     Updates all.
        /// </summary>
        public void UpdateAll()
        {
            if (this.InUpdateAll)
            {
                return;
            }

            this.InUpdateAll = true;
            try
            {
                lock (this.protocolLock)
                {
                    try
                    {
                        this.Connect();
                        this.UpdateAll(this.Progress, false);
                    }
                    catch (ReefStatusException ex)
                    {
                        this.HandelException(ex);
                    }
                }
            }
            finally
            {
                this.InUpdateAll = false;
            }
        }

        /// <summary>
        ///     Gets the info.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateInfo(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateInfo(this.protocol, null);
                }
                else
                {
                    this.UpdateInfoValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     Gets the sockets.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateLPorts(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateLPorts(this.protocol, null);
                }
                else
                {
                    this.UpdateLPortValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     Gets the sensors.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateLevelSensors(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateLevelSensors(this.protocol, null);
                }
                else
                {
                    this.UpdateLevelSensorsValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     Gets the sensors.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateProbes(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateProbes(this.protocol, null, false);
                }
                else
                {
                    this.UpdateProbeValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     Gets the sockets.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateSPorts(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateSPorts(this.protocol, null);
                }
                else
                {
                    this.UpdateSPortsValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     Clears the level alarm.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="levelSensor">
        ///     The level sensor.
        /// </param>
        public void ClearLevelAlarm(IProfilux basicProtocol, LevelSensor levelSensor)
        {
            basicProtocol.ClearLevelAlarm(levelSensor.Index);
            this.UpdateLevelSensorsValue(basicProtocol, levelSensor);
            this.UpdateAlarm(basicProtocol.Alarm, null);
        }

        /// <summary>
        ///     Resets the operational hours.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="baseInfo">
        ///     The base info.
        /// </param>
        public void ResetOperationalHours(IProfilux basicProtocol, BaseInfo baseInfo)
        {
            var light = baseInfo as Light;
            if (light != null)
            {
                basicProtocol.SetLightOperationHours(light.Channel, 0);
                this.UpdateLightOperationHours(basicProtocol, light);
            }
            else
            {
                var probe = baseInfo as Probe;
                if (probe == null)
                {
                    return;
                }

                basicProtocol.SetProbeOperationHours(probe.Index, 0);
                UpdateProbeOperationalHours(basicProtocol, probe);
            }
        }

        /// <summary>
        ///     Resets the reminder.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="reminder">
        ///     The reminder.
        /// </param>
        public void ResetReminder(IProfilux basicProtocol, Reminder reminder)
        {
            if (reminder.IsRepeating)
            {
                basicProtocol.ResetReminder(reminder.Index, reminder.Period);
            }
            else
            {
                basicProtocol.ClearReminder(reminder.Index);
            }

            this.UpdateReminders(basicProtocol, null);
        }

        /// <summary>
        ///     Sets the state of the light.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="light">
        ///     The light.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SetLightState(IProfilux basicProtocol, Light light, double value)
        {
            basicProtocol.SetLightValue(light.Channel, value);
        }

        public void SetLightTestTime(IProfilux basicProtocol, int value)
        {
            basicProtocol.SetLightTestValue(value);
        }

        /// <summary>
        ///     Sets the state of the socket.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        public void SetSocketState(IProfilux basicProtocol, SPort port, bool value)
        {
            basicProtocol.SetSocketState(port.PortNumber, value);
        }

        /// <summary>
        ///     Updates the alarm.
        /// </summary>
        /// <param name="alarmState">
        ///     State of the alarm.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateAlarm(CurrentState alarmState, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Alarm State";
            }

            if (this.controller.Info.Alarm != alarmState)
            {
                this.controller.Info.Alarm = alarmState;
                Logger.Instance.Log(new LogMessage(alarmState == CurrentState.On ? 1 : 0, "System Alarm is now " + alarmState));
            }
        }

        /// <summary>
        ///     Updates all.
        /// </summary>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        ///     if set to <c>true</c> [skip non essntals].
        /// </param>
        /// <returns>
        ///     True if all was updated
        /// </returns>
        public bool UpdateAll(IUpdateProgress progress, bool skipNonEssntals)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Update Started";
                double steps = protocol.ReminderCount + protocol.SensorCount + protocol.LevelSenosrCount + protocol.LPortCount + protocol.SPortCount + protocol.DigitalInputCount + protocol.TimerCount + protocol.LightCount + protocol.ProgrammableLogicCount;
                progress.SetProgressSteps(steps);
            }

            try
            {
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateInfo(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateProbes(protocol, progress, skipNonEssntals);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLevelSensors(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateDigitalInputs(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateDosingPumps(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLights(protocol, progress, skipNonEssntals);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateCurrentPumps(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateProgrammableLogic(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateSPorts(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLPorts(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateAssoications();

                this.controller.Info.LastUpdate = DateTime.Now;
            }
            finally
            {
                if (progress != null)
                {
                    progress.DisplayProgress = false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateCurrentPumpValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.Pumps.Count != 0)
            {
                foreach (var pump in this.controller.Pumps)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Current Pump " + pump.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    pump.OldValue = pump.Value;
                    pump.Value = basicProtocol.GetCurrentPumpValue(pump.Index);
                }
            }
            else
            {
                this.UpdateLights(basicProtocol, progress, false);
            }
        }

        /// <summary>
        ///     Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateCurrentPumps(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.CurrentPumpCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Current Pump" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    var index = i;
                    var pump = this.controller.Pumps.FirstOrDefault(p => p.Index == index);

                    if (basicProtocol.IsCurrentPumpAssinged(i))
                    {
                        if (pump == null)
                        {
                            pump = new CurrentPump { Id = basicProtocol.GetCurrentPumpId(index), Index = index };
                            this.controller.Pumps.Add(pump);
                        }

                        pump.OldValue = pump.Value;
                        pump.Value = basicProtocol.GetCurrentPumpValue(i);
                    }
                    else
                    {
                        if (pump != null)
                        {
                            this.controller.Pumps.Remove(pump);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2002, $"CurrentPump {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the digital input values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateDigitalInputValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.DigitalInputs.Count != 0)
            {
                foreach (var sensor in this.controller.DigitalInputs)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Digital Input " + sensor.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    sensor.OldValue = sensor.Value;
                    sensor.Value = basicProtocol.GetDigitalInputState(sensor.Index);
                }
            }
            else
            {
                this.UpdateDigitalInputs(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Updates the digital inputs.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateDigitalInputs(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.DigitalInputCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Digital Input" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                DigitalInputFunction mode;
                try
                {
                    mode = basicProtocol.GetDigitalInputFunction(i);
                }
                catch (ErrorCodeException)
                {
                    continue;
                }

                try
                {
                    var i1 = i;
                    var sensor = this.controller.DigitalInputs.FirstOrDefault(p => p.Index == i1);
                    if (mode != DigitalInputFunction.NotUsed || this.connectionSettings.GetAll)
                    {
                        if (sensor == null)
                        {
                            sensor = new DigitalInput { Id = basicProtocol.GetDigtialInputId(i), Index = i };
                            this.controller.DigitalInputs.Add(sensor);
                        }

                        sensor.Function = mode;

                        sensor.OldValue = sensor.Value;
                        sensor.Value = basicProtocol.GetDigitalInputState(i);
                    }
                    else
                    {
                        if (sensor != null)
                        {
                            this.controller.DigitalInputs.Remove(sensor);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2001, $"Digital Input {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateDosingPumps(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.TimerCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Timer" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    var channel = i;
                    var pump = this.controller.DosingPumps.FirstOrDefault(p => p.Channel == channel);
                    var settings = basicProtocol.GetTimerSettings(i);

                    if (settings.Mode == TimerMode.AutoDosing)
                    {
                        if (pump == null)
                        {
                            pump = new DosingPump { Id = basicProtocol.GetDousingPumpId(channel), Channel = channel };
                            this.controller.DosingPumps.Add(pump);
                        }

                        pump.Settings = settings;
                        pump.OldValue = pump.Value;
                        pump.Rate = basicProtocol.GetDosingRate(i);
                        pump.PerDay = settings.SwitchingCount;
                    }
                    else
                    {
                        if (pump != null)
                        {
                            this.controller.DosingPumps.Remove(pump);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2002, $"Timer {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the dosing pumps value.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="pump">
        ///     The pump.
        /// </param>
        public void UpdateDosingPumpsValue(IProfilux basicProtocol, DosingPump pump)
        {
            pump.OldValue = pump.Value;
            pump.Rate = basicProtocol.GetDosingRate(pump.Channel);

            var settings = basicProtocol.GetTimerSettings(pump.Channel);
            pump.Settings = settings;
            pump.PerDay = settings.SwitchingCount;
        }

        /// <summary>
        ///     Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateDosingPumpsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.DosingPumps.Count != 0)
            {
                foreach (var pump in this.controller.DosingPumps)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Dosing Pump " + pump.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    this.UpdateDosingPumpsValue(basicProtocol, pump);
                }
            }
            else
            {
                this.UpdateDosingPumps(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Updates the dosing rate.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="pump">
        ///     The pump.
        /// </param>
        /// <param name="rate">
        ///     The rate.
        /// </param>
        /// <param name="perDay">
        ///     The per day.
        /// </param>
        public void UpdateDosingRate(IProfilux basicProtocol, DosingPump pump, int rate, int perDay)
        {
            basicProtocol.UpdateDosingRate(pump.Channel, rate);
            var settings = pump.Settings;
            settings.SwitchingCount = perDay;
            basicProtocol.UpdateTimerSettings(pump.Channel, settings);
            this.UpdateDosingPumpsValue(basicProtocol, pump);
        }

        /// <summary>
        ///     Updates the info.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateInfo(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Getting Controller Information";
            }

            this.controller.Info.SoftwareVersion = basicProtocol.Version;
            this.controller.Info.ProductId = basicProtocol.ProductId;
            this.controller.Info.SerialNumber = basicProtocol.SerialNumber;
            this.controller.Info.SoftwareDate = basicProtocol.SoftwareDate;
            this.controller.Info.DeviceAddress = basicProtocol.DeviceAddress;

            this.controller.Info.Latitude = basicProtocol.Latitude;
            this.controller.Info.Longitude = basicProtocol.Longitude;
            this.controller.Info.MoonPhase = basicProtocol.MoonPhase;
            this.UpdateAlarm(basicProtocol.Alarm, progress);
            this.UpdateOperationMode(basicProtocol.OpMode, progress);
            this.UpdateMaintianceMode(basicProtocol, progress, 0);
            this.UpdateMaintianceMode(basicProtocol, progress, 1);
            this.UpdateMaintianceMode(basicProtocol, progress, 2);
            this.UpdateMaintianceMode(basicProtocol, progress, 3);

            for (var i = 0; i < basicProtocol.ReminderCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Reminder" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                var nextReminder = basicProtocol.GetNextReminder(i);

                var index = i;
                var reminder = this.controller.Info.Reminders.FirstOrDefault(r => r.Index == index);

                if (nextReminder.HasValue)
                {
                    if (reminder == null)
                    {
                        reminder = new Reminder { Index = i };
                        this.controller.Info.Reminders.Add(reminder);
                    }

                    // to reduce spam from the e-mail check to see if we have already sent the e-mail
                    if (reminder.Next != nextReminder)
                    {
                        reminder.SentMail = false;
                    }

                    reminder.Next = nextReminder.Value;
                    if (DateTime.Now > reminder.Next)
                    {
                        if (!reminder.IsOverdue)
                        {
                            reminder.IsOverdue = true;
                            Logger.Instance.Log(new LogMessage(2, "Reminder" + " " + reminder.Text + "IsOverdue"));
                        }
                    }
                    else
                    {
                        if (reminder.IsOverdue)
                        {
                            reminder.IsOverdue = false;
                            Logger.Instance.Log(new LogMessage(0, "Reminder" + " " + reminder.Text + "IsNotOverdue"));
                        }
                    }

                    reminder.Text = basicProtocol.GetReminderText(i);
                    reminder.Period = basicProtocol.GetReminderPeriod(i);
                    reminder.IsRepeating = basicProtocol.GetReminderRepeats(i);
                }
                else
                {
                    if (reminder != null)
                    {
                        this.controller.Info.Reminders.Remove(reminder);
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the alarm.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateInfoValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (Math.Abs(this.controller.Info.SoftwareVersion) < double.Epsilon)
            {
                this.UpdateInfo(basicProtocol, progress);
            }
            else
            {
                this.controller.Info.MoonPhase = basicProtocol.MoonPhase;
                this.UpdateAlarm(basicProtocol.Alarm, progress);
                this.UpdateOperationMode(basicProtocol.OpMode, progress);
                this.UpdateMaintianceMode(basicProtocol, progress, 0);
                this.UpdateMaintianceMode(basicProtocol, progress, 1);
                this.UpdateMaintianceMode(basicProtocol, progress, 2);
                this.UpdateMaintianceMode(basicProtocol, progress, 3);
                this.UpdateReminders(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Updates the L port values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateLPortValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.LPorts.Count != 0)
            {
                foreach (var port in this.controller.LPorts)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update 1-10V Port " + port.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    port.SetValue(basicProtocol.GetLPortValue(port.PortNumber));
                }
            }
            else
            {
                this.UpdateLPorts(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Gets the Devices.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateLPorts(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.LPortCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting L" + (i + 1) + " Port Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    var number = i;
                    var mode = basicProtocol.GetLPortFunction(i);
                    var port = this.controller.LPorts.FirstOrDefault(p => p.PortNumber == number);
                    if (mode.DeviceMode != DeviceMode.AlwaysOff || this.connectionSettings.GetAll)
                    {
                        if (port == null)
                        {
                            port = new LPort { Id = basicProtocol.GetLPortId(i), PortNumber = i };
                            this.controller.LPorts.Add(port);
                        }

                        port.Mode = mode;
                        PortMode.UpdateAssociatedModeItem(port.Mode, controller);

                        port.SetValue(basicProtocol.GetLPortValue(i));
                    }
                    else
                    {
                        if (port != null)
                        {
                            this.controller.LPorts.Remove(port);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2003, $"LPort {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Gets the sensors.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateLevelSensors(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.LevelSenosrCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Level Sensor" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                LevelSensorOpertationMode mode;
                try
                {
                    mode = basicProtocol.GetLevelSensorMode(i);
                }
                catch (ErrorCodeException)
                {
                    continue;
                }

                try
                {
                    var i1 = i;
                    var sensor = this.controller.LevelSensors.FirstOrDefault(p => p.Index == i1);

                    if (mode != LevelSensorOpertationMode.NotEnabled || this.connectionSettings.GetAll)
                    {
                        if (sensor == null)
                        {
                            sensor = new LevelSensor { Id = basicProtocol.GetLevelId(i), Index = i };
                            this.controller.LevelSensors.Add(sensor);
                        }

                        sensor.OpertationMode = mode;
                        var state = basicProtocol.GetLevelSensorState(i);
                        if (state.Alarm != sensor.AlarmState)
                        {
                            sensor.AlarmState = state.Alarm;
                            Logger.Instance.Log(new LogMessage(state.Alarm == CurrentState.On ? 1 : 0, "Level Sensor " + sensor.DisplayName + " Alarm is now " + state.Alarm));
                        }

                        sensor.OldValue = sensor.Value;
                        sensor.Value = state.State;

                        sensor.WaterMode = state.WaterMode;
                    }
                    else
                    {
                        if (sensor != null)
                        {
                            this.controller.LevelSensors.Remove(sensor);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2000, $"Level Sensor {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the level sensors value.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="sensor">
        ///     The sensor.
        /// </param>
        public void UpdateLevelSensorsValue(IProfilux basicProtocol, LevelSensor sensor)
        {
            var state = basicProtocol.GetLevelSensorState(sensor.Index);
            if (state.Alarm != sensor.AlarmState)
            {
                sensor.AlarmState = state.Alarm;
                Logger.Instance.Log(new LogMessage(state.Alarm == CurrentState.On ? 1 : 0, "Level Sensor " + sensor.DisplayName + " Alarm is now " + state.Alarm));
            }

            sensor.OldValue = sensor.Value;
            sensor.Value = state.State;

            sensor.WaterMode = state.WaterMode;
        }

        /// <summary>
        ///     Updates the level sensors values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateLevelSensorsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.LevelSensors.Count != 0)
            {
                foreach (var sensor in this.controller.LevelSensors)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Level Sensor " + sensor.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    this.UpdateLevelSensorsValue(basicProtocol, sensor);
                }
            }
            else
            {
                this.UpdateLevelSensors(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Updates the light value.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="light">
        ///     The light.
        /// </param>
        public void UpdateLightValue(IProfilux basicProtocol, Light light)
        {
            light.OldValue = light.Value;
            light.Value = basicProtocol.GetLightValue(light.Channel);
        }

        /// <summary>
        ///     Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        ///     if set to <c>true</c> [skip non essntals].
        /// </param>
        public void UpdateLights(IProfilux basicProtocol, IUpdateProgress progress, bool skipNonEssntals)
        {
            for (var i = 0; i < basicProtocol.LightCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Light" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    var channel = i;
                    var light = this.controller.Lights.FirstOrDefault(p => p.Channel == channel);

                    if (basicProtocol.IsLightActive(i))
                    {
                        if (light == null)
                        {
                            light = new Light { Id = basicProtocol.GetLightId(channel), Channel = channel };
                            this.controller.Lights.Add(light);
                        }

                        if (!skipNonEssntals)
                        {
                            light.IsDimmable = basicProtocol.IsLightDimmable(i);
                            this.UpdateLightOperationHours(basicProtocol, light);
                        }

                        light.OldValue = light.Value;
                        light.Value = basicProtocol.GetLightValue(i);

                        if (this.controller.Info.IsP3)
                        {
                            var lightName = basicProtocol.GetLightName(i);
                            if (!string.IsNullOrEmpty(lightName))
                            {
                                light.DisplayName = lightName;
                            }
                        }
                    }
                    else
                    {
                        if (light != null)
                        {
                            this.controller.Lights.Remove(light);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2002, $"Light {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateLightsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.Lights.Count != 0)
            {
                foreach (var light in this.controller.Lights)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Light " + light.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    this.UpdateLightValue(basicProtocol, light);
                    this.UpdateLightOperationHours(basicProtocol, light);
                }
            }
            else
            {
                this.UpdateLights(basicProtocol, progress, false);
            }
        }

        /// <summary>
        ///     The update maintiance mode.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        public void UpdateMaintianceMode(IProfilux basicProtocol, IUpdateProgress progress, int index)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Maintiance Mode " + index;
            }

            var maintenance = this.controller.Info.Maintenance.FirstOrDefault(item => item.Index == index);
            if (maintenance == null)
            {
                maintenance = new Maintenance(index);
                this.controller.Info.Maintenance.Add(maintenance);
            }

            maintenance.IsActive = basicProtocol.GetMaintenanceIsActive(index);
            maintenance.Duration = basicProtocol.GetMaintenanceDuration(index) * 60;
            maintenance.TimeLeft = basicProtocol.GetMaintenanceTimeLeft(index) * 60;
        }

        /// <summary>
        ///     Updates the operation mode.
        /// </summary>
        /// <param name="operationMode">
        ///     The operation mode.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateOperationMode(OperationMode operationMode, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Operation Mode";
            }

            if (this.controller.Info.OperationMode != operationMode)
            {
                this.controller.Info.OperationMode = operationMode;
                Logger.Instance.Log(new LogMessage(0, "Operation Mode Changed" + operationMode));
            }
        }

        /// <summary>
        ///     Updates the probe values.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateProbeValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.Probes.Count != 0)
            {
                foreach (var probe in this.controller.Probes)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Sensor " + probe.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    probe.SetValue(basicProtocol.GetSensorValue(probe.Index));
                    var alarm = basicProtocol.GetSensorAlarm(probe.Index);
                    if (alarm != probe.AlarmState)
                    {
                        probe.AlarmState = alarm;
                        Logger.Instance.Log(new LogMessage(alarm == CurrentState.On ? 1 : 0, "Sensor " + probe.DisplayName + " Alarm is now " + alarm));
                    }

                    UpdateProbeOperationalHours(basicProtocol, probe);
                }
            }
            else
            {
                this.UpdateProbes(basicProtocol, progress, false);
            }
        }

        /// <summary>
        ///     Updates the probes.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        ///     if set to <c>true</c> [skip non essntals].
        /// </param>
        public void UpdateProbes(IProfilux basicProtocol, IUpdateProgress progress, bool skipNonEssntals)
        {
            for (var i = 0; i < basicProtocol.SensorCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting Sensor" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                SensorType type;
                var mode = SensorMode.Normal;
                var active = false;
                try
                {
                    type = basicProtocol.GetSensorType(i);
                    if (type != SensorType.None && type != SensorType.Free)
                    {
                        mode = basicProtocol.GetSensorMode(i);
                        active = basicProtocol.GetSensorActive(i);
                    }
                }
                catch (ErrorCodeException)
                {
                    continue;
                }

                var i1 = i;
                var probe = this.controller.Probes.FirstOrDefault(p => p.Index == i1);

                var sensorId = basicProtocol.GetSensorId(i, type);

                if (type != SensorType.None && type != SensorType.Free && (active || this.connectionSettings.GetAll) && (mode == SensorMode.Normal || this.connectionSettings.GetAll))
                {
                    if (probe == null)
                    {
                        probe = new Probe { Format = basicProtocol.GetSensorFormat(i), Id = sensorId, Index = i, SensorType = type, SensorMode = mode };
                        this.controller.Probes.Add(probe);
                    }
                    else
                    {
                        probe.Format = basicProtocol.GetSensorFormat(i);
                        probe.SensorMode = mode;
                        probe.Id = sensorId;
                        probe.Index = i;
                        probe.SensorType = type;
                    }

                    if (!skipNonEssntals)
                    {
                        probe.NominalValue = probe.ConvertFromInt(basicProtocol.GetSensorNominalValue(i));
                        probe.AlarmDeviation = probe.ConvertFromInt(basicProtocol.GetSensorAlarmDeviation(i));
                        probe.AlarmEnable = basicProtocol.GetSensorAlarmEnable(i);
                        UpdateProbeOperationalHours(basicProtocol, probe);
                    }

                    if (this.controller.Info.IsP3)
                    {
                        var probeName = basicProtocol.GetProbeName(i);
                        if (!string.IsNullOrEmpty(probeName))
                        {
                            probe.DisplayName = probeName;
                        }
                    }

                    probe.SetValue(basicProtocol.GetSensorValue(i));
                    var alarm = basicProtocol.GetSensorAlarm(i);
                    if (alarm != probe.AlarmState)
                    {
                        probe.AlarmState = alarm;
                        Logger.Instance.Log(new LogMessage(alarm == CurrentState.On ? 1 : 0, "Sensor " + probe.DisplayName + " Alarm is now " + alarm));
                    }
                }
                else
                {
                    if (probe != null)
                    {
                        this.controller.Probes.Remove(probe);
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the reminders.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateReminders(IProfilux basicProtocol, IUpdateProgress progress)
        {
            foreach (var reminder in this.controller.Info.Reminders)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Update Reminder " + reminder.Text;
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                var nextReminder = basicProtocol.GetNextReminder(reminder.Index);
                if (nextReminder.HasValue)
                {
                    reminder.Next = nextReminder.Value;
                    if (DateTime.Now > reminder.Next)
                    {
                        if (!reminder.IsOverdue)
                        {
                            reminder.IsOverdue = true;
                            Logger.Instance.Log(new LogMessage(2, "Reminder" + " " + reminder.Text + " " + "Is Overdue"));
                        }
                    }
                    else
                    {
                        if (reminder.IsOverdue)
                        {
                            reminder.IsOverdue = false;
                            Logger.Instance.Log(new LogMessage(0, "Reminder" + " " + reminder.Text + " " + "Is Not Overdue"));
                        }
                    }
                }
            }
        }

        private void UpdateProgrammableLogic(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.ProgrammableLogicCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Programmable Logic" + (i + 1) + " Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                var index = i;
                var logic = this.controller.ProgrammableLogic.FirstOrDefault(r => r.Index == index);

                var input1 = basicProtocol.GetProgramLogicInput(0, i);
                var input2 = basicProtocol.GetProgramLogicInput(1, i);

                if (input1.DeviceMode != DeviceMode.AlwaysOff && input2.DeviceMode != DeviceMode.AlwaysOff)
                {
                    if (logic == null)
                    {
                        logic = new ProgramableLogic(i);
                        this.controller.ProgrammableLogic.Add(logic);
                    }

                    logic.Input1 = input1;
                    logic.Input2 = input2;
                    logic.Function = basicProtocol.GetProgramLogicFunction(i);
                }
                else
                {
                    if (logic != null)
                    {
                        this.controller.ProgrammableLogic.Remove(logic);
                    }
                }
            }

            foreach (var logic in this.controller.ProgrammableLogic)
            {
                PortMode.UpdateAssociatedModeItem(logic.Input1, controller);
                PortMode.UpdateAssociatedModeItem(logic.Input2, controller);
            }
        }

        /// <summary>
        ///     Updates the S port value.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        public void UpdateSPortValue(IProfilux basicProtocol, SPort port)
        {
            port.SetValue(basicProtocol.GetSPortValue(port.PortNumber));
        }

        /// <summary>
        ///     Gets the Devices.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateSPorts(IProfilux basicProtocol, IUpdateProgress progress)
        {
            for (var i = 0; i < basicProtocol.SPortCount; i++)
            {
                if (progress != null)
                {
                    progress.IncrementProgress();
                    progress.ProgressText = "Getting S" + (i + 1) + " Port Information";
                    if (progress.StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    var mode = basicProtocol.GetSPortFunction(i);
                    var portNumber = i;
                    var port = this.controller.SPorts.FirstOrDefault(p => p.PortNumber == portNumber);

                    if (mode.DeviceMode != DeviceMode.AlwaysOff || this.connectionSettings.GetAll)
                    {
                        if (port == null)
                        {
                            port = new SPort { Id = basicProtocol.GetSPortId(portNumber), PortNumber = portNumber };
                            this.controller.SPorts.Add(port);
                        }

                        port.Mode = mode;
                        PortMode.UpdateAssociatedModeItem(port.Mode, controller);

                        port.SetValue(basicProtocol.GetSPortValue(i));
                        port.OldCurrentValue = port.Current;
                        port.Current = basicProtocol.GetSPortCurrent(i);

                        if (this.controller.Info.IsP3)
                        {
                            var portName = basicProtocol.GetSPortName(i);
                            if (!string.IsNullOrEmpty(portName))
                            {
                                port.DisplayName = portName;
                            }
                        }
                    }
                    else
                    {
                        if (port != null)
                        {
                            this.controller.SPorts.Remove(port);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(new LogMessage(2002, $"SPort {i} not supported code {ex.ErrorCode}") { Exception = ex });
                }
            }
        }

        /// <summary>
        ///     Updates the S ports value.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        public void UpdateSPortsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.controller.SPorts.Count != 0)
            {
                foreach (var port in this.controller.SPorts)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Update Socket " + port.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    this.UpdateSPortValue(basicProtocol, port);
                    port.OldCurrentValue = port.Current;
                    port.Current = basicProtocol.GetSPortCurrent(port.PortNumber);
                }
            }
            else
            {
                this.UpdateSPorts(basicProtocol, progress);
            }
        }

        /// <summary>
        ///     Updates the values.
        /// </summary>
        /// <param name="progress">
        ///     The progress.
        /// </param>
        /// <returns>
        ///     retunes True if the values were updated
        /// </returns>
        public bool UpdateValues(IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Log Data Started";
                double steps = this.controller.Probes.Count + this.controller.LevelSensors.Count + this.controller.SPorts.Count + this.controller.LPorts.Count + this.controller.Lights.Count
                               + this.controller.DosingPumps.Count + this.controller.Info.Reminders.Count;
                progress.SetProgressSteps(steps);
            }

            try
            {
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateInfoValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateProbeValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLevelSensorsValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateSPortsValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLPortValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateDigitalInputValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateDosingPumpsValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateLightsValues(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateCurrentPumps(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateProgrammableLogic(protocol, progress);
                if (progress != null && progress.StopProcessing)
                {
                    return false;
                }

                this.UpdateAssoications();

                this.controller.Info.LastUpdate = DateTime.Now;
            }
            finally
            {
                if (progress != null)
                {
                    progress.DisplayProgress = false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Updates the probe operational hours.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="probe">
        ///     The probe.
        /// </param>
        private static void UpdateProbeOperationalHours(IProfilux basicProtocol, Probe probe)
        {
            var hours = basicProtocol.GetProbeOperationHours(probe.Index);

            if (probe.EnableMaxOperationHours && !probe.IsOverMaxOperationHours && probe.MaxOperationHours < hours / 60.0)
            {
                Logger.Instance.Log(new LogMessage(2, $"{probe.DisplayName} is now over its maximum operational hours"));
            }

            probe.OperationHours = hours;
        }

        /////////////////////////////////////////////////

        /// <summary>
        ///     Updates the light operation hours.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="light">
        ///     The light.
        /// </param>
        private void UpdateLightOperationHours(IProfilux basicProtocol, Light light)
        {
            var hours = basicProtocol.GetLightOperationHours(light.Channel);

            if (light.EnableMaxOperationHours && !light.IsOverMaxOperationHours && light.MaxOperationHours < hours / 60.0)
            {
                Logger.Instance.Log(new LogMessage(2, $"{light.DisplayName} is now over its maximum operational hours"));
            }

            light.OperationHours = hours;
        }

        /// <summary>
        ///     Waters the change.
        /// </summary>
        /// <param name="basicProtocol">
        ///     The basic protocol.
        /// </param>
        /// <param name="levelSensor">
        ///     The level sensor.
        /// </param>
        public void WaterChange(IProfilux basicProtocol, LevelSensor levelSensor)
        {
            basicProtocol.WaterChange(levelSensor.Index);
        }

        /// <summary>
        ///     Gets the data points.
        /// </summary>
        /// <param name="callback">
        ///     The callback.
        /// </param>
        /// <returns>
        ///     List of Data Points
        /// </returns>
        public Collection<ItemDataRow> GetDataPoints(IProgressCallback callback)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                return this.protocol.GetDataPoints(callback, this.controller.Probes);
            }
        }

        /// <summary>
        ///     Gets the display string.
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
        ///     Gets the view text.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
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
        ///     Sends the clear level alarm.
        /// </summary>
        /// <param name="levelSensor">
        ///     The level sensor.
        /// </param>
        public void SendClearLevelAlarm(LevelSensor levelSensor)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.ClearLevelAlarm(this.protocol, levelSensor);
                    Logger.Instance.Log(new LogMessage(0, "Level Sensor Alarm " + levelSensor.DisplayName + " Cleared"));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the feed.
        /// </summary>
        /// <param name="enable">
        ///     The enable.
        /// </param>
        public void SendFeed(bool enable)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.protocol.FeedPause(enable);
                    Logger.Instance.Log(new LogMessage(0, "Feed Pause Set"));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the key command.
        /// </summary>
        /// <param name="key">
        ///     The key.
        /// </param>
        public void KeyCommand(FaceplateKey key)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.protocol.KeyCommand(key);
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the state of the light.
        /// </summary>
        /// <param name="light">
        ///     The light.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="force">
        ///     if set to <c>true</c> [force].
        /// </param>
        public void SendLightState(Light light, double value, bool force = false)
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
                        this.SetLightState(this.protocol, light, value);
                        this.UpdateLightValue(this.protocol, light);
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
        }

        /// <summary>
        ///     Sends the state of the light.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SendLightTestTime(int value)
        {
            this.newLightTestTimeValue = value;
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
                        this.SetLightTestTime(this.protocol, this.newLightTestTimeValue);
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
        }

        /// <summary>
        ///     The send light text.
        /// </summary>
        /// <param name="light">
        ///     The light.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        public void SendLightText(Light light, string text)
        {
            if (this.controller.Info.IsP3)
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
            }
        }

        /// <summary>
        ///     Sends the maintenance.
        /// </summary>
        /// <param name="enable">
        ///     The enable.
        /// </param>
        /// <param name="maintenance">
        ///     The maintenance.
        /// </param>
        public void SendMaintenance(bool enable, Maintenance maintenance)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.protocol.Maintenace(enable, maintenance.Index);
                    Logger.Instance.Log(new LogMessage(0, "Maintenance Mode" + " " + maintenance.Index + " " + (enable ? "Enabled" : "Disabled")));
                    this.UpdateOperationMode(this.protocol.OpMode, null);
                    this.UpdateMaintianceMode(this.protocol, null, maintenance.Index);
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the operation mode.
        /// </summary>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        public void SendOperationMode(OperationMode mode)
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
                        Logger.Instance.Log(new LogMessage(0, "Set Operation Mode " + mode));
                        this.UpdateOperationMode(this.protocol.OpMode, null);
                        if (lastMode == OperationMode.ManualSockets)
                        {
                            this.UpdateSPortsValues(this.protocol, null);
                        }
                        else if (lastMode == OperationMode.ManualIllumination)
                        {
                            this.UpdateLPortValues(this.protocol, null);
                            this.UpdateLightsValues(this.protocol, null);
                        }
                    }
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     The send probe text.
        /// </summary>
        /// <param name="probe">
        ///     The probe.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void SendProbeText(Probe probe, string value)
        {
            if (this.controller.Info.IsP3)
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
            }
        }

        /// <summary>
        ///     The send reset operational hours.
        /// </summary>
        /// <param name="baseInfo">
        ///     The base info.
        /// </param>
        public void SendResetOperationalHours(BaseInfo baseInfo)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.ResetOperationalHours(this.protocol, baseInfo);
                    Logger.Instance.Log(new LogMessage(0, $"Operational Hours for {baseInfo.DisplayName} Reset"));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Resets the reminder.
        /// </summary>
        /// <param name="reminder">
        ///     The reminder.
        /// </param>
        public void SendResetReminder(Reminder reminder)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.ResetReminder(this.protocol, reminder);
                    Logger.Instance.Log(new LogMessage(0, "Reminder" + " " + reminder.Text + " " + "Reset"));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     The send s port text.
        /// </summary>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="text">
        ///     The text.
        /// </param>
        public void SendSPortText(SPort port, string text)
        {
            if (this.controller.Info.IsP3)
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
            }
        }

        /// <summary>
        ///     Sends the state of the socket.
        /// </summary>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        public void SendSocketState(SPort port, bool value)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.SetSocketState(this.protocol, port, value);
                    this.UpdateSPortValue(this.protocol, port);
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the start water change.
        /// </summary>
        /// <param name="levelSensor">
        ///     The level sensor.
        /// </param>
        public void SendStartWaterChange(LevelSensor levelSensor)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.WaterChange(this.protocol, levelSensor);
                    Logger.Instance.Log(new LogMessage(0, "Water Change for" + levelSensor.DisplayName + "Started"));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Sends the update dosing rate.
        /// </summary>
        /// <param name="pump">
        ///     The pump.
        /// </param>
        /// <param name="rate">
        ///     The rate.
        /// </param>
        /// <param name="perDay">
        ///     the rate per day
        /// </param>
        public void SendUpdateDosingRate(DosingPump pump, int rate, int perDay)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.UpdateDosingRate(this.protocol, pump, rate, perDay);
                    Logger.Instance.Log(new LogMessage(0, "Updated" + " " + pump.DisplayName + " " + "Dosing Rate To" + (rate * perDay) + pump.Units));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Stops this instance.
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
        ///     Starts a Thunder Storm.
        /// </summary>
        /// <param name="duration">
        ///     The duration.
        /// </param>
        public void ThunderStorm(int duration)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.protocol.Thunderstorm(duration);
                    Logger.Instance.Log(new LogMessage(0, "Thunderstorm Started " + duration));
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        /// <summary>
        ///     Updates the digital input.
        /// </summary>
        /// <param name="fullUpdate">
        ///     if set to <c>true</c> [full update].
        /// </param>
        public void UpdateDigitalInput(bool fullUpdate)
        {
            lock (this.protocolLock)
            {
                this.Connect();
                if (fullUpdate)
                {
                    this.UpdateDigitalInputs(this.protocol, null);
                }
                else
                {
                    this.UpdateDigitalInputValues(this.protocol, null);
                }
            }
        }

        /// <summary>
        ///     The update maintenance.
        /// </summary>
        /// <param name="maintenance">
        ///     The maintenance.
        /// </param>
        public void UpdateMaintenance(Maintenance maintenance)
        {
            lock (this.protocolLock)
            {
                try
                {
                    this.Connect();
                    this.UpdateOperationMode(this.protocol.OpMode, null);
                    this.UpdateMaintianceMode(this.protocol, null, maintenance.Index);
                }
                catch (ReefStatusException ex)
                {
                    this.HandelException(ex);
                }
            }
        }

        #region Methods

        /// <summary>
        ///     Connects this instance.
        /// </summary>
        private void Connect()
        {
            lock (this.protocolLock)
            {
                if (this.protocol == null || !this.protocol.IsConnected)
                {
                    this.protocol = ProfiluxController.Connect(this.connectionSettings);
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
        ///     Handles the Tick event of the connectionTimer control.
        /// </summary>
        /// <param name="state">
        ///     The state.
        /// </param>
        private void ConnectionTimerTick(object state)
        {
            this.Disconnect();
        }

        /// <summary>
        ///     Disconnects this instance.
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

        private void UpdateAssoications()
        {
            foreach (var logic in controller.ProgrammableLogic)
            {
                PortMode.UpdateAssociatedModeItem(logic.Input1, controller);
                PortMode.UpdateAssociatedModeItem(logic.Input2, controller);
            }

            foreach (var sport in controller.SPorts)
            {
                PortMode.UpdateAssociatedModeItem(sport.Mode, controller);
            }

            foreach (var lport in controller.LPorts)
            {
                PortMode.UpdateAssociatedModeItem(lport.Mode, controller);
            }
        }

        /// <summary>
        ///     Handels the exception.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
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