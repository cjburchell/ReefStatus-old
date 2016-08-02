using System;
using System.Collections.Generic;

namespace RedPoint.ReefStatus.Common.Database
{
    using System.Linq;

    using LoveSeat;

    using RedPoint.ReefStatus.Common.ProfiLux;

    public class CouchDataAccess : DataAccess, IDataAccess
    {
        private static CouchDatabase GetLoggingDatabase()
        {
            var client = new CouchClient("localhost", 5984, "admin", "admin", false, AuthenticationType.Basic);
            if (!client.HasDatabase("reefstatus_log"))
            {
                client.CreateDatabase("reefstatus_log");
            }

            return client.GetDatabase("reefstatus_log");
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="name">The name.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="Controller">The Controller.</param>
        public override void InsertItem(double value, DateTime time, string type, bool optimize, double? oldValue)
        {
            try
            {
                var log = new DataLog { Time = time, Type = type, Value = value };

                var db = GetLoggingDatabase();

                if (optimize)
                {
                    if (oldValue.HasValue && oldValue.Value == log.Value)
                    {
                        var oldPoint = this.GetDataPoints(type, 1, true).FirstOrDefault();
                        if (oldPoint != null)
                        {
                            oldPoint.Time = log.Time;
                            log = oldPoint;
                        }
                    }
                }

                db.SaveDocument(new Document<DataLog>(log));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(110, "Unable To insert Item into database", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(string type, int count, bool @descending)
        {
            return new List<DataLog>();
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(string type, bool @descending)
        {
            return new List<DataLog>();
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(string type, DateTime dateTime, bool @descending)
        {
            return new List<DataLog>();
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="type">The type.</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// a list of data points for the range
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(DateTime startTime, DateTime endTime, string type)
        {
            return new List<DataLog>();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
        }
    }
}
