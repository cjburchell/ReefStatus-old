using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RedPoint.ReefStatus.Common.Database
{
    using System.Linq;

    using LoveSeat;

    using Newtonsoft.Json;

    using RedPoint.ReefStatus.Common.Settings;

    public class CouchDataAccess : DataAccess, IDataAccess
    {
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="type">the type of the value</param>
        /// <param name="ttl">time to live in days</param>
        /// <param name="oldValue">the old value</param>
        public override void InsertItem(double value, DateTime time, string type, int ttl = 0, double? oldValue = null)
        {
            try
            {
                var log = new DataLog { Time = time, Type = type, Value = value, Ttl =  ttl};

                var db = GetDatabase();

                if (oldValue.HasValue && Math.Abs(oldValue.Value - log.Value) < double.Epsilon)
                {
                    var oldPoint = this.GetRawDataPointsFromLastHour(type).Select(JsonConvert.DeserializeObject<DataLog>).OrderByDescending(item => item.Time).FirstOrDefault();
                    if (oldPoint != null)
                    {
                        oldPoint.Time = log.Time;
                        log = oldPoint;
                    }
                }

                db.SaveDocument(new Document<DataLog>(log));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(110, "Unable To insert Item into database", ex);
            }
        }

        protected override double GetLastHourAvrage(string type)
        {
            return this.GetRawDataPointsFromLastHour(type)
                .Select(JsonConvert.DeserializeObject<DataLog>)
                .Average(item => item.Value);
        }

        protected override double GetLastDayAvrage(string type)
        {
            return this.GetDataPoints(type).Average(item => item.Value);
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(string type)
        {
            var result = GetRawDataPoints(type);
            return result.Select(JsonConvert.DeserializeObject<DataLog>);
        }

        public IEnumerable<string> GetRawDataPoints(string type)
        {
            var db = GetDatabase();
            var options = new ViewOptions {IncludeDocs = true};
            options.Key.Add(type);
            var result = db.View<DataLog>("active", options, "daylog");
            return result.RawDocs;
        }

        public IEnumerable<string> GetRawDataPointsFromLastHour(string type)
        {
            var db = GetDatabase();
            var options = new ViewOptions { IncludeDocs = true };
            options.Key.Add(type);
            var result = db.View<DataLog>("last_hour", options, "daylog");
            return result.RawDocs;
        }

        private static CouchDatabase GetDatabase()
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
        public void SaveSettings(LoggingSettings settings)
        {
            this.SaveSettings<LoggingSettings>(settings);
        }

        public void SaveSettings(MailSettings settings)
        {
            this.SaveSettings<MailSettings>(settings);
        }

        public void SaveSettings(ConnectionSettings settings)
        {
            this.SaveSettings<ConnectionSettings>(settings);
        }

        private void SaveSettings<T>(T settings) where T : CouchDocument
        {
            try
            {
                var db = GetDatabase();
                var result = db.SaveDocument(new Document<T>(settings));
                var isOk = result["ok"].ToObject<bool>();
                if (isOk)
                {
                    settings.Rev = result["rev"].ToObject<string>();
                }
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
        public MailSettings LoadMailSettings()
        {
            return this.LoadSettings<MailSettings>("settings_mail");
        }

        private T LoadSettings<T>(string id) where T : CouchDocument, new()
        {
            T settings = null;
            try
            {
                var db = GetDatabase();
                settings = db.GetDocument<T>(id);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(new LogMessage(3003, "Unable to load settings file using default settings") { Exception = ex });
            }

            if (settings == null)
            {
                settings = new T { Id = id };
                SaveSettings(settings);
            }

            return settings;
        }

        public LoggingSettings LoadLoggingSettings()
        {
            return this.LoadSettings<LoggingSettings>("settings_logging");
        }

        public ConnectionSettings LoadConnectionSettings()
        {
            return this.LoadSettings<ConnectionSettings>("settings_connection");
        }
    }
}
