

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Communication;
    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// Controller Class
    /// </summary>
    public class Controller : BindableBase
    {
        #region Fields

        /// <summary>
        /// Id of the controller
        /// </summary>
        private int id;

        /// <summary>
        /// Name of the controller
        /// </summary>
        private string name;

        /// <summary>
        /// The commands
        /// </summary>
        private CommandThread commands;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        public Controller()
        {
            this.Connection = new ConnectionSettings();
            this.Info = new Info();
            this.Items = new SafeObservableCollection<BaseInfo>();
            this.ProgrammableLogic = new SafeObservableCollection<ProgramableLogic>();
        }

        #endregion

        #region Public Properties

        [XmlIgnore]
        public CommandThread Commands
        {
            get
            {
                return this.commands ?? (this.commands = new CommandThread(DataService.Instance, this));
            }
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public ConnectionSettings Connection { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                if (string.IsNullOrEmpty(this.Name))
                {
                    this.Name = "Controller" + this.id;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Controller info.
        /// </summary>
        /// <value>The info.</value>
        public Info Info { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public SafeObservableCollection<BaseInfo> Items { get; set; }

        /// <summary>
        /// Gets or sets the programmable logic.
        /// </summary>
        /// <value>
        /// The programmable logic.
        /// </value>
        public SafeObservableCollection<ProgramableLogic> ProgrammableLogic { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(() => this.Name);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sent alarm email].
        /// </summary>
        /// <value><c>true</c> if [sent alarm email]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool SentAlarmEmail { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="levelSensor">
        /// The level sensor.
        /// </param>
        public void ClearLevelAlarm(IProfilux basicProtocol, LevelSensor levelSensor)
        {
            basicProtocol.ClearLevelAlarm(levelSensor.Index);
            this.UpdateLevelSensorsValue(basicProtocol, levelSensor);
            this.UpdateAlarm(basicProtocol.Alarm, null);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemId">
        /// The id.
        /// </param>
        /// <returns>
        /// and item with the given id
        /// </returns>
        public BaseInfo GetItem(string itemId)
        {
            return this.Items.FirstOrDefault(p => p.Id == itemId);
        }

        /// <summary>
        /// Resets the operational hours.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="baseInfo">
        /// The base info.
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
        /// Resets the reminder.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="reminder">
        /// The reminder.
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
        /// Sets the state of the light.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="light">
        /// The light.
        /// </param>
        /// <param name="value">
        /// The value.
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
        /// Sets the state of the socket.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="value">
        /// if set to <c>true</c> [value].
        /// </param>
        public void SetSocketState(IProfilux basicProtocol, SPort port, bool value)
        {
            basicProtocol.SetSocketState(port.PortNumber, value);
        }

        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="alarmState">
        /// State of the alarm.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateAlarm(CurrentState alarmState, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Alarm State";
            }

            if (this.Info.Alarm != alarmState)
            {
                this.Info.Alarm = alarmState;
                Logger.Instance.Log(
                    new LogMessage(alarmState == CurrentState.On ? 1 : 0, "System Alarm is now " + alarmState));
            }
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        /// if set to <c>true</c> [skip non essntals].
        /// </param>
        /// <returns>
        /// True if all was updated
        /// </returns>
        public bool UpdateAll(IProfilux protocol, IUpdateProgress progress, bool skipNonEssntals)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Update Started";
                double steps = protocol.ReminderCount + protocol.SensorCount + protocol.LevelSenosrCount
                               + protocol.LPortCount + protocol.SPortCount + protocol.DigitalInputCount
                               + protocol.TimerCount + protocol.LightCount + protocol.ProgrammableLogicCount;
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

                this.Info.LastUpdate = DateTime.Now;
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
        /// Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateCurrentPumpValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<CurrentPump>().Count() != 0)
            {
                foreach (var pump in this.Items.OfType<CurrentPump>())
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
        /// Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
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
                    var pump =
                        (CurrentPump)this.Items.FirstOrDefault(p => p is CurrentPump && ((CurrentPump)p).Index == index);

                    if (basicProtocol.IsCurrentPumpAssinged(i))
                    {
                        if (pump == null)
                        {
                            pump = new CurrentPump { Id = basicProtocol.GetCurrentPumpId(index), Index = index, };
                            this.Items.Add(pump);
                        }

                        pump.OldValue = pump.Value;
                        pump.Value = basicProtocol.GetCurrentPumpValue(i);
                    }
                    else
                    {
                        if (pump != null)
                        {
                            this.Items.Remove(pump);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2002, string.Format("CurrentPump {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the digital input values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateDigitalInputValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<DigitalInput>().Count() != 0)
            {
                foreach (var sensor in this.Items.OfType<DigitalInput>())
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
        /// Updates the digital inputs.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
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
                    var sensor =
                        (DigitalInput)this.Items.FirstOrDefault(p => p is DigitalInput && ((DigitalInput)p).Index == i1);
                    if (mode != DigitalInputFunction.NotUsed || this.Connection.GetAll)
                    {
                        if (sensor == null)
                        {
                            sensor = new DigitalInput { Id = basicProtocol.GetDigtialInputId(i), Index = i };
                            this.Items.Add(sensor);
                        }

                        sensor.Function = mode;

                        sensor.OldValue = sensor.Value;
                        sensor.Value = basicProtocol.GetDigitalInputState(i);
                    }
                    else
                    {
                        if (sensor != null)
                        {
                            this.Items.Remove(sensor);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2001, string.Format("Digital Input {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
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
                    var pump =
                        (DosingPump)
                            this.Items.FirstOrDefault(p => p is DosingPump && ((DosingPump)p).Channel == channel);
                    var settings = basicProtocol.GetTimerSettings(i);

                    if (settings.Mode == TimerMode.AutoDosing)
                    {
                        if (pump == null)
                        {
                            pump = new DosingPump { Id = basicProtocol.GetDousingPumpId(channel), Channel = channel };
                            this.Items.Add(pump);
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
                            this.Items.Remove(pump);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2002, string.Format("Timer {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the dosing pumps value.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="pump">
        /// The pump.
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
        /// Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateDosingPumpsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<DosingPump>().Count() != 0)
            {
                foreach (var pump in this.Items.OfType<DosingPump>())
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
        /// Updates the dosing rate.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="pump">
        /// The pump.
        /// </param>
        /// <param name="rate">
        /// The rate.
        /// </param>
        /// <param name="perDay">
        /// The per day.
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
        /// Updates the info.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateInfo(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Getting Controller Information";
            }

            this.Info.SoftwareVersion = basicProtocol.Version;
            this.Info.ProductId = basicProtocol.ProductId;
            this.Info.SerialNumber = basicProtocol.SerialNumber;
            this.Info.SoftwareDate = basicProtocol.SoftwareDate;
            this.Info.DeviceAddress = basicProtocol.DeviceAddress;
            if (this.Info.IsP3)
            {
                this.Connection.WebServerPort = basicProtocol.WebServerPort;
            }

            this.Info.Latitude = basicProtocol.Latitude;
            this.Info.Longitude = basicProtocol.Longitude;
            this.Info.MoonPhase = basicProtocol.MoonPhase;
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
                var reminder = this.Info.Reminders.FirstOrDefault(r => r.Index == index);

                if (nextReminder.HasValue)
                {
                    if (reminder == null)
                    {
                        reminder = new Reminder { Index = i };
                        this.Info.Reminders.Add(reminder);
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
                            Logger.Instance.Log(
                                new LogMessage(
                                    2,
                                    Language.GetResource("strReminder") + " " + reminder.Text
                                    + Language.GetResource("strIsOverdue")));
                        }
                    }
                    else
                    {
                        if (reminder.IsOverdue)
                        {
                            reminder.IsOverdue = false;
                            Logger.Instance.Log(
                                new LogMessage(
                                    0,
                                    Language.GetResource("strReminder") + " " + reminder.Text
                                    + Language.GetResource("strIsNotOverdue")));
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
                        this.Info.Reminders.Remove(reminder);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the alarm.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateInfoValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Info.SoftwareVersion == 0)
            {
                this.UpdateInfo(basicProtocol, progress);
            }
            else
            {
                this.Info.MoonPhase = basicProtocol.MoonPhase;
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
        /// Updates the L port values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateLPortValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<LPort>().Count() != 0)
            {
                foreach (var port in this.Items.OfType<LPort>())
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
        /// Gets the Devices.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <exception cref="ProtocolException">
        /// If there is a failure in the protocol
        /// </exception>
        /// <exception cref="ConnectionException">
        /// There is a failure in the communication
        /// </exception>
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
                    var port = (LPort)this.Items.FirstOrDefault(p => p is LPort && ((LPort)p).PortNumber == number);
                    if (mode.DeviceMode != DeviceMode.AlwaysOff || this.Connection.GetAll)
                    {
                        if (port == null)
                        {
                            port = new LPort { Id = basicProtocol.GetLPortId(i), PortNumber = i };
                            port.UpdateMode(mode, this.Items);
                            this.Items.Add(port);
                        }
                        else
                        {
                            port.UpdateMode(mode, this.Items);
                        }

                        port.SetValue(basicProtocol.GetLPortValue(i));
                    }
                    else
                    {
                        if (port != null)
                        {
                            this.Items.Remove(port);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2003, string.Format("LPort {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <exception cref="ProtocolException">
        /// If there is a failure in the protocol
        /// </exception>
        /// <exception cref="ConnectionException">
        /// There is a failure in the communication
        /// </exception>
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
                    var sensor =
                        (LevelSensor)this.Items.FirstOrDefault(p => p is LevelSensor && ((LevelSensor)p).Index == i1);

                    if (mode != LevelSensorOpertationMode.NotEnabled || this.Connection.GetAll)
                    {
                        if (sensor == null)
                        {
                            sensor = new LevelSensor { Id = basicProtocol.GetLevelId(i), Index = i };
                            this.Items.Add(sensor);
                        }

                        sensor.OpertationMode = mode;
                        var state = basicProtocol.GetLevelSensorState(i);
                        if (state.Alarm != sensor.IsAlarmOn)
                        {
                            sensor.IsAlarmOn = state.Alarm;
                            Logger.Instance.Log(
                                new LogMessage(
                                    state.Alarm == CurrentState.On ? 1 : 0,
                                    "Level Sensor " + sensor.DisplayName + " Alarm is now " + state.Alarm));
                        }

                        sensor.OldValue = sensor.Value;
                        sensor.Value = state.State;

                        sensor.WaterMode = state.WaterMode;
                    }
                    else
                    {
                        if (sensor != null)
                        {
                            this.Items.Remove(sensor);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2000, string.Format("Level Sensor {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the level sensors value.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="sensor">
        /// The sensor.
        /// </param>
        public void UpdateLevelSensorsValue(IProfilux basicProtocol, LevelSensor sensor)
        {
            var state = basicProtocol.GetLevelSensorState(sensor.Index);
            if (state.Alarm != sensor.IsAlarmOn)
            {
                sensor.IsAlarmOn = state.Alarm;
                Logger.Instance.Log(
                    new LogMessage(
                        state.Alarm == CurrentState.On ? 1 : 0,
                        "Level Sensor " + sensor.DisplayName + " Alarm is now " + state.Alarm));
            }

            sensor.OldValue = sensor.Value;
            sensor.Value = state.State;

            sensor.WaterMode = state.WaterMode;
        }

        /// <summary>
        /// Updates the level sensors values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateLevelSensorsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<LevelSensor>().Count() != 0)
            {
                foreach (var sensor in this.Items.OfType<LevelSensor>())
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
        /// Updates the light value.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="light">
        /// The light.
        /// </param>
        public void UpdateLightValue(IProfilux basicProtocol, Light light)
        {
            light.OldValue = light.Value;
            light.Value = basicProtocol.GetLightValue(light.Channel);
        }

        /// <summary>
        /// Updates the lights.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        /// if set to <c>true</c> [skip non essntals].
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
                    var light = (Light)this.Items.FirstOrDefault(p => p is Light && ((Light)p).Channel == channel);

                    if (basicProtocol.IsLightActive(i))
                    {
                        if (light == null)
                        {
                            light = new Light { Id = basicProtocol.GetLightId(channel), Channel = channel, };
                            this.Items.Add(light);
                        }

                        if (!skipNonEssntals)
                        {
                            light.IsDimmable = basicProtocol.IsLightDimmable(i);
                            this.UpdateLightOperationHours(basicProtocol, light);
                        }

                        light.OldValue = light.Value;
                        light.Value = basicProtocol.GetLightValue(i);

                        if (this.Info.IsP3)
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
                            this.Items.Remove(light);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2002, string.Format("Light {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the update lights values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateLightsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<Light>().Count() != 0)
            {
                foreach (var light in this.Items.OfType<Light>())
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
        /// The update maintiance mode.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public void UpdateMaintianceMode(IProfilux basicProtocol, IUpdateProgress progress, int index)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Maintiance Mode " + index;
            }

            this.Info.Maintenance[index].IsActive = basicProtocol.GetMaintenanceIsActive(index);
            this.Info.Maintenance[index].Duration = basicProtocol.GetMaintenanceDuration(index) * 60;
            this.Info.Maintenance[index].TimeLeft = basicProtocol.GetMaintenanceTimeLeft(index) * 60;
        }

        /// <summary>
        /// Updates the operation mode.
        /// </summary>
        /// <param name="operationMode">
        /// The operation mode.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateOperationMode(OperationMode operationMode, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.ProgressText = "Update Operation Mode";
            }

            if (this.Info.OperationMode != operationMode)
            {
                this.Info.OperationMode = operationMode;
                Logger.Instance.Log(
                    new LogMessage(
                        0,
                        Language.GetResource("strOperationModechanged") + Language.GetFrendyName(operationMode)));
            }
        }

        /// <summary>
        /// Updates the probe values.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateProbeValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<Probe>().Count() != 0)
            {
                foreach (Probe probe in this.Items.OfType<Probe>())
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
                    if (alarm != probe.IsAlarmOn)
                    {
                        probe.IsAlarmOn = alarm;
                        Logger.Instance.Log(
                            new LogMessage(
                                alarm == CurrentState.On ? 1 : 0,
                                "Sensor " + probe.DisplayName + " Alarm is now " + alarm));
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
        /// Updates the probes.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <param name="skipNonEssntals">
        /// if set to <c>true</c> [skip non essntals].
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
                var probe = (Probe)this.Items.FirstOrDefault(p => p is Probe && ((Probe)p).Index == i1);

                var sensorId = basicProtocol.GetSensorId(i, type);

                if (type != SensorType.None && type != SensorType.Free && (active || this.Connection.GetAll)
                    && (mode == SensorMode.Normal || this.Connection.GetAll))
                {
                    if (probe == null)
                    {
                        probe = new Probe
                                {
                                    Format = basicProtocol.GetSensorFormat(i),
                                    Id = sensorId,
                                    Index = i,
                                    SensorType = type,
                                    SensorMode = mode
                                };
                        this.Items.Add(probe);
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

                    if (this.Info.IsP3)
                    {
                        var probeName = basicProtocol.GetProbeName(i);
                        if (!string.IsNullOrEmpty(probeName))
                        {
                            probe.DisplayName = probeName;
                        }
                    }

                    probe.SetValue(basicProtocol.GetSensorValue(i));
                    var alarm = basicProtocol.GetSensorAlarm(i);
                    if (alarm != probe.IsAlarmOn)
                    {
                        probe.IsAlarmOn = alarm;
                        Logger.Instance.Log(
                            new LogMessage(
                                alarm == CurrentState.On ? 1 : 0,
                                "Sensor " + probe.DisplayName + " Alarm is now " + alarm));
                    }
                }
                else
                {
                    if (probe != null)
                    {
                        this.Items.Remove(probe);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the reminders.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateReminders(IProfilux basicProtocol, IUpdateProgress progress)
        {
            foreach (var reminder in this.Info.Reminders)
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
                            Logger.Instance.Log(
                                new LogMessage(
                                    2,
                                    Language.GetResource("strReminder") + " " + reminder.Text + " "
                                    + Language.GetResource("strIsOverdue")));
                        }
                    }
                    else
                    {
                        if (reminder.IsOverdue)
                        {
                            reminder.IsOverdue = false;
                            Logger.Instance.Log(
                                new LogMessage(
                                    0,
                                    Language.GetResource("strReminder") + " " + reminder.Text + " "
                                    + Language.GetResource("strIsNotOverdue")));
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
                var logic = this.ProgrammableLogic.FirstOrDefault(r => r.Index == index);

                var input1 = basicProtocol.GetProgramLogicInput(0, i);
                var input2 = basicProtocol.GetProgramLogicInput(1, i);

                if (input1.DeviceMode != DeviceMode.AlwaysOff && input2.DeviceMode != DeviceMode.AlwaysOff)
                {
                    if (logic == null)
                    {
                        logic = new ProgramableLogic { Index = i };
                        this.ProgrammableLogic.Add(logic);
                    }

                    logic.Input1 = input1;
                    logic.Input2 = input2;
                    logic.Function = basicProtocol.GetProgramLogicFunction(i);
                }
                else
                {
                    if (logic != null)
                    {
                        this.ProgrammableLogic.Remove(logic);
                    }
                }
            }

            foreach (var logic in this.ProgrammableLogic)
            {
                logic.Update(this.Items, this.ProgrammableLogic);
            }
        }

        /// <summary>
        /// Updates the S port value.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="port">
        /// The port.
        /// </param>
        public void UpdateSPortValue(IProfilux basicProtocol, SPort port)
        {
            port.SetValue(basicProtocol.GetSPortValue(port.PortNumber));
        }

        /// <summary>
        /// Gets the Devices.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <exception cref="ProtocolException">
        /// If there is a failure in the protocol
        /// </exception>
        /// <exception cref="ConnectionException">
        /// There is a failure in the commuincation
        /// </exception>
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
                    var port = (SPort)this.Items.FirstOrDefault(p => p is SPort && ((SPort)p).PortNumber == portNumber);

                    if (mode.DeviceMode != DeviceMode.AlwaysOff || this.Connection.GetAll)
                    {
                        if (port == null)
                        {
                            port = new SPort { Id = basicProtocol.GetSPortId(portNumber), PortNumber = portNumber };
                            port.UpdateMode(mode, this.Items, this.ProgrammableLogic);
                            this.Items.Add(port);
                        }
                        else
                        {
                            port.UpdateMode(mode, this.Items, this.ProgrammableLogic);
                        }

                        port.SetValue(basicProtocol.GetSPortValue(i));
                        port.OldCurrentValue = port.Current;
                        port.Current = basicProtocol.GetSPortCurrent(i);

                        if (this.Info.IsP3)
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
                            this.Items.Remove(port);
                        }
                    }
                }
                catch (ErrorCodeException ex)
                {
                    Logger.Instance.Log(
                        new LogMessage(2002, string.Format("SPort {0} not supported code {1}", i, ex.ErrorCode))
                        {
                            Exception
                                =
                                ex
                        });
                }
            }
        }

        /// <summary>
        /// Updates the S ports value.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void UpdateSPortsValues(IProfilux basicProtocol, IUpdateProgress progress)
        {
            if (this.Items.OfType<SPort>().Count() != 0)
            {
                foreach (SPort port in this.Items.OfType<SPort>())
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
        /// Updates the values.
        /// </summary>
        /// <param name="protocol">
        /// The protocol.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <returns>
        /// retunes True if the values were updated
        /// </returns>
        public bool UpdateValues(IProfilux protocol, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Log Data Started";
                double steps = this.Items.OfType<Probe>().Count() + this.Items.OfType<LevelSensor>().Count() + this.Items.OfType<SPort>().Count() + this.Items.OfType<LPort>().Count()
                               + this.Items.OfType<Light>().Count() + this.Items.OfType<DosingPump>().Count() + this.Info.Reminders.Count;
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

                this.Info.LastUpdate = DateTime.Now;
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
        /// Waters the change.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="levelSensor">
        /// The level sensor.
        /// </param>
        public void WaterChange(IProfilux basicProtocol, LevelSensor levelSensor)
        {
            basicProtocol.WaterChange(levelSensor.Index);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the probe operational hours.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="probe">
        /// The probe.
        /// </param>
        private static void UpdateProbeOperationalHours(IProfilux basicProtocol, Probe probe)
        {
            var hours = basicProtocol.GetProbeOperationHours(probe.Index);

            if (probe.EnableMaxOperationHours
                && (!probe.IsOverMaxOperationHours && probe.MaxOperationHours < (hours / 60.0)))
            {
                Logger.Instance.Log(
                    new LogMessage(2, string.Format(Language.GetResource("strOpHoursOver"), probe.DisplayName)));
            }

            probe.OperationHours = hours;
        }


        /////////////////////////////////////////////////

        /// <summary>
        /// Updates the light operation hours.
        /// </summary>
        /// <param name="basicProtocol">
        /// The basic protocol.
        /// </param>
        /// <param name="light">
        /// The light.
        /// </param>
        private void UpdateLightOperationHours(IProfilux basicProtocol, Light light)
        {
            var hours = basicProtocol.GetLightOperationHours(light.Channel);

            if (light.EnableMaxOperationHours
                && (!light.IsOverMaxOperationHours && light.MaxOperationHours < (hours / 60.0)))
            {
                Logger.Instance.Log(
                    new LogMessage(2, string.Format(Language.GetResource("strOpHoursOver"), light.DisplayName)));
            }

            light.OperationHours = hours;
        }

        #endregion
    }
}