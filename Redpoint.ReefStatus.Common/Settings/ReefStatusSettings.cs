// <copyright file="ReefStatusSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;	
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Mvvm;
    using Microsoft.Win32;
    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Date Range Mode
    /// </summary>
    public enum DateRangeMode
    {
        /// <summary>
        /// Range in Days
        /// </summary>
        Days = 0,

        /// <summary>
        /// Range in Weeks
        /// </summary>
        Weeks = 1,

        /// <summary>
        /// Range in Months
        /// </summary>
        Months = 2,

        /// <summary>
        /// Range in hours
        /// </summary>
        Hours = 3,
    }

    /// <summary>
    /// The screen saver mode
    /// </summary>
    public enum ScreenSaverMode
    {
        /// <summary>
        /// Show Parameters
        /// </summary>
        Paramaters,

        /// <summary>
        /// Show View
        /// </summary>
        View,
    }

    public enum ReportLayout
    {
        None,
        Grid,
        Tabbed
    }

    /// <summary>
    /// Application settings
    /// </summary>
    public class ReefStatusSettings : BindableBase
    {
        /// <summary>
        /// Registry Key
        /// </summary>
        public const string RegistryRootKey = "HKEY_CURRENT_USER";

        ////public static readonly string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        ////public const string RegistryRootKey = "HKEY_LOCAL_MACHINE";

        /// <summary>
        /// Application Directory
        /// </summary>
        public static readonly string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Settings file
        /// </summary>
        private const string SettingsFile = "settings.dat";

        /// <summary>
        /// Instance of the settings
        /// </summary>
        private static ReefStatusSettings instance;


        /// <summary>
        /// Auto Service Starup
        /// </summary>
        private bool autoServiceStarup;


        /// <summary>
        /// Settings File Location
        /// </summary>
        private string settingsFileLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReefStatusSettings"/> class.
        /// </summary>
        public ReefStatusSettings()
        {
            this.Controllers = new ObservableCollection<Controller>();
            this.CustomGraph = new CustomGraphSettings();
            this.Logging = new LoggingSettings();
            this.Web = new WebInterfaceSettings();
            this.SettingsFileLocation = AppDataDir + "\\ReefStatus\\" + SettingsFile;
            this.Mail = new MailSettings();
            this.Window = new WindowSettings();
            this.Layout = ReportLayout.Tabbed;
        }
    
        /// <summary>
        /// Gets the next Controller id.
        /// </summary>
        /// <returns>the next ID</returns>
        public int GetNextControllerId()
        {
            return this.Controllers.Count != 0 ? Math.Max(this.Controllers.Count, this.Controllers[this.Controllers.Count - 1].Id + 1) : 0;
        }

        private ReportLayout layout;

        /// <summary>
        /// Gets or sets a value indicating whether [tabed layout].
        /// </summary>
        /// <value><c>true</c> if [tabed layout]; otherwise, <c>false</c>.</value>
        public ReportLayout Layout
        {
            get
            {
                return this.layout;
            }

            set
            {
                if (this.layout != value)
                {
                    this.layout = value;
                    this.OnPropertyChanged(() => this.Layout);
                }
            }
        }

        private int gridColumns;

        /// <summary>
        /// Gets or sets the grid columns.
        /// </summary>
        /// <value>The grid columns.</value>
        public int GridColumns
        {
            get
            {
                return this.gridColumns;
            }

            set
            {
                if (this.gridColumns != value)
                {
                    this.gridColumns = value;
                    this.OnPropertyChanged(() => this.GridColumns);
                }
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ReefStatusSettings Instance
        {
            get
            {
                return instance ?? (instance = LoadSettings());
            }
        }

        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>The window.</value>
        public WindowSettings Window { get; set; }

        /// <summary>
        /// Gets or sets the web.
        /// </summary>
        /// <value>The web.</value>
        public WebInterfaceSettings Web { get; set; }

        /// <summary>
        /// Gets or sets the custom graph.
        /// </summary>
        /// <value>The custom graph.</value>
        public CustomGraphSettings CustomGraph { get; set; }

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        /// <value>The logging.</value>
        public LoggingSettings Logging { get; set; }

        /// <summary>
        /// Gets or sets the mail.
        /// </summary>
        /// <value>The mail.</value>
        public MailSettings Mail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto service starup].
        /// </summary>
        /// <value><c>true</c> if [auto service starup]; otherwise, <c>false</c>.</value>
        public bool AutoServiceStarup
        {
            get
            {
                return this.autoServiceStarup;
            }

            set
            {
                this.autoServiceStarup = value;
                this.OnPropertyChanged(() => this.AutoServiceStarup);
            }
        }


        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string SettingsFileLocation
        {
            get
            {
                return this.settingsFileLocation;
            }

            set
            {
                this.settingsFileLocation = value;
                this.OnPropertyChanged(() => this.SettingsFileLocation);
            }
        }

        /// <summary>
        /// Gets or sets the Controllers.
        /// </summary>
        /// <value>The Controller.</value>
        public ObservableCollection<Controller> Controllers { get; set; }

        /// <summary>
        /// Saves the settings to the specified file.
        /// </summary>
        public void Save()
        {
            try
            {
                Registry.SetValue(RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status", "SettingsFile", this.SettingsFileLocation);

                if (File.Exists(this.SettingsFileLocation))
                {
                    File.Copy(this.SettingsFileLocation, this.SettingsFileLocation + ".bak", true);
                }

                using (TextWriter writer = new StreamWriter(this.SettingsFileLocation))
                {
                    var serializer = new XmlSerializer(typeof(ReefStatusSettings), new[] { typeof(LPort), typeof(SPort), typeof(Probe), typeof(LevelSensor), typeof(UserInfo), typeof(DigitalInput), typeof(Light), typeof(DosingPump), typeof(CurrentPump) });
                    serializer.Serialize(writer, this);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(this.SettingsFileLocation + ".bak"))
                {
                    File.Copy(this.SettingsFileLocation + ".bak", this.SettingsFileLocation, true);
                    Logger.Instance.Log(new LogMessage(3000, "Unable to save settigns, restoring backup") { Exception = ex });
                }
                else
                {
                    Logger.Instance.Log(new LogMessage(3001, "Unable to save settings: " + this.SettingsFileLocation) { Exception = ex });
                }
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>
        /// The settings object that was created from the file
        /// </returns>
        private static ReefStatusSettings LoadSettings()
        {
            ReefStatusSettings settings;
            string defaultPath = AppDataDir + "\\ReefStatus\\" + SettingsFile;
            string settingsFileLocation;
            try
            {
                settingsFileLocation = Registry.GetValue(RegistryRootKey + "\\SOFTWARE\\RedPoint\\Reef Status", "SettingsFile", defaultPath) as string;
                if (string.IsNullOrEmpty(settingsFileLocation))
                {
                    settingsFileLocation = defaultPath;
                }
            }
            catch (IOException)
            {
                settingsFileLocation = defaultPath;
            }

            try
            {
                if (File.Exists(settingsFileLocation))
                {
                    using (TextReader reader = new StreamReader(settingsFileLocation))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ReefStatusSettings), new[] { typeof(LPort), typeof(SPort), typeof(Probe), typeof(LevelSensor), typeof(UserInfo), typeof(DigitalInput), typeof(Light), typeof(DosingPump), typeof(CurrentPump) });
                        settings = (ReefStatusSettings)serializer.Deserialize(reader);
                        settings.SettingsFileLocation = settingsFileLocation;
                        reader.Close();
                    }
                }
                else
                {
                    if (File.Exists(settingsFileLocation + ".bak"))
                    {
                        // try with the backup file
                        File.Copy(settingsFileLocation + ".bak", settingsFileLocation, true);
                        File.Delete(settingsFileLocation + ".bak");
                        settings = LoadSettings();
                    }
                    else
                    {
                        settings = new ReefStatusSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(settingsFileLocation + ".bak"))
                {
                    // try with the backup file
                    File.Copy(settingsFileLocation + ".bak", settingsFileLocation, true);
                    File.Delete(settingsFileLocation + ".bak");
                    settings = LoadSettings();
                }
                else
                {
                    Logger.Instance.Log(new LogMessage(3003, "Unable to load settings file using default settings") { Exception = ex });
                    settings = new ReefStatusSettings();
                }
            }

            if (settings.Controllers.Count > 1)
            {
                settings.Logging.AllowMulitControllers = true;
            }

            foreach (var Controller in settings.Controllers)
            {
                foreach (var logic in Controller.ProgrammableLogic)
                {
                    logic.Update(Controller.Items, Controller.ProgrammableLogic);
                }

                foreach (var sport in Controller.Items.OfType<SPort>())
                {
                    sport.ModeItem = sport.GetAssociatedModeItem(Controller.Items, Controller.ProgrammableLogic);
                }

                foreach (var lport in Controller.Items.OfType<LPort>())
                {
                    lport.ModeItem = lport.GetAssociatedModeItem(Controller.Items, Controller.ProgrammableLogic);
                }
            }

            return settings;
        }

        /// <summary>
        /// Gets the Controller.
        /// </summary>
        /// <param name="baseInfo">The base info.</param>
        /// <returns></returns>
        public Controller GetController(BaseInfo baseInfo)
        {
            return this.Controllers.FirstOrDefault(Controller => Controller.Items.Contains(baseInfo));
        }

        /// <summary>
        /// Gets the Controller.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Controller GetController(int id)
        {
            return this.Controllers.FirstOrDefault(Controller => Controller.Id == id);
        }

        /// <summary>
        /// Gets the Controller.
        /// </summary>
        /// <param name="reminder">The reminder.</param>
        /// <returns></returns>
        public Controller GetController(Reminder reminder)
        {
            return this.Controllers.FirstOrDefault(Controller => Controller.Info.Reminders.Contains(reminder));
        }

        /// <summary>
        /// Gets the Controller.
        /// </summary>
        /// <param name="info">The info to find.</param>
        /// <returns></returns>
        public Controller GetController(Info info)
        {
            return this.Controllers.FirstOrDefault(Controller => Controller.Info == info);
        }
    }
}
