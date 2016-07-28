namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using UI;

    internal abstract class LinqDataAccess : DataAccess, IDataAccess
    {

        /// <summary>
        /// Gets the data logs.
        /// </summary>
        /// <value>The data logs.</value>
        protected abstract IQueryable<DataLog> DataLogs { get; }

        /// <summary>
        /// Gets the data types.
        /// </summary>
        /// <value>The data types.</value>
        protected abstract IQueryable<DataType> DataTypes { get; }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controler">The controler.</param>
        /// <returns>a list of data points</returns>
        public Collection<DataPoint> GetDataPoints(string graph, bool descending, int controler)
        {
            try
            {
                Collection<DataPoint> data = new Collection<DataPoint>();
                IEnumerable<DataLog> logs;
                int type = this.GetTypeIndex(graph);
                if (descending)
                {
                    logs = from item in this.DataLogs
                           where item.Type == type
                           orderby item.Time descending
                           select item;
                }
                else
                {
                    logs = from item in this.DataLogs
                           where item.Type == type && item.Controller == controler
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
        /// <param name="graph">The graph.</param>
        /// <param name="count">The count.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controler">The controler.</param>
        /// <returns>list of data points</returns>
        public Collection<DataPoint> GetDataPoints(string graph, int count, bool descending, int controler)
        {
            try
            {
                Collection<DataPoint> data = new Collection<DataPoint>();
                IQueryable<DataLog> logs = this.GetDataPoints(this.GetTypeIndex(graph), count, descending, controler);

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
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controler">The controler.</param>
        /// <returns>A list of data points</returns>
        public IQueryable<DataLog> GetDataPoints(int type, int count, bool descending, int controler)
        {
            try
            {
                IQueryable<DataLog> logs;
                if (descending)
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == controler
                            orderby item.Time descending
                            select item).Take(count);
                }
                else
                {
                    logs = (from item in this.DataLogs
                            where item.Type == type && item.Controller == controler
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
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="contorler">The contorler.</param>
        /// <returns>List of data points</returns>
        public Collection<DataPoint> GetDataPoints(string graph, DateTime startTime, bool descending, int contorler)
        {
            try
            {
                Collection<DataPoint> data = new Collection<DataPoint>();
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
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="graph">The graph.</param>
        /// <param name="controler">The controler.</param>
        /// <returns>a list of data points for the range</returns>
        public Collection<DataPoint> GetDataPoints(DateTime startTime, DateTime endTime, string graph, int controler)
        {
            try
            {
                Collection<DataPoint> data = new Collection<DataPoint>();
                int type = this.GetTypeIndex(graph);

                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Type == type && item.Time > startTime && item.Time <= endTime && item.Controller == controler
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
        /// Gets the index of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>the index of the type</returns>
        public int GetTypeIndex(string type)
        {
            try
            {
                IEnumerable<int> typeList = from types in this.DataTypes
                                           where types.Type == type
                                           select types.Index;

                if (!typeList.Any())
                {
                    this.InsertType(type);
                    return (from types in this.DataTypes
                            where types.Type == type
                            select types.Index).Single();
                }

                return typeList.First();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(106, "Unable get the type index", ex);
            }
        }

        /// <summary>
        /// Removes the logs.
        /// </summary>
        /// <param name="timefrom">The timefrom.</param>
        public void RemoveLogs(DateTime timefrom)
        {
            try
            {
                IEnumerable<DataLog> logs = from item in this.DataLogs
                           where item.Time < timefrom
                           select item;

                this.DeleteAll(logs);
                DeleteCount++;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(107, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Removes the data set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="controler">The controler.</param>
        public void RemoveDataSet(string type, int controler)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == controler
                                            select item;

                DeleteCount += logs.Count();
                this.DeleteAll(logs);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(108, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="logs">The logs.</param>
        protected abstract void DeleteAll(IEnumerable<DataLog> logs);

        /// <summary>
        /// Cleanups the data set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="controler">the controler</param>
        public void CleanupDataSet(string type, int controler)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                List<DataLog> items = (from item in this.DataLogs
                                       where item.Type == typeIndex && item.Controller == controler
                                       orderby item.Time ascending
                                       select item).ToList();

                Collection<DataLog> toDelete = new Collection<DataLog>();

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

                DeleteCount += toDelete.Count;
                this.DeleteAll(toDelete);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(109, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="type">The type.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="controler">The controler.</param>
        public override void InsertItem(double value, DateTime time, string type, bool optimize, int controler, double? oldValue)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);

                var log = new DataLog { Time = time, Type = typeIndex, Value = value, Controller = (short)controler };

                if (optimize)
                {
                    if (!oldValue.HasValue)
                    {
                        // it is a new value we must add it
                        this.InsertItem(log);
                    }
                    else
                    {
                        if (oldValue.Value != value)
                        {
                            this.InsertItem(log);
                        }
                        else
                        {
                            var datapoints = this.GetDataPoints(typeIndex, 1, true, controler).ToList();
                            if (datapoints.Count == 1)
                            {
                                datapoints.First().Time = log.Time;
                                this.SubmitChanges();
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
        /// Submits the changes.
        /// </summary>
        protected abstract void SubmitChanges();

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="log">The log.</param>
        public void InsertItem(DataLog log)
        {
            try
            {
                WriteCount++;
                this.Insert(log);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(111, "Unable To insert Item into database", ex);
            }
        }

        /// <summary>
        /// Inserts the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        protected abstract void Insert(DataLog log);

        /// <summary>
        /// Inserts the items.
        /// </summary>
        /// <param name="log">The log.</param>
        public void InsertItems(Collection<DataLog> log)
        {
            try
            {
                WriteCount += log.Count;
                this.InserAll(log);
                
            }
            catch (Exception ex)
            {
                throw new DataAccessException(112, "Unable To insert Item into database", ex);
            }
        }

        protected abstract void InserAll(Collection<DataLog> log);

        /// <summary>
        /// Gets the number of data entries.
        /// </summary>
        /// <returns>the number of dial entreies</returns>
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
        /// Gets the types.
        /// </summary>
        /// <returns>a dictionary of types and there indexes</returns>
        public override Dictionary<int, string> GetTypes()
        {
            try
            {
                Dictionary<int, string> types = new Dictionary<int, string>();
                foreach (DataType dataType in this.DataTypes)
                {
                    types.Add(dataType.Index, dataType.Type);
                }

                return types;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(114, "get the types from the database", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="typeIndex">Index of the type.</param>
        /// <param name="newTypeIndex">New index of the type.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="controler"></param>
        /// <returns>The Data points</returns>
        public Collection<DataLog> GetDataPoints(int typeIndex, int newTypeIndex, IProgressCallback callback, int controler)
        {
            try
            {
                Collection<DataLog> data = new Collection<DataLog>();

                IEnumerable<DataLog> logs = from item in this.DataLogs
                                            where item.Type == typeIndex && item.Controller == controler
                                   select item;

                foreach (DataLog log in logs)
                {
                    if (callback.IsAborting)
                    {
                        return data;
                    }

                    DataLog dataLog = new DataLog
                        {
                            Time = log.Time, 
                            Value = log.Value, 
                            Type = newTypeIndex, 
                            Controller = (short)controler
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
        /// <param name="controler"></param>
        /// <returns>The Data points</returns>
        public IEnumerable GetDataPoints(string type, int controler)
        {
            try
            {
                int typeIndex = this.GetTypeIndex(type);
                return from item in this.DataLogs
                       where item.Type == typeIndex && item.Controller == controler
                       select new { item.Time, item.Value };
            }
            catch (Exception ex)
            {
                throw new DataAccessException(116, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="controler">The controler.</param>
        /// <returns>A list of points</returns>
        protected override Dictionary<int, double> GetDataPoints(DateTime time, int controler)
        {
            try
            {
                var logs = from item in this.DataLogs
                           where item.Time == time && item.Controller == controler
                           select item;

                Dictionary<int, double> data = new Dictionary<int, double>();
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
        /// <returns>List of Times</returns>
        protected override IEnumerable<DateTime> GetTimes()
        {
            return (from item in this.DataLogs select item.Time).Distinct().OrderBy(n => n);
        }


        /// <summary>
        /// Inserts the type.
        /// </summary>
        /// <param name="type">The type.</param>
        private void InsertType(string type)
        {
            try
            {
                DataType dataType = new DataType { Type = type };
                this.Insert(dataType);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(123, "Unable To insert type into database", ex);
            }
        }

        /// <summary>
        /// Inserts the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        protected abstract void Insert(DataType dataType);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Called when [dispose].
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        #region IDataAccess Members


        /// <summary>
        /// Backups the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="archiveLocation">The archive location.</param>
        public void Backup(string connectionString, string archiveLocation)
        {
        }

        #endregion

        #region IDataAccess Members


        public void RemoveDataPoint(int index)
        {
            try
            {
                var items = from item in this.DataLogs
                            where item.Index == index
                            select item;

                this.DeleteAll(items);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(123, "Unable To remove item from database", ex);
            }
        }

        #endregion
    }
}
