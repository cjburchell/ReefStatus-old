// <copyright file="ReefStatusSettings.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Settings
{
    using System;
    using LoveSeat;

    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    /// <summary>
    /// Application settings
    /// </summary>
    public class ReefStatusSettings : IReefStatusSettings
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = "settings";

        [JsonProperty("_rev")]
        public string Rev { get; set; }

        /// <summary>
        /// Gets or sets the web.
        /// </summary>
        /// <value>The web.</value>
        public WebInterfaceSettings Web { get; set; } = new WebInterfaceSettings();

        /// <summary>
        /// Gets or sets the logging.
        /// </summary>
        /// <value>The logging.</value>
        public LoggingSettings Logging { get; set; } = new LoggingSettings();

        /// <summary>
        /// Gets or sets the mail.
        /// </summary>
        /// <value>The mail.</value>
        public MailSettings Mail { get; set; } = new MailSettings();

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public ConnectionSettings Connection { get; set; } = new ConnectionSettings();

        private static CouchDatabase GetSettingsDatabase()
        {
            var client = new CouchClient("localhost", 5984, "admin", "admin", false, AuthenticationType.Basic);
            if (!client.HasDatabase("reefstatus"))
            {
                client.CreateDatabase("reefstatus");
            }

            return client.GetDatabase("reefstatus");
        }

        /// <summary>
        /// Saves the settings to the specified file.
        /// </summary>
        public static void SaveSettings(IReefStatusSettings settings)
        {
            try
            {
                var db = GetSettingsDatabase();
                db.SaveDocument(new Document<ReefStatusSettings>((ReefStatusSettings)settings));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>
        /// The settings object that was created from the file
        /// </returns>
        public static IReefStatusSettings LoadSettings()
        {
            IReefStatusSettings settings = null;
            try
            {
                var db = GetSettingsDatabase();
                settings = db.GetDocument<ReefStatusSettings>("settings");
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(new LogMessage(3003, "Unable to load settings file using default settings") { Exception = ex });
            }

            if (settings == null)
            {
                settings = new ReefStatusSettings();
                SaveSettings(settings);
            }

            return settings;
        }

        /// <summary>
        /// Saves the settings to the specified file.
        /// </summary>
        public static void SaveControler(Controller controller)
        {
            try
            {
                var db = GetSettingsDatabase();
                db.SaveDocument(new Document<Controller>(controller));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        public static Controller LoadControler()
        {
            Controller controller = null;
            try
            {
                var db = GetSettingsDatabase();
                controller = db.GetDocument<Controller>("controller");
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(new LogMessage(3003, "Unable to load settings file using default settings") { Exception = ex });
            }

            if (controller == null)
            {
                controller = new Controller();
                SaveControler(controller);
            }

            {
                foreach (var logic in controller.ProgrammableLogic)
                {
                    logic.Update(controller);
                }

                foreach (var sport in controller.SPorts)
                {
                    sport.ModeItem = sport.GetAssociatedModeItem(controller);
                }

                foreach (var lport in controller.LPorts)
                {
                    lport.ModeItem = lport.GetAssociatedModeItem(controller);
                }
            }

            return controller;
        }
    }
}
