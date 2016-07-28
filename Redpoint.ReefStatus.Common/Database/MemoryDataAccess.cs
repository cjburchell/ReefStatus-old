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
    using RedPoint.ReefStatus.Common.UI;

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

        /// <summary>
        /// Gets the data types.
        /// </summary>
        /// <value>The data types.</value>
        private Collection<DataType> DataTypes
        {
            get
            {
                return this.dataTypes;
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of data points
        /// </returns>
        public IEnumerable<DataLog> GetDataPoints(int type, int count, bool descending, int Controller)
        {
            try
            {
                IEnumerable<DataLog> logs;
                if (descending)
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == Controller
                            orderby item.Time descending
                            select item).Take(count);
                }
                else
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == Controller
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
                this.WriteCount++;
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
                this.WriteCount += log.Count;
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
        /// Backups the specified connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="archiveLocation">
        /// The archive location.
        /// </param>
        public void Backup(string connectionString, string archiveLocation)
        {
        }

        /// <summary>
        /// Cleanups the data set.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="Controller">
        /// the Controller
        /// </param>
        public void CleanupDataSet(string type, int Controller)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                List<DataLog> items = (from item in this.DataLogs
                                       where item.Type == typeIndex && item.Controller == Controller
                                       orderby item.Time ascending
                                       select item).ToList();

                var toDelete = new Collection<DataLog>();

                for (int i = 2; i < items.Count; i++)
                {
                    DataLog log0 = items[i - 2];
                    DataLog log1 = items[i - 1];
                    DataLog log2 = items[i];

                    if (log1.Value == log2.Value && log0.Value == log2.Value)
                    {
                        toDelete.Add(log1);
                    }
                }

                this.DeleteCount += toDelete.Count;
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
        /// <param name="Controller">The Controller.</param>
        /// <returns> a list of data points</returns>
        /// <exception cref="DataAccessException">
        /// </exception>
        public Collection<DataPoint> GetDataPoints(string graph, bool descending, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                IEnumerable<DataLog> logs;
                int type = this.GetTypeIndex(graph);
                if (descending)
                {
                    logs = from item in this.DataLogs where item.Type == type orderby item.Time descending select item;
                }
                else
                {
                    logs = from item in this.DataLogs
                           where item.Type == type && item.Controller == Controller
                           orderby item.Time ascending
                           select item;
                }

                foreach (DataLog log in logs)
                {
                    data.Add(new DataPoint(graph, log.Time, log.Value, log.Index));
                }

                return data;
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// list of data points
        /// </returns>
        public Collection<DataPoint> GetDataPoints(string graph, int count, bool descending, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                IEnumerable<DataLog> logs = this.GetDataPoints(this.GetTypeIndex(graph), count, descending, Controller);

                foreach (DataLog log in logs)
                {
                    data.Add(new DataPoint(graph, log.Time, log.Value, log.Index));
                }

                return data;
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
        public Collection<DataPoint> GetDataPoints(string graph, DateTime startTime, bool descending, int contorler)
        {
            try
            {
                var data = new Collection<DataPoint>();
                IEnumerable<DataLog> logs;
                int type = this.GetTypeIndex(graph);
                if (descending)
                {
                    logs = from item in this.DataLogs
                           where item.Type == type && item.Time > startTime && item.Controller == contorler
                           orderby item.Time descending
                           select item;
                }
                else
                {
                    logs = from item in this.DataLogs
                           where item.Type == type && item.Time > startTime && item.Controller == contorler
                           orderby item.Time ascending
                           select item;
                }

                foreach (DataLog log in logs)
                {
                    data.Add(new DataPoint(graph, log.Time, log.Value, log.Index));
                }

                return data;
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points for the range
        /// </returns>
        public Collection<DataPoint> GetDataPoints(DateTime startTime, DateTime endTime, string graph, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                int type = this.GetTypeIndex(graph);

                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where
                                                item.Type == type && item.Time > startTime && item.Time <= endTime &&
                                                item.Controller == Controller
                                            orderby item.Time descending
                                            select item;

                foreach (DataLog log in logs)
                {
                    data.Add(new DataPoint(graph, log.Time, log.Value, log.Index));
                }

                return data;
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
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">contorler index</param>
        /// <returns>The Data points</returns>
        public Collection<DataLog> GetDataPoints(
            int typeIndex, int newTypeIndex, IProgressCallback callback, int Controller)
        {
            try
            {
                var data = new Collection<DataLog>();

                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == Controller
                                            select item;

                foreach (DataLog log in logs)
                {
                    if (callback.IsAborting)
                    {
                        return data;
                    }

                    var dataLog = new DataLog
                        {
                           Time = log.Time, Value = log.Value, Type = newTypeIndex, Controller = (short)Controller 
                        };
                    data.Add(dataLog);
                }

                return data;
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
        /// <param name="Controller">Controller Index</param>
        /// <returns>The Data points</returns>
        public IEnumerable GetDataPoints(string type, int Controller)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                return from item in this.DataLogs
                       where item.Type == typeIndex && item.Controller == Controller
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
        public static Stats GetStats(DateTime endTime, DateTime startTime, Collection<DataPoint> points, bool getStdDev = false)
        {
            return getStdDev
                       ? new Stats
                           {
                               Max = MaxDataPoint(endTime, startTime, points),
                               Min = MinDataPoint(endTime, startTime, points),
                               Average = AverageDataPoint(endTime, startTime, points),
                               StdDeviation = StdDevDataPoint(endTime, startTime, points)
                           }

                       : new Stats
                           {
                               Max = MaxDataPoint(endTime, startTime, points),
                               Min = MinDataPoint(endTime, startTime, points),
                               Average = AverageDataPoint(endTime, startTime, points)
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
        public int GetTypeIndex(string type)
        {
            try
            {
                IEnumerable<int> typeList = from types in this.DataTypes where types.Type == type select types.Index;

                if (typeList.Count() == 0)
                {
                    this.InsertType(type);
                    return (from types in this.DataTypes where types.Type == type select types.Index).Single();
                }

                return typeList.First();
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public override void InsertItem(double value, DateTime time, string type, bool optimize, int Controller, double? oldValue)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);

                var log = new DataLog { Time = time, Type = typeIndex, Value = value, Controller = (short)Controller };

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
                            List<DataLog> datapoints = this.GetDataPoints(typeIndex, 1, true, Controller).ToList();
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        public void RemoveDataSet(string type, int Controller)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == Controller
                                            select item;

                this.DeleteCount += logs.Count();
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
                IEnumerable<DataLog> logs = from item in this.DataLogs where item.Time < timefrom select item;

                this.DeleteAll(logs);
                this.DeleteCount++;
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// A list of points
        /// </returns>
        protected override Dictionary<int, double> GetDataPoints(DateTime time, int Controller)
        {
            try
            {
                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Time == time && item.Controller == Controller
                                            select item;

                var data = new Dictionary<int, double>();
                foreach (DataLog item in logs)
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
        private static double AverageDataPoint(DateTime endTime, DateTime startTime, Collection<DataPoint> points)
        {
            try
            {
                var average = (from item in points
                        where item.Time <= endTime && item.Time > startTime
                        select (double?)item.Value).Average();

                return average.HasValue ? average.Value : 0;
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
            log.Index = this.lastIndex;
            this.lastIndex++;
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
        private static double MaxDataPoint(DateTime endTime, DateTime startTime, Collection<DataPoint> points)
        {
            try
            {
                var max = (from item in points
                                            where
                                                item.Time <= endTime && item.Time > startTime
                           select (double?)item.Value).Max();
                return max.HasValue ? max.Value : 0;
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
        private static double MinDataPoint(DateTime endTime, DateTime startTime, Collection<DataPoint> points)
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
        public static DateTime GetMinDate(Collection<DataPoint> points)
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
        private void RemoveAll(Collection<DataLog> items)
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
        private static double StdDevDataPoint(DateTime endTime, DateTime startTime, Collection<DataPoint> points)
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
        public void RemoveDataPoint(int index)
        {
            var data = this.DataLogs.FirstOrDefault(item => item.Index == index);
            this.DataLogs.Remove(data);
        }

        #endregion
    }
}