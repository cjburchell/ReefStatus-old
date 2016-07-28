
namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Commands;
    using RedPoint.ReefStatus.Common.Communication;
    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;
    using RedPoint.ReefStatus.Common.ViewModel;


    public class ControllerViewModel : BindableBase
    {
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer lightTestTimer;

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer timer;

        /// <summary>
        /// The add user value command.
        /// </summary>
        private ICommand addUserValueCommand;

        /// <summary>
        /// The all lights off command.
        /// </summary>
        private ICommand allLightsOffCommand;

        /// <summary>
        /// The all lights on command.
        /// </summary>
        private ICommand allLightsOnCommand;

        /// <summary>
        /// The back command
        /// </summary>
        private ICommand backCommand;

        /// <summary>
        /// The clear level alarm command.
        /// </summary>
        private ICommand clearLevelAlarmCommand;

        /// <summary>
        /// The close connection options
        /// </summary>
        private ICommand closeConnectionOptions;

        /// <summary>
        /// Feed Command
        /// </summary>
        private ICommand feedCommand;

        /// <summary>
        /// The light test command.
        /// </summary>
        private ICommand lightTestCommand;

        /// <summary>
        /// maintenance Command
        /// </summary>
        private ICommand maintenanceCommand;

        /// <summary>
        /// Manual Command
        /// </summary>
        private ICommand manualCommand;

        /// <summary>
        /// Manual Illumination Command
        /// </summary>
        private ICommand manualIlluminationCommand;

        /// <summary>
        /// The new dosing command.
        /// </summary>
        private ICommand newDosingCommand;
        /// <summary>
        /// The play light test command
        /// </summary>
        private ICommand playLightTestCommand;

        /// <summary>
        /// Command to reset the hours
        /// </summary>
        private ICommand resetOperationalHoursCommand;

        /// <summary>
        /// The show connection options.
        /// </summary>
        private bool showConnectionOptions;

        /// <summary>
        /// The show item command
        /// </summary>
        private ICommand showItemCommand;

        /// <summary>
        /// The start water change command.
        /// </summary>
        private ICommand startWaterChangeCommand;

        /// <summary>
        /// The stop light test command
        /// </summary>
        private DelegateCommand stopLightTestCommand;

        /// <summary>
        /// The update dosing command.
        /// </summary>
        private ICommand updateDosingCommand;

        /// <summary>
        /// The new dosing per day.
        /// </summary>
        private int newDosingPerDay;

        /// <summary>
        /// The new dosing rate.
        /// </summary>
        private int newDosingRate;

        /// <summary>
        /// The new user value.
        /// </summary>
        private double newUserValue;

        /// <summary>
        /// The new user value date.
        /// </summary>
        private DateTime newUserValueDate;

        /// <summary>
        /// The is playing light test
        /// </summary>
        private bool isPlayingLightTest;

        /// <summary>
        /// The current selection.
        /// </summary>
        private object currentSelection;

        /// <summary>
        /// The display item area
        /// </summary>
        private bool displayItemArea;

        private int lightTestTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerViewModel" /> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ControllerViewModel(Controller controller)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.Controller = controller;
            this.Commands = this.Controller.Commands;
            this.Remote = new RemoteViewModel(this.Commands, this.Controller);

            this.UpdateCommand = new DelegateCommand(this.UpdateItem, () => (this.CurrentSelection is UserInfo));
            this.DeleteCommand = new DelegateCommand<BaseInfo>(this.DeleteItem, arg => (arg is UserInfo));
            this.AddDataItemCommand = new DelegateCommand(this.AddDataItem);

            this.timer = new Timer();
            this.timer.Tick += this.TimerTick;
            this.timer.Interval = 1000;
            this.timer.Start();

            this.lightTestTimer = new Timer();
            this.lightTestTimer.Tick += this.LightTestTimerTick;
            this.lightTestTimer.Interval = 33;
        }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        public Controller Controller { get; private set; }

        /// <summary>
        /// Gets AddDataItemCommand.
        /// </summary>
        public ICommand AddDataItemCommand { get; private set; }

        /// <summary>
        /// Gets the add user value command.
        /// </summary>
        /// <value>The add user value command.</value>
        public ICommand AddUserValueCommand
        {
            get
            {
                return this.addUserValueCommand ?? (this.addUserValueCommand = new DelegateCommand(this.AddUserValue));
            }
        }

        /// <summary>
        /// Gets AllLightsOffCommand.
        /// </summary>
        public ICommand AllLightsOffCommand
        {
            get
            {
                return this.allLightsOffCommand
                       ?? (this.allLightsOffCommand = new DelegateCommand(() => this.SetAllLights(false)));
            }
        }

        /// <summary>
        /// Gets AllLightsOnCommand.
        /// </summary>
        public ICommand AllLightsOnCommand
        {
            get
            {
                return this.allLightsOnCommand
                       ?? (this.allLightsOnCommand = new DelegateCommand(() => this.SetAllLights(true)));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return this.backCommand
                       ?? (this.backCommand = new DelegateCommand(() => { this.DisplayItemArea = false; }));
            }
        }

        /// <summary>
        /// Gets ClearLevelAlarmCommand.
        /// </summary>
        public ICommand ClearLevelAlarmCommand
        {
            get
            {
                return this.clearLevelAlarmCommand
                       ?? (this.clearLevelAlarmCommand =
                           new DelegateCommand<BaseInfo>(
                               this.ClearLevelAlarm,
                               arg =>
                                   (arg is LevelSensor && ((LevelSensor)arg).IsAlarmOn == CurrentState.On)));
            }
        }

        [XmlIgnore]
        public ICommand CloseConnectionOptions
        {
            get
            {
                return this.closeConnectionOptions
                       ?? (this.closeConnectionOptions =
                           new DelegateCommand(() => { this.ShowConnectionOptions = false; }));
            }
        }

        /// <summary>
        /// Gets or sets the light value.
        /// </summary>
        /// <value>The light value.</value>
        public int LightTestTimeValue
        {
            get
            {
                return this.LightTestTime;
            }

            set
            {
                if (this.LightTestTime == value)
                {
                    return;
                }

                this.Commands.SendLightTestTime(value);
                this.LightTestTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the light value.
        /// </summary>
        /// <value>The light value.</value>
        public int LightTestTime
        {
            get
            {
                return this.lightTestTime;
            }

            set
            {
                if (this.lightTestTime != value)
                {
                    this.lightTestTime = value;
                    this.OnPropertyChanged(() => this.LightTestTime);
                    this.OnPropertyChanged(() => this.LightTestTimeValue);
                }
            }
        }

        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>The commands.</value>
        public CommandThread Commands { get; private set; }

        /// <summary>
        /// Gets or sets CurrentSelection.
        /// </summary>
        public object CurrentSelection
        {
            get
            {
                return this.currentSelection;
            }

            set
            {
                if (this.currentSelection == value)
                {
                    return;
                }

                this.currentSelection = value;
                this.OnPropertyChanged(() => this.CurrentSelection);
            }
        }

        /// <summary>
        /// Gets DeleteCommand.
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        public bool DisplayItemArea
        {
            get
            {
                return this.displayItemArea;
            }
            set
            {
                if (value.Equals(this.displayItemArea))
                {
                    return;
                }
                this.displayItemArea = value;
                this.OnPropertyChanged(() => this.DisplayItemArea);
            }
        }

        /// <summary>
        /// Gets the feed command.
        /// </summary>
        /// <value>The feed command.</value>
        public ICommand FeedCommand
        {
            get
            {
                return this.feedCommand ?? (this.feedCommand = new DelegateCommand(this.Feed));
            }
        }

        [XmlIgnore]
        public bool IsPlayingLightTest
        {
            get
            {
                return this.isPlayingLightTest;
            }
            set
            {
                if (this.isPlayingLightTest == value)
                {
                    return;
                }

                this.isPlayingLightTest = value;
                this.OnPropertyChanged(() => this.IsPlayingLightTest);
            }
        }

        /// <summary>
        /// Gets LightTestCommand.
        /// </summary>
        [XmlIgnore]
        public ICommand LightTestCommand
        {
            get
            {
                return this.lightTestCommand ?? (this.lightTestCommand = new DelegateCommand(this.LightTest));
            }
        }


        /// <summary>
        /// Gets the maintenance command.
        /// </summary>
        /// <value>The maintenance command.</value>
        [XmlIgnore]
        public ICommand MaintenanceCommand
        {
            get
            {
                return this.maintenanceCommand
                       ?? (this.maintenanceCommand = new DelegateCommand<Maintenance>(this.Maintenance));
            }
        }

        /// <summary>
        /// Gets the manual command.
        /// </summary>
        /// <value>The manual command.</value>
        [XmlIgnore]
        public ICommand ManualCommand
        {
            get
            {
                return this.manualCommand ?? (this.manualCommand = new DelegateCommand(this.ManualSockets));
            }
        }

        /// <summary>
        /// Gets the manual illumination command.
        /// </summary>
        /// <value>The manual illumination command.</value>
        [XmlIgnore]
        public ICommand ManualIlluminationCommand
        {
            get
            {
                return this.manualIlluminationCommand
                       ?? (this.manualIlluminationCommand = new DelegateCommand(this.ManualIllumination));
            }
        }

        /// <summary>
        /// Gets the new dosing command.
        /// </summary>
        /// <value>The new dosing command.</value>
        [XmlIgnore]
        public ICommand NewDosingCommand
        {
            get
            {
                return this.newDosingCommand
                       ?? (this.newDosingCommand =
                           new DelegateCommand(
                               this.NewDosing,
                               () => (this.CurrentSelection is DosingPump)));
            }
        }

        /// <summary>
        /// Gets or sets the new dosing per day.
        /// </summary>
        /// <value>The new dosing per day.</value>
        [XmlIgnore]
        public int NewDosingPerDay
        {
            get
            {
                return this.newDosingPerDay;
            }

            set
            {
                this.newDosingPerDay = value;
                this.OnPropertyChanged(() => this.NewDosingPerDay);
            }
        }

        /// <summary>
        /// Gets or sets the new dosing rate.
        /// </summary>
        /// <value>The new dosing rate.</value>
        [XmlIgnore]
        public int NewDosingRate
        {
            get
            {
                return this.newDosingRate;
            }

            set
            {
                this.newDosingRate = value;
                this.OnPropertyChanged(() => this.NewDosingRate);
            }
        }

        /// <summary>
        /// Gets or sets the new user value.
        /// </summary>
        /// <value>The new user value.</value>
        [XmlIgnore]
        public double NewUserValue
        {
            get
            {
                return this.newUserValue;
            }

            set
            {
                if (this.newUserValue == value)
                {
                    return;
                }

                this.newUserValue = value;
                this.OnPropertyChanged(() => this.NewUserValue);
            }
        }

        /// <summary>
        /// Gets or sets the new user value date.
        /// </summary>
        /// <value>The new user value date.</value>
        [XmlIgnore]
        public DateTime NewUserValueDate
        {
            get
            {
                return this.newUserValueDate;
            }

            set
            {
                if (this.newUserValueDate != value)
                {
                    this.newUserValueDate = value;
                    this.OnPropertyChanged(() => this.NewUserValueDate);
                }
            }
        }

        [XmlIgnore]
        public ICommand PlayLightTestCommand
        {
            get
            {
                return this.playLightTestCommand
                       ?? (this.playLightTestCommand = new DelegateCommand(this.PlayLightTest));
            }
        }

        /// <summary>
        /// Gets or sets Remote.
        /// </summary> 
        [XmlIgnore]
        public RemoteViewModel Remote { get; set; }

        /// <summary>
        /// Gets the reset operational hours command.
        /// </summary>
        /// <value>The reset operational hours command.</value>
        [XmlIgnore]
        public ICommand ResetOperationalHoursCommand
        {
            get
            {
                return this.resetOperationalHoursCommand
                       ?? (this.resetOperationalHoursCommand =
                           new DelegateCommand<BaseInfo>(
                               this.ResetOperationalHours,
                               arg => (arg != null && (arg is Probe || arg is Light))));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowConnectionOptions.
        /// </summary>
        [XmlIgnore]
        public bool ShowConnectionOptions
        {
            get
            {
                return this.showConnectionOptions;
            }

            set
            {
                if (this.showConnectionOptions != value)
                {
                    this.showConnectionOptions = value;
                    this.OnPropertyChanged(() => this.ShowConnectionOptions);
                }
            }
        }

        [XmlIgnore]
        public ICommand ShowItemCommand
        {
            get
            {
                return this.showItemCommand ?? (this.showItemCommand = new DelegateCommand<object>(
                    item =>
                    {
                        this.CurrentSelection = item;
                        this.DisplayItemArea = true;
                    }));
            }
        }

        /// <summary>
        /// Gets the new dosing command.
        /// </summary>
        /// <value>The new dosing command.</value>
        [XmlIgnore]
        public ICommand StartWaterChangeCommand
        {
            get
            {
                return this.startWaterChangeCommand
                       ?? (this.startWaterChangeCommand =
                           new DelegateCommand<BaseInfo>(
                               this.StartWaterChange,
                               arg => (arg is LevelSensor && ((LevelSensor)arg).CanDoWaterChange)));
            }
        }

        [XmlIgnore]
        public ICommand StopLightTestCommand
        {
            get
            {
                return this.stopLightTestCommand
                       ?? (this.stopLightTestCommand =
                           new DelegateCommand(this.StopLightTest, () => this.LightTestTime != 0));
            }
        }

        /// <summary>
        /// Gets UpdateCommand.
        /// </summary>
        [XmlIgnore]
        public ICommand UpdateCommand { get; private set; }

        /// <summary>
        /// Gets UpdateDosingCommand.
        /// </summary>
        [XmlIgnore]
        public ICommand UpdateDosingCommand
        {
            get
            {
                return this.updateDosingCommand
                       ?? (this.updateDosingCommand =
                           new DelegateCommand(
                               this.UpdateDosing,
                               () => (this.CurrentSelection is DosingPump)));
            }
        }

        /// <summary>
        /// Maintenances this instance.
        /// </summary>
        /// <param name="maintenance">
        /// The maintenance.
        /// </param>
        public void Maintenance(Maintenance maintenance)
        {
            this.Commands.SendMaintenance(!maintenance.IsActive, maintenance);
        }

        /// <summary>
        /// Feeds this instance.
        /// </summary>
        public void Feed()
        {
            this.Commands.SendFeed(true);
        }

        /// <summary>
        /// The add data item.
        /// </summary>
        private void AddDataItem()
        {
            var userIndex = this.Controller.Items.OfType<UserInfo>()
                .Aggregate(
                    0,
                    (current, item) => Math.Max(int.Parse(item.Id.Substring(3), CultureInfo.InvariantCulture), current));

            userIndex += 1;

            BaseInfo settings = new UserInfo
            {
                Id = "USR" + userIndex,
                DisplayName = "USR" + userIndex,
                Units = string.Empty
            };

            this.Controller.Items.Add(settings);
            ReefStatusSettings.Instance.Save();

            this.CurrentSelection = settings;
        }

        /// <summary>
        /// Adds the user value.
        /// </summary>
        private void AddUserValue()
        {
            var userInfo = this.CurrentSelection as UserInfo;
            if (userInfo != null)
            {
                // only display the newest value
                if (userInfo.Time < this.NewUserValueDate)
                {
                    userInfo.Value = this.NewUserValue;
                    userInfo.Time = this.NewUserValueDate;
                }

                try
                {
                    using (var access = ReefStatusSettings.Instance.Logging.Connection.Create())
                    {
                        access.InsertItem(
                            this.NewUserValue,
                            this.NewUserValueDate,
                            userInfo.Id,
                            false,
                            userInfo.Controller.Id,
                            null);
                    }
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }

                ReefStatusSettings.Instance.Save();

                this.dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            if (userInfo.HasGraph)
                            {
                                userInfo.Graph.Refresh();
                            }

                            if (userInfo.HasDataPoints)
                            {
                                userInfo.DataPoints.Refresh();
                            }

                            if (userInfo.HasReport)
                            {
                                userInfo.Report.UpdateReport();
                            }
                        }));
            }
        }

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void ClearLevelAlarm(BaseInfo item)
        {
            if (item != null)
            {
                this.Commands.SendClearLevelAlarm((LevelSensor)item);
            }
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void DeleteItem(BaseInfo item)
        {
            this.Controller.Items.Remove(item);
            ReefStatusSettings.Instance.Save();

            if (this.CurrentSelection == item)
            {
                this.CurrentSelection = null;
            }
        }

        /// <summary>
        /// The light test.
        /// </summary>
        private void LightTest()
        {
            this.Commands.SendOperationMode(
                this.Controller.Info.OperationMode == OperationMode.LightTest ? OperationMode.Normal : OperationMode.LightTest);
        }

        private void LightTestTimerTick(object sender, EventArgs e)
        {
            if (this.Controller.Info.OperationMode != OperationMode.LightTest)
            {
                this.IsPlayingLightTest = false;
            }

            if (!this.IsPlayingLightTest)
            {
                this.lightTestTimer.Stop();
                return;
            }

            var value = this.LightTestTimeValue + 1;
            if (value > 1439)
            {
                this.StopLightTest();
            }

            this.LightTestTimeValue = value;
            this.stopLightTestCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Manuals the sockets.
        /// </summary>
        private void ManualIllumination()
        {
            this.Commands.SendOperationMode(
                this.Controller.Info.OperationMode == OperationMode.ManualIllumination
                    ? OperationMode.Normal
                    : OperationMode.ManualIllumination);
        }

        /// <summary>
        /// Manuals the sockets.
        /// </summary>
        private void ManualSockets()
        {
            this.Commands.SendOperationMode(
                this.Controller.Info.OperationMode == OperationMode.ManualSockets
                    ? OperationMode.Normal
                    : OperationMode.ManualSockets);
        }

        /// <summary>
        /// News the dosing.
        /// </summary>
        private void NewDosing()
        {
            var pump = this.CurrentSelection as DosingPump;
            if (pump != null)
            {
                pump.Commands.SendUpdateDosingRate(pump, this.NewDosingRate, this.NewDosingPerDay);
                this.dispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            if (pump.HasGraph)
                            {
                                pump.Graph.Refresh();
                            }

                            if (pump.HasDataPoints)
                            {
                                pump.DataPoints.Refresh();
                            }

                            if (pump.HasReport)
                            {
                                pump.Report.UpdateReport();
                            }
                        }));
            }
        }

        private void PlayLightTest()
        {
            if (this.IsPlayingLightTest)
            {
                this.IsPlayingLightTest = false;
                this.lightTestTimer.Stop();
            }
            else
            {
                this.IsPlayingLightTest = true;
                this.lightTestTimer.Start();
            }
        }

        /// <summary>
        /// Resets the operational hours.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void ResetOperationalHours(BaseInfo item)
        {
            this.Commands.SendResetOperationalHours(item);
        }

        /// <summary>
        /// The set all lights.
        /// </summary>
        /// <param name="isOn">
        /// The is on.
        /// </param>
        private void SetAllLights(bool isOn)
        {
            foreach (var light in this.Controller.Items.OfType<Light>())
            {
                this.Commands.SendLightState(light, isOn ? 100 : 0, true);
            }
        }

        /// <summary>
        /// The start water change.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        private void StartWaterChange(BaseInfo item)
        {
            this.Commands.SendStartWaterChange((LevelSensor)item);
        }

        private void StopLightTest()
        {
            this.IsPlayingLightTest = false;
            this.LightTestTimeValue = 0;
            this.lightTestTimer.Stop();
            this.stopLightTestCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// The timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TimerTick(object sender, EventArgs e)
        {
            foreach (var maintenance in this.Controller.Info.Maintenance.Where(maintenance => maintenance.IsActive))
            {
                if (maintenance.TimeLeft == 0)
                {
                    this.Commands.UpdateMaintenance(maintenance);
                }
                else
                {
                    maintenance.TimeLeft--;
                }
            }
        }

        /// <summary>
        /// The update dosing.
        /// </summary>
        private void UpdateDosing()
        {
            var pump = this.CurrentSelection as DosingPump;
            if (pump != null)
            {
                this.NewDosingPerDay = pump.PerDay;
                this.NewDosingRate = pump.Rate;
            }
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        private void UpdateItem()
        {
            this.NewUserValueDate = DateTime.Now;
            var item = this.CurrentSelection as BaseInfo;
            if (item != null && item.Value != null && item.Value is double)
            {
                this.NewUserValue = (double)item.Value;
            }
            else
            {
                this.NewUserValue = 0;
            }
        }
    }
}
