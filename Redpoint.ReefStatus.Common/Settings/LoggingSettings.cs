// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingSettings.cs" company="Redpoint Apps">
//   2010
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Settings
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using System.Xml.Serialization;

    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    using RedPoint.ReefStatus.Common.Database;
    using RedPoint.ReefStatus.Common.UI.ViewModel;

    /// <summary>
    /// The logging settings.
    /// </summary>
    public class LoggingSettings : BindableBase
    {
        /// <summary>
        /// The archive database.
        /// </summary>
        private bool archiveDatabase;

        /// <summary>
        /// The archive database duration.
        /// </summary>
        private int archiveDatabaseDuration;

        /// <summary>
        /// The archive database mode.
        /// </summary>
        private DateRangeMode archiveDatabaseMode;

        /// <summary>
        /// The database connection.
        /// </summary>
        private string databaseConnection;

        /// <summary>
        /// The last archive time.
        /// </summary>
        private DateTime lastArchiveTime;

        /// <summary>
        /// The limit database.
        /// </summary>
        private bool limitDatabase;

        /// <summary>
        /// The limit database duration.
        /// </summary>
        private int limitDatabaseDuration;

        /// <summary>
        /// The limit database mode.
        /// </summary>
        private DateRangeMode limitDatabaseMode;

        /// <summary>
        /// The log interval.
        /// </summary>
        private int logInterval;


        /// <summary>
        /// The clean up command.
        /// </summary>
        private ICommand cleanUpCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingSettings"/> class.
        /// </summary>
        public LoggingSettings()
        {
            this.LogInterval = 5;
            this.DatabaseConnection = ReefStatusSettings.AppDataDir + "\\ReefStatus\\ReefStatus.db";
            this.ArchiveDatabase = false;
            this.ArchiveDatabaseDuration = 1;
            this.ArchiveDatabaseMode = DateRangeMode.Days;

            this.LimitDatabase = false;
            this.LimitDatabaseDuration = 1;
            this.LimitDatabaseMode = DateRangeMode.Days;

            this.LastArchiveTime = DateTime.Now;
        }

        private bool allowMulitControllers;

        public bool AllowMulitControllers
        {
            get
            {
                return this.allowMulitControllers;
            }
            set
            {
                if (this.allowMulitControllers != value)
                {
                    this.allowMulitControllers = value;
                    this.OnPropertyChanged(() => this.AllowMulitControllers);
                }
            }
        }

        /// <summary>
        /// Gets or sets the last archive time.
        /// </summary>
        /// <value>The last archive time.</value>
        public DateTime LastArchiveTime
        {
            get
            {
                return this.lastArchiveTime;
            }

            set
            {
                if (this.lastArchiveTime != value)
                {
                    this.lastArchiveTime = value;
                    this.OnPropertyChanged(() => this.LastArchiveTime);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [archive database].
        /// </summary>
        /// <value><c>true</c> if [archive database]; otherwise, <c>false</c>.</value>
        public bool ArchiveDatabase
        {
            get
            {
                return this.archiveDatabase;
            }

            set
            {
                if (this.archiveDatabase != value)
                {
                    this.archiveDatabase = value;
                    this.OnPropertyChanged(() => this.ArchiveDatabase);
                }
            }
        }

        /// <summary>
        /// Gets or sets the duration of the archive database.
        /// </summary>
        /// <value>The duration of the archive database.</value>
        public int ArchiveDatabaseDuration
        {
            get
            {
                return this.archiveDatabaseDuration;
            }

            set
            {
                if (this.archiveDatabaseDuration != value)
                {
                    this.archiveDatabaseDuration = value;
                    this.OnPropertyChanged(() => this.ArchiveDatabaseDuration);
                }
            }
        }

        /// <summary>
        /// Gets or sets the archive database mode.
        /// </summary>
        /// <value>The archive database mode.</value>
        public DateRangeMode ArchiveDatabaseMode
        {
            get
            {
                return this.archiveDatabaseMode;
            }

            set
            {
                if (this.archiveDatabaseMode != value)
                {
                    this.archiveDatabaseMode = value;
                    this.OnPropertyChanged(() => this.ArchiveDatabaseMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [limit database].
        /// </summary>
        /// <value><c>true</c> if [limit database]; otherwise, <c>false</c>.</value>
        public bool LimitDatabase
        {
            get
            {
                return this.limitDatabase;
            }

            set
            {
                if (this.limitDatabase != value)
                {
                    this.limitDatabase = value;
                    this.OnPropertyChanged(() => this.LimitDatabase);
                }
            }
        }

        /// <summary>
        /// Gets or sets the duration of the limit database.
        /// </summary>
        /// <value>The duration of the limit database.</value>
        public int LimitDatabaseDuration
        {
            get
            {
                return this.limitDatabaseDuration;
            }

            set
            {
                if (this.limitDatabaseDuration != value)
                {
                    this.limitDatabaseDuration = value;
                    this.OnPropertyChanged(() => this.LimitDatabaseDuration);
                }
            }
        }

        /// <summary>
        /// Gets or sets the limit database mode.
        /// </summary>
        /// <value>The limit database mode.</value>
        public DateRangeMode LimitDatabaseMode
        {
            get
            {
                return this.limitDatabaseMode;
            }

            set
            {
                if (this.limitDatabaseMode != value)
                {
                    this.limitDatabaseMode = value;
                    this.OnPropertyChanged(() => this.LimitDatabaseMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the log interval.
        /// </summary>
        /// <value>The log interval.</value>
        public int LogInterval
        {
            get
            {
                return this.logInterval;
            }

            set
            {
                if (this.logInterval != value)
                {
                    this.logInterval = value;
                    this.OnPropertyChanged(() => this.LogInterval);
                }
            }
        }

        /// <summary>
        /// Gets the clean up command.
        /// </summary>
        /// <value>The clean up command.</value>
        [XmlIgnore]
        public ICommand CleanUpCommand
        {
            get
            {
                return this.cleanUpCommand ?? (this.cleanUpCommand = new DelegateCommand(this.CleanUp));
            }
        }

        /// <summary>
        /// Gets or sets the database connection.
        /// </summary>
        /// <value>The database connection.</value>
        public string DatabaseConnection
        {
            get
            {
                return this.databaseConnection;
            }

            set
            {
                if (this.databaseConnection != value)
                {
                    this.databaseConnection = value;
                    this.OnPropertyChanged(() => this.DatabaseConnection);
                }
            }
        }

        /// <summary>
        /// Gets the database settings.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        public DatabaseConnection Connection
        {
            get
            {
                return new DatabaseConnection(this.DatabaseConnection);
            }
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <value>The record count.</value>
        [XmlIgnore]
        public int RecordCount
        {
            get
            {
                try
                {
                    using (var database = this.Connection.Create())
                    {
                        return database != null ? database.RecordCount() : 0;
                    }
                }
                catch (ReefStatusException ex)
                {               
                    Logger.Instance.LogError(ex);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the records write.
        /// </summary>
        /// <value>The records write.</value>
        [XmlIgnore]
        public int RecordsWrite
        {
            get
            {
                try
                {
                    using (var database = this.Connection.Create())
                    {
                        return database != null ? database.WriteCount : 0;
                    }
                }
                catch (ReefStatusException ex)
                {               
                    Logger.Instance.LogError(ex);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the records delete.
        /// </summary>
        /// <value>The records delete.</value>
        [XmlIgnore]
        public int RecordsDelete
        {
            get
            {
                try
                {
                    using (var database = this.Connection.Create())
                    {
                        return database != null ? database.DeleteCount : 0;
                    }
                }
                catch (ReefStatusException ex)
                {
                    Logger.Instance.LogError(ex);
                }

                return 0;
            }
        }

        /// <summary>
        /// Cleans up the database.
        /// </summary>
        public void CleanUp()
        {
            new Thread(
                () =>
                    {
                        try
                    {
                        using (var database = this.Connection.Create())
                        {
                            if (database != null)
                            {
                                var types = database.GetTypes();
                                foreach (string type in types.Values)
                                {
                                    foreach (var Controller in ReefStatusSettings.Instance.Controllers.ToList())
                                    {
                                        database.CleanupDataSet(type, Controller.Id);
                                    }
                                }
                            }
                        }
                    }
                    catch (ReefStatusException ex)
                    {
                        Logger.Instance.LogError(ex);
                    }

                    this.OnPropertyChanged(() => this.RecordCount);
                    this.OnPropertyChanged(() => this.RecordsWrite);
                    this.OnPropertyChanged(() => this.RecordsDelete);
                }).Start();
        }
    }
}