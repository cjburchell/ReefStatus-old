// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Redpoint Apps ">
//   2010
// </copyright>
// <summary>
//   The main window view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Gui.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Threading;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Win32;

    using RedPoint.ReefStatus.Common;
    using RedPoint.ReefStatus.Common.Core;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;
    using RedPoint.ReefStatus.Common.UI;

    using MessageBox = RedPoint.ReefStatus.Common.UI.Controls.MessageBox;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

    /// <summary>
    /// The main window view model.
    /// </summary>
    public class MainWindowViewModel : BindableBase, IUpdateProgress
    {
        #region Fields

        /// <summary>
        /// The clear last error timer.
        /// </summary>
        private readonly Timer clearLastErrorTimer = new Timer();

        /// <summary>
        /// The dispatcher.
        /// </summary>
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// The add Controller command.
        /// </summary>
        private ICommand addControllerCommand;

        /// <summary>
        /// The current progress.
        /// </summary>
        private double currentProgress;

        /// <summary>
        /// The delete Controller command.
        /// </summary>
        private ICommand deleteControllerCommand;

        /// <summary>
        /// The display progress.
        /// </summary>
        private bool displayProgress;

        /// <summary>
        /// The is in alarm.
        /// </summary>
        private bool isInAlarm;

        /// <summary>
        /// The last log.
        /// </summary>
        private LogMessage lastLog;

        /// <summary>
        /// The progress steps.
        /// </summary>
        private double progressSteps;

        /// <summary>
        /// The progress text.
        /// </summary>
        private string progressText;

        /// <summary>
        /// The selected tab.
        /// </summary>
        private ControllerViewModel selectedItem;

        /// <summary>
        /// The stop command.
        /// </summary>
        private ICommand stopCommand;

        /// <summary>
        /// The update progress.
        /// </summary>
        private double updateProgress;

        private Controller selectedController;

        private DataService dataService;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="MainWindowViewModel"/> class from being created. 
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        private MainWindowViewModel()
        {
            this.Progress = new ProgressViewModel();

            this.dispatcher = Dispatcher.CurrentDispatcher;

            Logger.Instance.OnError += this.LoggerOnError;
            this.Settings = ReefStatusSettings.LoadSettings();
            this.dataService = new DataService(this.Settings);
            this.dataService.Start();

            this.WindowClosingCommand = new DelegateCommand(this.WindowClosing);

            this.UpdateSettingsCommand = new DelegateCommand(this.UpdateSettings);

            this.ImportFromFileCommand = new DelegateCommand<object>(this.ImportFromFile, arg => arg is Controller);
            this.ImportFromProfiluxCommand = new DelegateCommand<object>(
                this.ImportFromProfilux,
                arg => arg is Controller);
            this.ExportDataMenuItemCommand = new DelegateCommand<object>(this.ExportData, arg => arg is Controller);
            this.RefreshCommand = new DelegateCommand(this.UpdateAll, () => this.HasController);
            this.ShowProfiluxControlCommand = new DelegateCommand(this.ShowProfiluxControl, () => this.HasProfilux);

            this.dataService.OnLogItem += this.AddToGraphDisplay;

            this.UpdateAll();

            this.clearLastErrorTimer.Tick += this.ClearLastErrorTimerTick;
            this.clearLastErrorTimer.Interval = 20000;

            ErrorLogViewModel.Starup();

            if (this.Settings.Controllers.Count == 0)
            {
                this.AddController();
            }

            this.SelectedController = this.Settings.Controllers[0];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets AddControllerCommand.
        /// </summary>
        [XmlIgnore]
        public ICommand AddControllerCommand
        {
            get
            {
                return this.addControllerCommand ?? (this.addControllerCommand = new DelegateCommand(this.AddController));
            }
        }

        /// <summary>
        /// Gets DeleteControllerCommand.
        /// </summary>
        public ICommand DeleteControllerCommand
        {
            get
            {
                return this.deleteControllerCommand
                       ?? (this.deleteControllerCommand = new DelegateCommand<Controller>(this.DeleteController));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DisplayProgress.
        /// </summary>
        public bool DisplayProgress
        {
            get
            {
                return this.displayProgress;
            }

            set
            {
                if (this.displayProgress != value)
                {
                    this.displayProgress = value;
                    this.OnPropertyChanged(() => this.DisplayProgress);
                }
            }
        }

        /// <summary>
        /// Gets the export data menu item command.
        /// </summary>
        /// <value>The export data menu item command.</value>
        public ICommand ExportDataMenuItemCommand { get; private set; }

        /// <summary>
        /// Gets a value indicating whether HasProfilux.
        /// </summary>
        public bool HasProfilux
        {
            get
            {
                var path = this.ProfiluxPath;

                var appLocation3 = path + "\\" + "ProfiLuxControl6.exe";
                if (File.Exists(appLocation3))
                {
                    return true;
                }

                var appLocation = path + "\\" + "ProfiLuxControl II.exe";
                if (File.Exists(appLocation))
                {
                    return true;
                }

                var appLocation2 = path + "\\" + "ProfiLuxControl.exe";
                if (File.Exists(appLocation2))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the import from file command.
        /// </summary>
        /// <value>The import from file command.</value>
        public ICommand ImportFromFileCommand { get; private set; }

        /// <summary>
        /// Gets the import from profilux command.
        /// </summary>
        /// <value>The import from profilux command.</value>
        public ICommand ImportFromProfiluxCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in alarm.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is in alarm; otherwise, <c>false</c>.
        /// </value>
        public bool IsInAlarm
        {
            get
            {
                return this.isInAlarm;
            }

            set
            {
                if (this.isInAlarm != value)
                {
                    this.isInAlarm = value;
                    this.OnPropertyChanged(() => this.IsInAlarm);
                }
            }
        }

        /// <summary>
        /// Gets the last log.
        /// </summary>
        /// <value>The last log.</value>
        public int LastLogCode
        {
            get
            {
                if (this.LastLog != null)
                {
                    return this.LastLog.Code;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the last error message.
        /// </summary>
        /// <value>The last error message.</value>
        public string LastLogMessage
        {
            get
            {
                if (this.LastLog != null)
                {
                    return this.LastLog.Message;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets ProfiluxPath.
        /// </summary>
        public string ProfiluxPath
        {
            get
            {
                string path = null;

                var ghlKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\GHL\\ProfiLuxControl II");
                if (ghlKey != null)
                {
                    var keyNames = ghlKey.GetSubKeyNames();

                    if (keyNames.Length != 0)
                    {
                        try
                        {
                            var dirKey = ghlKey.OpenSubKey(keyNames[0]);
                            if (dirKey != null)
                            {
                                path = (string)dirKey.GetValue("BinDir");
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }
                }

                if (string.IsNullOrEmpty(path))
                {
                    var progfiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    if (string.IsNullOrEmpty(progfiles))
                    {
                        progfiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    }

                    try
                    {
                        foreach (var directory in Directory.GetDirectories(progfiles, "ProfiLuxControl*"))
                        {
                            if (File.Exists(directory + "\\" + "ProfiLuxControl6.exe"))
                            {
                                path = directory;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        path = string.Empty;
                    }

                    if (string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            foreach (var directory in
                                Directory.GetDirectories(progfiles, "ProfiLuxControl_*")
                                    .Where(directory => File.Exists(directory + "\\" + "ProfiLuxControl.exe")))
                            {
                                path = directory;
                            }
                        }
                        catch (ArgumentException)
                        {
                            path = string.Empty;
                        }
                    }
                }

                return path;
            }
        }

        /// <summary>
        /// Gets Progress.
        /// </summary>
        /// <value>The progress.</value>
        public ProgressViewModel Progress { get; private set; }

        /// <summary>
        /// Gets or sets the progress text.
        /// </summary>
        /// <value>The progress text.</value>
        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            set
            {
                if (this.progressText != value)
                {
                    this.progressText = value;
                    this.OnPropertyChanged(() => this.ProgressText);
                }
            }
        }

        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        public ICommand RefreshCommand { get; private set; }

        /// <summary>
        /// Gets or sets SelectedTab.
        /// </summary>
        public ControllerViewModel SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (this.selectedItem == value)
                {
                    return;
                }

                this.selectedItem = value;
                this.OnPropertyChanged(() => this.SelectedItem);
            }
        }

        /// <summary>
        /// Gets or sets SelectedTab.
        /// </summary>
        public Controller SelectedController
        {
            get
            {
                return this.selectedController;
            }

            set
            {
                if (this.selectedController == value)
                {
                    return;
                }

                this.selectedController = value;
                this.OnPropertyChanged(() => this.SelectedController);

                if (this.SelectedItem != null)
                {
                    this.SelectedItem.Remote.IsActive = false;
                    this.SelectedItem.Remote.Stop();

                    foreach (var item in this.SelectedItem.Controller.Items.Where(item => item.HasReport))
                    {
                        item.Report.Stop();
                    }
                }

                this.SelectedItem = value == null ? null : new ControllerViewModel(value);       
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public IReefStatusSettings Settings { get; }

        /// <summary>
        /// Gets the show profilux control command.
        /// </summary>
        /// <value>The show profilux control command.</value>
        public ICommand ShowProfiluxControlCommand { get; private set; }

        /// <summary>
        /// Gets the stop command.
        /// </summary>
        /// <value>The stop command.</value>
        public ICommand StopCommand
        {
            get
            {
                return this.stopCommand ?? (this.stopCommand = new DelegateCommand(this.StopRefresh));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [stop processing].
        /// </summary>
        /// <value><c>true</c> if [stop processing]; otherwise, <c>false</c>.</value>
        public bool StopProcessing { get; set; }

        /// <summary>
        /// Gets or sets the update progress.
        /// </summary>
        /// <value>The update progress.</value>
        public double UpdateProgress
        {
            get
            {
                return this.updateProgress;
            }

            set
            {
                if (this.updateProgress == value)
                {
                    return;
                }

                this.updateProgress = value;
                this.OnPropertyChanged(() => this.UpdateProgress);
            }
        }

        /// <summary>
        /// Gets the update settings command.
        /// </summary>
        public ICommand UpdateSettingsCommand { get; private set; }

        /// <summary>
        /// Gets the window closing command.
        /// </summary>
        /// <value>The window closing command.</value>
        public ICommand WindowClosingCommand { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether HasController.
        /// </summary>
        protected bool HasController
        {
            get
            {
                return this.Settings.Controllers.Count != 0;
            }
        }

        /// <summary>
        /// Gets or sets LastLog.
        /// </summary>
        private LogMessage LastLog
        {
            get
            {
                return this.lastLog;
            }

            set
            {
                if (this.lastLog != value)
                {
                    this.lastLog = value;
                    this.OnPropertyChanged(() => this.LastLogCode);
                    this.OnPropertyChanged(() => this.LastLogMessage);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Increments the progress.
        /// </summary>
        public void IncrementProgress()
        {
            this.currentProgress++;
            this.UpdateProgress = this.currentProgress / this.progressSteps * 100;
        }

        /// <summary>
        /// Sets the progress steps.
        /// </summary>
        /// <param name="steps">
        /// The steps.
        /// </param>
        public void SetProgressSteps(double steps)
        {
            this.progressSteps = steps;
            this.UpdateProgress = 0;
            this.currentProgress = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the Controller.
        /// </summary>
        private void AddController()
        {
            var Controller = new Controller { Id = this.Settings.GetNextControllerId() };
            this.Settings.Controllers.Add(Controller);
            this.SelectedController = Controller;
            this.SelectedItem.ShowConnectionOptions = true;
        }

        /// <summary>
        /// Adds to graph display.
        /// </summary>
        /// <param name="timeStamp">
        /// The time stamp.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void AddToGraphDisplay(DateTime timeStamp, Controller Controller)
        {
            if (!this.dispatcher.CheckAccess())
            {
                this.dispatcher.BeginInvoke(new Action(() => this.AddToGraphDisplay(timeStamp, Controller)));
            }
            else
            {
                this.IsInAlarm = this.Settings.Controllers.Any(item => item.Info.Alarm == CurrentState.On);

                foreach (var sensor in Controller.Items)
                {
                    if (!(sensor is UserInfo) && sensor.HasGraph)
                    {
                        sensor.Graph.UpdatePoint(timeStamp);
                    }
                }
            }
        }

        /// <summary>
        /// The clear last error timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event.
        /// </param>
        private void ClearLastErrorTimerTick(object sender, EventArgs e)
        {
            this.LastLog = null;
            this.clearLastErrorTimer.Stop();
        }

        /// <summary>
        /// The delete Controller.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void DeleteController(Controller Controller)
        {
            if (Controller != null)
            {
                if (MessageBox.Show(
                    "Are you sure you want to delete " + Controller.Name,
                    "Delete Controller?",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Settings.Controllers.Remove(Controller);
                }
            }
        }

        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void ExportData(object Controller)
        {
            var Controller1 = Controller as Controller;
            if (Controller1 != null)
            {
                var dialog = new SaveFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };

                if (dialog.ShowDialog().Value)
                {
                    ExportToFile.Start(dialog.FileName, this.Progress, Controller1.Id);
                }
            }
        }

        /// <summary>
        /// Imports from file.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void ImportFromFile(object Controller)
        {
            var Controller1 = Controller as Controller;
            if (Controller1 != null)
            {
                var dialog = new OpenFileDialog { Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*" };

                if (dialog.ShowDialog().Value)
                {
                    Common.UI.ImportFromFile.Start(dialog.FileName, this.Progress, Controller1.Id);
                }
            }
        }

        /// <summary>
        /// Imports from profilux.
        /// </summary>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void ImportFromProfilux(object Controller)
        {
            var Controller1 = Controller as Controller;
            if (Controller1 != null)
            {
                Common.UI.ImportFromProfilux.Start(this.Progress, Controller1, this.dispatcher, Controller1.Commands);
            }
        }

        /// <summary>
        /// Loggers the on error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void LoggerOnError(LogMessage message)
        {
            this.dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        this.LastLog = message;
                        this.clearLastErrorTimer.Start();
                    }));
        }

        /// <summary>
        /// Shows the profilux control.
        /// </summary>
        private void ShowProfiluxControl()
        {
            var path = this.ProfiluxPath;

            try
            {
                var appLocation3 = path + "\\" + "ProfiLuxControl6.exe";
                if (File.Exists(appLocation3))
                {
                    var app = new Process { StartInfo = { WorkingDirectory = path, FileName = appLocation3 } };
                    app.Start();
                }

                var appLocation = path + "\\" + "ProfiLuxControl II.exe";
                if (File.Exists(appLocation))
                {
                    var app = new Process { StartInfo = { WorkingDirectory = path, FileName = appLocation } };
                    app.Start();
                }

                var appLocation2 = path + "\\" + "ProfiLuxControl.exe";
                if (File.Exists(appLocation2))
                {
                    var app = new Process { StartInfo = { WorkingDirectory = path, FileName = appLocation2 } };
                    app.Start();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Stops the refresh.
        /// </summary>
        private void StopRefresh()
        {
            this.StopProcessing = true;
        }

        /// <summary>
        /// Updates all.
        /// </summary>
        private void UpdateAll()
        {
            foreach (var Controller in this.Settings.Controllers)
            {
                Controller.Commands.Progress = this;
                Controller.Commands.UpdateAll();

                foreach (var item in
                    Controller.Items.Where(item => item.SaveToDatabase).Where(item => !(item is UserInfo)))
                {
                    var item1 = item;
                    this.dispatcher.BeginInvoke(
                        new Action(
                            () =>
                            {
                                if (item1.HasDataPoints)
                                {
                                    item1.DataPoints.Refresh();
                                }

                                if (item1.HasReport)
                                {
                                    item1.Report.UpdateReport();
                                }
                            }));
                }
            }
        }

        /// <summary>
        /// The update settings.
        /// </summary>
        private void UpdateSettings()
        {
            this.dataService.UpdateSettings();
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        private void WindowClosing()
        {
            // shut down the data service and save its state
            this.StopRefresh();
            this.Settings.Save();

            this.SelectedController = null;

            this.dataService.Stop();
        }

        #endregion
    }
}