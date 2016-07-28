// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusViewModel.cs" company="RedpointGames">
//   2010
// </copyright>
// <summary>
//   The status view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Input;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The status view model.
    /// </summary>
    public class StatusViewModel : ViewModelBase
    {
        #region Constants and Fields

        /// <summary>
        /// The add user value command.
        /// </summary>
        private ICommand addUserValueCommand;

        /// <summary>
        /// The clear level alarm command.
        /// </summary>
        private ICommand clearLevelAlarmCommand;

        /// <summary>
        /// The current selection.
        /// </summary>
        private BaseInfo currentSelection;

        /// <summary>
        /// The new dosing command.
        /// </summary>
        private ICommand newDosingCommand;

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
        /// Command to reset the hours
        /// </summary>
        private ICommand resetOperationalHoursCommand;

        /// <summary>
        /// The selected tab.
        /// </summary>
        private int selectedTab;

        /// <summary>
        /// The start water change command.
        /// </summary>
        private ICommand startWaterChangeCommand;

        /// <summary>
        /// The update dosing command.
        /// </summary>
        private ICommand updateDosingCommand;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusViewModel"/> class. 
        /// Prevents a default instance of the <see cref="StatusViewModel"/> class from being created.
        /// </summary>
        /// <param name="controler">
        /// The controler.
        /// </param>
        public StatusViewModel(Controler controler)
        {
            this.UpdateCommand = new DelegateCommand(
                this.UpdateItem, () => (this.CurrentSelection != null && this.CurrentSelection is UserInfo));
            this.DeleteCommand = new DelegateCommand<BaseInfo>(
                this.DeleteItem, arg => (arg != null && arg is UserInfo));
            this.AddDataItemCommand = new DelegateCommand(this.AddDataItem);

            this.Controler = controler;
        }

        #endregion

        #region Properties

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
        /// Gets the new dosing command.
        /// </summary>
        /// <value>The new dosing command.</value>
        public ICommand ClearLevelAlarmCommand
        {
            get
            {
                return this.clearLevelAlarmCommand ??
                       (this.clearLevelAlarmCommand =
                        new DelegateCommand<BaseInfo>(
                            this.ClearLevelAlarm,
                            arg =>
                            (arg != null && arg is LevelSensor &&
                             ((LevelSensor)arg).IsAlarmOn == CurrentState.On)));
            }
        }

        /// <summary>
        /// Gets the controler.
        /// </summary>
        /// <value>The controler.</value>
        public Controler Controler { get; private set; }

        /// <summary>
        /// Gets or sets CurrentPumpSelection.
        /// </summary>
        public BaseInfo CurrentPumpSelection
        {
            get
            {
                return this.CurrentSelection is CurrentPump ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets CurrentSelection.
        /// </summary>
        public BaseInfo CurrentSelection
        {
            get
            {
                return this.currentSelection;
            }

            set
            {
                if (this.currentSelection != value)
                {
                    this.currentSelection = value;
                    this.OnPropertyChanged("CurrentSelection");
                    this.OnPropertyChanged("ProbeSelection");
                    this.OnPropertyChanged("LevelSelection");
                    this.OnPropertyChanged("DigitalInputSelection");
                    this.OnPropertyChanged("SPortSelection");
                    this.OnPropertyChanged("LPortSelection");
                    this.OnPropertyChanged("UserValueSelection");
                    this.OnPropertyChanged("LightSelection");
                    this.OnPropertyChanged("DosingPumpSelection");
                    this.OnPropertyChanged("CurrentPumpSelection");
                    this.OnChangeAllGraphStates();
                }
            }
        }

        /// <summary>
        /// Gets DeleteCommand.
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        /// <summary>
        /// Gets or sets the digital input selection.
        /// </summary>
        /// <value>The digital input selection.</value>
        public BaseInfo DigitalInputSelection
        {
            get
            {
                return this.CurrentSelection is DigitalInput ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the dosing pump selection.
        /// </summary>
        /// <value>The dosing pump selection.</value>
        public BaseInfo DosingPumpSelection
        {
            get
            {
                return this.CurrentSelection is DosingPump ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the graph range.
        /// </summary>
        /// <value>The graph range.</value>
        public GraphRange GraphRange
        {
            get
            {
                if (this.CurrentSelection != null)
                {
                    return this.CurrentSelection.Range;
                }

                return GraphRange.All;
            }

            set
            {
                if (this.CurrentSelection != null && this.CurrentSelection.Range != value)
                {
                    this.CurrentSelection.Range = value;
                    this.OnChangeAllGraphStates();
                }
            }
        }

        /// <summary>
        /// Gets or sets the L port selection.
        /// </summary>
        /// <value>The L port selection.</value>
        public BaseInfo LPortSelection
        {
            get
            {
                return this.CurrentSelection is LPort ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the level selection.
        /// </summary>
        /// <value>The level selection.</value>
        public BaseInfo LevelSelection
        {
            get
            {
                return this.CurrentSelection is LevelSensor ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets the light selection.
        /// </summary>
        /// <value>The light selection.</value>
        public BaseInfo LightSelection
        {
            get
            {
                return this.CurrentSelection is Light ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets the new dosing command.
        /// </summary>
        /// <value>The new dosing command.</value>
        public ICommand NewDosingCommand
        {
            get
            {
                return this.newDosingCommand ??
                       (this.newDosingCommand =
                        new DelegateCommand(
                            this.NewDosing, () => (this.CurrentSelection != null && this.CurrentSelection is DosingPump)));
            }
        }

        /// <summary>
        /// Gets or sets the new dosing per day.
        /// </summary>
        /// <value>The new dosing per day.</value>
        public int NewDosingPerDay
        {
            get
            {
                return this.newDosingPerDay;
            }

            set
            {
                this.newDosingPerDay = value;
                this.OnPropertyChanged("NewDosingPerDay");
            }
        }

        /// <summary>
        /// Gets or sets the new dosing rate.
        /// </summary>
        /// <value>The new dosing rate.</value>
        public int NewDosingRate
        {
            get
            {
                return this.newDosingRate;
            }

            set
            {
                this.newDosingRate = value;
                this.OnPropertyChanged("NewDosingRate");
            }
        }

        /// <summary>
        /// Gets or sets the new user value.
        /// </summary>
        /// <value>The new user value.</value>
        public double NewUserValue
        {
            get
            {
                return this.newUserValue;
            }

            set
            {
                if (this.newUserValue != value)
                {
                    this.newUserValue = value;
                    this.OnPropertyChanged("NewUserValue");
                }
            }
        }

        /// <summary>
        /// Gets or sets the new user value date.
        /// </summary>
        /// <value>The new user value date.</value>
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
                    this.OnPropertyChanged("NewUserValueDate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the probe selection.
        /// </summary>
        /// <value>The probe selection.</value>
        public BaseInfo ProbeSelection
        {
            get
            {
                return this.CurrentSelection is Probe ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets the reset operational hours command.
        /// </summary>
        /// <value>The reset operational hours command.</value>
        public ICommand ResetOperationalHoursCommand
        {
            get
            {
                return this.resetOperationalHoursCommand ??
                       (this.resetOperationalHoursCommand =
                        new DelegateCommand<BaseInfo>(
                            this.ResetOperationalHours,
                            arg =>
                            (arg != null &&
                             (arg is Probe || arg is Light))));
            }
        }

        /// <summary>
        /// Gets or sets the S port selection.
        /// </summary>
        /// <value>The S port selection.</value>
        public BaseInfo SPortSelection
        {
            get
            {
                return this.CurrentSelection is SPort ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        /// <summary>
        /// Gets or sets SelectedTab.
        /// </summary>
        public int SelectedTab
        {
            get
            {
                return this.selectedTab;
            }

            set
            {
                if (this.selectedTab != value)
                {
                    this.selectedTab = value;
                    this.OnPropertyChanged("SelectedTab");
                }
            }
        }

        /// <summary>
        /// Gets the new dosing command.
        /// </summary>
        /// <value>The new dosing command.</value>
        public ICommand StartWaterChangeCommand
        {
            get
            {
                return this.startWaterChangeCommand ??
                       (this.startWaterChangeCommand =
                        new DelegateCommand<BaseInfo>(
                            this.StartWaterChange,
                            arg =>
                            (arg != null && arg is LevelSensor &&
                             ((LevelSensor)arg).CanDoWaterChange)));
            }
        }

        /// <summary>
        /// Gets UpdateCommand.
        /// </summary>
        public ICommand UpdateCommand { get; private set; }

        /// <summary>
        /// Gets UpdateDosingCommand.
        /// </summary>
        public ICommand UpdateDosingCommand
        {
            get
            {
                return this.updateDosingCommand ??
                       (this.updateDosingCommand =
                        new DelegateCommand(
                            this.UpdateDosing,
                            () => (this.CurrentSelection != null && this.CurrentSelection is DosingPump)));
            }
        }

        /// <summary>
        /// Gets or sets the user value selection.
        /// </summary>
        /// <value>The user value selection.</value>
        public BaseInfo UserValueSelection
        {
            get
            {
                return this.CurrentSelection is UserInfo ? this.CurrentSelection : null;
            }

            set
            {
                this.CurrentSelection = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called when [change all graph states].
        /// </summary>
        public void OnChangeAllGraphStates()
        {
            this.OnPropertyChanged("DataPoints");
            this.OnPropertyChanged("Report");
            this.OnPropertyChanged("IsGraphShown");
            this.OnPropertyChanged("GraphRange");
            this.OnPropertyChanged("IsCurrentGraphShown");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the data item.
        /// </summary>
        private void AddDataItem()
        {
            int userIndex = this.Controler.UserValues.Cast<UserInfo>().Aggregate(0, (current, item) => Math.Max(int.Parse(item.Id.Substring(3), CultureInfo.InvariantCulture), current));

            userIndex += 1;

            BaseInfo settings = new UserInfo
                {
                   Id = "USR" + userIndex, DisplayName = "USR" + userIndex, Units = string.Empty 
                };

            this.Controler.Items.Add(settings);
            this.Controler.ChangeUserValues();
            ReefStatusSettings.Instance.Save();
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
                    using (IDataAccess access = ReefStatusSettings.Instance.Logging.Connection.Create())
                    {
                        access.InsertItem(
                            this.NewUserValue, this.NewUserValueDate, userInfo.Id, false, userInfo.Controler.Id);
                    }
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }

                ReefStatusSettings.Instance.Save();
            }
        }

        /// <summary>
        /// Clears the level alarm.
        /// </summary>
        private void ClearLevelAlarm(BaseInfo item)
        {
            if (item != null)
            {
                this.Controler.Commands.SendClearLevelAlarm((LevelSensor)item);
            }
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        private void DeleteItem(BaseInfo item)
        {
            this.Controler.Items.Remove(item);
            this.Controler.ChangeUserValues();
            ReefStatusSettings.Instance.Save();
        }

        /// <summary>
        /// News the dosing.
        /// </summary>
        private void NewDosing()
        {
            var pump = (DosingPump)this.CurrentSelection;
            pump.Commands.SendUpdateDosingRate(pump, this.NewDosingRate, this.NewDosingPerDay);
        }

        /// <summary>
        /// Resets the operational hours.
        /// </summary>
        private void ResetOperationalHours(BaseInfo item)
        {
            this.Controler.Commands.SendResetOperationalHours(item);
        }

        /// <summary>
        /// The start water change.
        /// </summary>
        private void StartWaterChange(BaseInfo item)
        {
            this.Controler.Commands.SendStartWaterChange((LevelSensor)item);
        }

        /// <summary>
        /// The update dosing.
        /// </summary>
        private void UpdateDosing()
        {
            var pump = (DosingPump)this.CurrentSelection;
            this.NewDosingPerDay = pump.PerDay;
            this.NewDosingRate = pump.Rate;
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        private void UpdateItem()
        {
            this.NewUserValueDate = DateTime.Now;
            if (this.CurrentSelection.Value == null)
            {
                this.NewUserValue = 0;
            }
            else
            {
                this.NewUserValue = (double)this.CurrentSelection.Value;
            }
        }

        #endregion
    }
}