// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryDataAccess.cs" company="Redpoint">
//      2010
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// The memory data access.
    /// </summary>
    public class MemoryDataAccess : DataAccess, IDataAccess
    {
        #region Constants and Fields

        /// <summary>
        /// The data logs.
        /// </summary>
        private readonly Collection<DataLog> dataLogs = new Collection<DataLog>();

        /// <summary>
        /// The data types.
        /// </summary>
        private readonly Collection<DataType> dataTypes = new Collection<DataType>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data logs.
        /// </summary>
        /// <value>The data logs.</value>
        private Collection<DataLog> DataLogs
        {
            get
            {
                return this.dataLogs;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="descending">
        /// if set to <c>true</c> [descending].
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of data points
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(string type, int count, bool descending, int controller)
        {
            try
            {
                IEnumerable<DataLog> logs;
                if (descending)
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == controller
                            orderby item.Time descending
                            select item).Take(count);
                }
                else
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == controller
                            orderby item.Time ascending
                            select item).Take(count);
                }

                return logs;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(103, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        public void InsertItem(DataLog log)
        {
            try
            {
                this.Insert(log);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(111, "Unable To insert Item into database", ex);
            }
        }

        /// <summary>
        /// Inserts the items.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        public void InsertItems(Collection<DataLog> log)
        {
            try
            {
                this.InserAll(log);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(112, "Unable To insert Item into database", ex);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDataAccess

        /// <summary>
        /// Cleanups the data set.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="controller">
        /// the Controller
        /// </param>
        public void CleanupDataSet(string type, int controller)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                List<DataLog> items = (from item in this.DataLogs
                                       where item.Type == typeIndex && item.Controller == controller
                                       orderby item.Time ascending
                                       select item).ToList();

                var toDelete = new Collection<DataLog>();

                for (int i = 2; i < items.Count; i++)
                {
                    var log0 = items[i - 2];
                    var log1 = items[i - 1];
                    var log2 = items[i];

                    if (log1.Value == log2.Value && log0.Value == log2.Value)
                    {
                        toDelete.Add(log1);
                    }
                }

                this.DeleteAll(toDelete);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(109, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// The get data points.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="descending">The descending.</param>
        /// <param name="controller">The Controller.</param>
        /// <returns> a list of data points</returns>
        /// <exception cref="DataAccessException">
        /// </exception>
        public IEnumerable<Common.ProfiLux.DataLog> GetDataPoints(string graph, bool descending, int controller)
        {
            try
            {
                IEnumerable<DataLog> logs;
                var type = this.GetTypeIndex(graph);
                if (descending)
                {
                    logs = from item in this.DataLogs where item.Type == type orderby item.Time descending select item;
                }
                else
                {
                    logs = from item in this.DataLogs
                           where item.Type == type && item.Controller == controller
                           orderby item.Time ascending
                           select item;
                }

                return logs.Select(log => new Common.ProfiLux.DataLog(graph, log.Time, log.Value));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(101, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="descending">
        /// if set to <c>true</c> [descending].
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// list of data points
        /// </returns>
        public IEnumerable<Common.ProfiLux.DataLog> GetDataPoints(string graph, int count, bool descending, int controller)
        {
            try
            {
                var logs = this.GetDataPoints(this.GetTypeIndex(graph), count, descending, controller);
                return logs.Select(log => new Common.ProfiLux.DataLog(graph, log.Time, log.Value));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(102, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="descending">
        /// if set to <c>true</c> [descending].
        /// </param>
        /// <param name="contorler">
        /// The contorler.
        /// </param>
        /// <returns>
        /// List of data points
        /// </returns>
        public IEnumerable<Common.ProfiLux.DataLog> GetDataPoints(string graph, DateTime startTime, bool descending, int contorler)
        {
            try
            {
                var type = this.GetTypeIndex(graph);
                var logs = @descending
                                       ? (from item in this.DataLogs where item.Type == type && item.Time > startTime && item.Controller == contorler orderby item.Time descending select item)
                                       : (from item in this.DataLogs where item.Type == type && item.Time > startTime && item.Controller == contorler orderby item.Time ascending select item);

                return logs.Select(log => new Common.ProfiLux.DataLog(graph, log.Time, log.Value));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(104, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points for the range
        /// </returns>
        public IEnumerable<Common.ProfiLux.DataLog> GetDataPoints(DateTime startTime, DateTime endTime, string graph, int controller)
        {
            try
            {
                var type = this.GetTypeIndex(graph);
                var logs = from item in this.DataLogs
                                            where
                                                item.Type == type && item.Time > startTime && item.Time <= endTime &&
                                                item.Controller == controller
                                            orderby item.Time descending
                                            select item;

                return logs.Select(log => new Common.ProfiLux.DataLog(graph, log.Time, log.Value));
            }
            catch (Exception ex)
            {
                throw new DataAccessException(105, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="typeIndex">Index of the type.</param>
        /// <param name="newTypeIndex">New index of the type.</param>
        /// <param name="controller">contorler index</param>
        /// <returns>The Data points</returns>
        public IEnumerable<DataLog> GetDataPoints(
            string typeIndex, string newTypeIndex, int controller)
        {
            try
            {
                var logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == controller
                                            select item;
                return logs.Select(log => new DataLog
                {
                    Time = log.Time,
                    Value = log.Value,
                    Type = newTypeIndex,
                    Controller = (short)controller
                });
            }
            catch (Exception ex)
            {
                throw new DataAccessException(115, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="controller">Controller Index</param>
        /// <returns>The Data points</returns>
        public IEnumerable GetDataPoints(string type, int controller)
        {
            try
            {
                var typeIndex = this.GetTypeIndex(type);
                return from item in this.DataLogs
                       where item.Type == typeIndex && item.Controller == controller
                       select new { item.Time, item.Value };
            }
            catch (Exception ex)
            {
                throw new DataAccessException(116, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the stats.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <param name="getStdDev">if set to <c>true</c> [get STD dev].</param>
        /// <returns>The status</returns>
        public static Stats GetStats(DateTime endTime, DateTime startTime, IEnumerable<Common.ProfiLux.DataLog> points, bool getStdDev = false)
        {
            var dataPoints = points as IList<Common.ProfiLux.DataLog> ?? points.ToList();
            return getStdDev
                       ? new Stats
                           {
                               Max = MaxDataPoint(endTime, startTime, dataPoints),
                               Min = MinDataPoint(endTime, startTime, dataPoints),
                               Average = AverageDataPoint(endTime, startTime, dataPoints),
                               StdDeviation = StdDevDataPoint(endTime, startTime, dataPoints)
                           }

                       : new Stats
                           {
                               Max = MaxDataPoint(endTime, startTime, dataPoints),
                               Min = MinDataPoint(endTime, startTime, dataPoints),
                               Average = AverageDataPoint(endTime, startTime, dataPoints)
                           };
        }

        /// <summary>
        /// Gets the index of the type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// the index of the type
        /// </returns>
        public string GetTypeIndex(string type)
        {
            try
            {
                var typeList = from types in this.DataTypes where types.Type == type select types.Index;
                var enumerable = typeList as IList<string> ?? typeList.ToList();
                if (enumerable.Count == 0)
                {
                    this.InsertType(type);
                    return (from types in this.DataTypes where types.Type == type select types.Index).Single();
                }

                return enumerable.First();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(106, "Unable get the type index", ex);
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns>
        /// a dictionary of types and there indexes
        /// </returns>
        public override Dictionary<int, string> GetTypes()
        {
            try
            {
                return this.DataTypes.ToDictionary(dataType => dataType.Index, dataType => dataType.Type);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(114, "get the types from the database", ex);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="optimize">
        /// if set to <c>true</c> [optimize].
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <param name="oldValue">old value</param>
        public override void InsertItem(double value, DateTime time, string type, bool optimize, int controller, double? oldValue)
        {
            try
            {
                var typeIndex = this.GetTypeIndex(type);

                var log = new DataLog { Time = time, Type = typeIndex, Value = value, Controller = controller };

                if (optimize)
                {

                    if (!oldValue.HasValue)
                    {
                        // it is a new value we must add it
                        this.InsertItem(log);
                    }
                    else
                    {
                        if (oldValue.Value != log.Value)
                        {
                            this.InsertItem(log);
                        }
                        else
                        {
                            List<DataLog> datapoints = this.GetDataPoints(typeIndex, 1, true, controller).ToList();
                            if (datapoints.Count == 1)
                            {
                                datapoints.First().Time = log.Time;
                            }
                            else
                            {
                                this.InsertItem(log);
                            }
                        }
                    }
                }
                else
                {
                    this.InsertItem(log);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(110, "Unable To insert Item into database", ex);
            }
        }

        /// <summary>
        /// Gets the number of data entries.
        /// </summary>
        /// <returns>
        /// the number of dial entreies
        /// </returns>
        public override int RecordCount()
        {
            try
            {
                return this.DataLogs.Count();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(113, "Unable get the type index", ex);
            }
        }

        /// <summary>
        /// Removes the data set.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        public void RemoveDataSet(string type, int controller)
        {
            try
            {
                var typeIndex = this.GetTypeIndex(type);
                var logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == controller
                                            select item;

                this.DeleteAll(logs);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(108, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Removes the logs.
        /// </summary>
        /// <param name="timefrom">
        /// The timefrom.
        /// </param>
        public void RemoveLogs(DateTime timefrom)
        {
            try
            {
                var logs = from item in this.DataLogs where item.Time < timefrom select item;
                this.DeleteAll(logs);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(107, "Unable to delete old logs from database", ex);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.OnDispose();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of points
        /// </returns>
        protected override Dictionary<string, double> GetDataPoints(DateTime time, int controller)
        {
            try
            {
                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Time == time && item.Controller == controller
                                            select item;

                var data = new Dictionary<string, double>();
                foreach (var item in logs)
                {
                    if (!data.ContainsKey(item.Type))
                    {
                        data.Add(item.Type, item.Value);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(117, "Unable get a data point", ex);
            }
        }

        /// <summary>
        /// Gets the times.
        /// </summary>
        /// <returns>
        /// List of Times
        /// </returns>
        protected override IEnumerable<DateTime> GetTimes()
        {
            return (from item in this.DataLogs select item.Time).Distinct().OrderBy(n => n);
        }

        /// <summary>
        /// Called when [dispose].
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Averages the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns>the average of a data point</returns>
        private static double AverageDataPoint(DateTime endTime, DateTime startTime, IEnumerable<Common.ProfiLux.DataLog> points)
        {
            try
            {
                var average = (from item in points
                        where item.Time <= endTime && item.Time > startTime
                        select (double?)item.Value).Average();

                return average ?? 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(119, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="logs">
        /// The logs.
        /// </param>
        private void DeleteAll(IEnumerable<DataLog> logs)
        {
            var items = new Collection<DataLog>();
            foreach (DataLog dataLog in logs)
            {
                items.Add(dataLog);
            }

            this.RemoveAll(items);
        }

        private int lastIndex;

        /// <summary>
        /// Insers all.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        private void InserAll(IEnumerable<DataLog> log)
        {
            foreach (var dataLog in log)
            {
                this.Insert(dataLog);
            }
        }

        /// <summary>
        /// Inserts the specified log.
        /// </summary>
        /// <param name="log">
        /// The log.
        /// </param>
        private void Insert(DataLog log)
        {
            this.DataLogs.Add(log);
        }

        /// <summary>
        /// Inserts the specified data type.
        /// </summary>
        /// <param name="dataType">
        /// Type of the data.
        /// </param>
        private void Insert(DataType dataType)
        {
            this.DataTypes.Add(dataType);
        }

        /// <summary>
        /// Inserts the type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        private void InsertType(string type)
        {
            try
            {
                this.Insert(new DataType { Type = type });
            }
            catch (Exception ex)
            {
                throw new DataAccessException(123, "Unable To insert type into database", ex);
            }
        }


        /// <summary>
        /// Maxes the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double MaxDataPoint(DateTime endTime, DateTime startTime, IEnumerable<Common.ProfiLux.DataLog> points)
        {
            try
            {
                var max = (from item in points
                                            where
                                                item.Time <= endTime && item.Time > startTime
                           select (double?)item.Value).Max();
                return max ?? 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(121, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Mins the data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double MinDataPoint(DateTime endTime, DateTime startTime, IEnumerable<Common.ProfiLux.DataLog> points)
        {
            try
            {
                var min = (from item in points
                           where item.Time <= endTime && item.Time > startTime
                           select (double?)item.Value).Min();

                return min.HasValue? min.Value : 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(120, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the min date.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        public static DateTime GetMinDate(IEnumerable<Common.ProfiLux.DataLog> points)
        {
            try
            {
                return (from item in points
                        select item.Time).Min();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(120, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        private void RemoveAll(IList<DataLog> items)
        {
            if (items.Count != 0)
            {
                this.DataLogs.Remove(items[0]);
                items.RemoveAt(0);
                this.RemoveAll(items);
            }
        }

        /// <summary>
        /// STDs the dev data point.
        /// </summary>
        /// <param name="endTime">The end time.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="points">The points.</param>
        /// <returns></returns>
        private static double StdDevDataPoint(DateTime endTime, DateTime startTime, IEnumerable<Common.ProfiLux.DataLog> points)
        {
            try
            {
                return CalculateStdDev(
                    from item in points
                    where item.Time <= endTime && item.Time > startTime
                    select item.Value);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(118, "Unable to Read Data Points", ex);
            }
        }

        #endregion

        #region IDataAccess Members

        /// <summary>
        /// Removes the data point.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveDataPoint(string id)
        {
            var data = this.DataLogs.FirstOrDefault(item => item.Id == id);
            this.DataLogs.Remove(data);
        }

        #endregion
    }
}