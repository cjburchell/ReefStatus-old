// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdoDataAccess.cs" company="Redpoint Apps">
//   2010
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// The ado data access.
    /// </summary>
    public abstract class AdoDataAccess : DataAccess
    {
        #region Properties

        /// <summary>
        /// Gets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public abstract string InsertCommand { get; }

        /// <summary>
        /// Gets or sets the conection.
        /// </summary>
        /// <value>The conection.</value>
        protected DbConnection Connection { get; set; }

        /// <summary>
        /// Gets LogIndex.
        /// </summary>
        protected virtual string LogIndex
        {
            get
            {
                return "INDEX";
            }
        }

        /// <summary>
        /// Gets SqlCountCommand.
        /// </summary>
        protected virtual string SqlCountCommand
        {
            get
            {
                return "SELECT COUNT(*) FROM LOG";
            }
        }

        /// <summary>
        /// Gets SqlDeleteItemCommand.
        /// </summary>
        protected virtual string SqlDeleteItemCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LOG.INDEX=?";
            }
        }

        /// <summary>
        /// Gets SqlDeleteItemCommand.
        /// </summary>
        protected virtual string SqlDeleteLastItemCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LOG.TYPE=? AND CONTROLLER=? ORDER BY LOG.TIME DESC LIMIT 1";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand
        {
            get
            {
                return "SELECT LOG.TIME, LOG.VALUE FROM LOG WHERE TYPE=? AND CONTROLLER=?";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand1.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand1
        {
            get
            {
                return "SELECT LOG.TIME, LOG.VALUE, LOG.INDEX FROM LOG WHERE TYPE=? AND CONTROLLER=? ORDER BY LOG.TIME ";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand2.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand2
        {
            get
            {
                return
                    "SELECT LOG.TIME, LOG.VALUE, LOG.INDEX FROM LOG WHERE TYPE=? AND LOG.TIME>? AND CONTROLLER=? ORDER BY LOG.TIME ";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand3.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand3
        {
            get
            {
                return "SELECT LOG.TIME, LOG.VALUE FROM LOG WHERE TYPE=? AND CONTROLLER=? ORDER BY LOG.TIME DESC";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand4.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand4
        {
            get
            {
                return
                    "SELECT LOG.TIME, LOG.VALUE, LOG.INDEX FROM LOG WHERE TYPE=? AND LOG.TIME>? AND LOG.TIME<=? AND CONTROLLER=? ORDER BY LOG.TIME DESC";
            }
        }

        /// <summary>
        /// Gets SqlGetDataPointsCommand5.
        /// </summary>
        protected virtual string SqlGetDataPointsCommand5
        {
            get
            {
                return "SELECT VALUE, TYPE FROM LOG WHERE LOG.TIME = ? AND CONTROLLER=? ";
            }
        }

        /// <summary>
        /// Gets SqlGetTypeIndexCommand.
        /// </summary>
        protected virtual string SqlGetTypeIndexCommand
        {
            get
            {
                return "SELECT TYPES.INDEX FROM TYPES WHERE TYPE=?";
            }
        }

        /// <summary>
        /// Gets SqlGetTypesCommand.
        /// </summary>
        protected virtual string SqlGetTypesCommand
        {
            get
            {
                return "SELECT * FROM TYPES";
            }
        }

        /// <summary>
        /// Gets SqlInsertTypeCommand.
        /// </summary>
        protected virtual string SqlInsertTypeCommand
        {
            get
            {
                return "INSERT INTO TYPES (TYPE) VALUES (?)";
            }
        }

        /// <summary>
        /// Gets SqlRemoveDataSetCommand.
        /// </summary>
        protected virtual string SqlRemoveDataSetCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LOG.TYPE=? AND CONTROLLER=?";
            }
        }

        /// <summary>
        /// Gets SqlRemoveLogsCommand.
        /// </summary>
        protected virtual string SqlRemoveLogsCommand
        {
            get
            {
                return "DELETE FROM LOG WHERE LOG.TIME<?";
            }
        }

        /// <summary>
        /// Gets SqlTimesCommand.
        /// </summary>
        protected virtual string SqlTimesCommand
        {
            get
            {
                return "SELECT DISTINCT LOG.TIME FROM LOG ORDER BY LOG.TIME DESC";
            }
        }

        /// <summary>
        /// Gets TypeIndex.
        /// </summary>
        protected virtual string TypeIndex
        {
            get
            {
                return "INDEX";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Backups the specified database connection.
        /// </summary>
        /// <param name="databaseConnection">
        /// The database connection.
        /// </param>
        /// <param name="archiveLocation">
        /// The archive location.
        /// </param>
        public abstract void Backup(string databaseConnection, string archiveLocation);

        /// <summary>
        /// Cleanups the data set.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="Controller">
        /// the contrler ID
        /// </param>
        public void CleanupDataSet(string type, int Controller)
        {
            try
            {
                Collection<DataPoint> items = this.GetDataPoints(type, false, Controller);
                for (int i = 2; i < items.Count; i++)
                {
                    DataPoint log0 = items[i - 2];
                    DataPoint log1 = items[i - 1];
                    DataPoint log2 = items[i];

                    if (log1.Value == log2.Value && log0.Value == log2.Value)
                    {
                        this.DeleteItem(log1);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(219, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (this.Connection != null)
            {
                this.Connection.Close();
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="newType">
        /// The new type.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points
        /// </returns>
        public Collection<DataLog> GetDataPoints(int type, int newType, IProgressCallback callback, int Controller)
        {
            try
            {
                var data = new Collection<DataLog>();

                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (callback.IsAborting)
                            {
                                return data;
                            }

                            var item = new DataLog
                                {
                                    Controller = 0, 
                                    Time = (DateTime)reader["TIME"], 
                                    Value = (double)reader["VALUE"], 
                                    Type = newType
                                };
                            data.Add(item);
                        }
                    }
                }

                return data;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(203, "Unable to Read Data Points", ex);
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
        /// <param name="decending">
        /// if set to <c>true</c> [decending].
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// The Data points
        /// </returns>
        public Collection<DataPoint> GetDataPoints(string graph, int count, bool decending, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                var type = this.GetTypeIndex(graph);

                var order = "ASC";
                if (decending)
                {
                    order = "DESC";
                }

                using (var command = this.CreateCommand(this.SqlGetDataPointsCommand1 + order + " LIMIT " + count))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(
                                new DataPoint(
                                    graph, 
                                    (DateTime)reader["TIME"], 
                                    (double)reader["VALUE"], 
                                    ToInt(reader[this.LogIndex])));
                        }
                    }
                }

                return data;
            }
            catch (InvalidOperationException ex)
            {
                throw new DataAccessException(213, "Unable to Read Data Points", ex);
            }
            catch (DbException ex)
            {
                throw new DataAccessException(213, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="decending">
        /// if set to <c>true</c> [decending].
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// The Data points
        /// </returns>
        public Collection<DataPoint> GetDataPoints(string graph, bool decending, int Controller)
        {
            var data = new Collection<DataPoint>();
            try
            {
                int type = this.GetTypeIndex(graph);
                string order = "ASC";
                if (decending)
                {
                    order = "DESC";
                }

                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand1 + order))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(
                                new DataPoint(
                                    graph, 
                                    (DateTime)reader["TIME"], 
                                    (double)reader["VALUE"], 
                                    ToInt(reader[this.LogIndex])));
                        }
                    }
                }

                return data;
            }
            catch (IndexOutOfRangeException)
            {
                // if there are no values we could get this execption
                return data;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(214, "Unable to Read Data Points", ex);
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
        /// <param name="decending">
        /// if set to <c>true</c> [decending].
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// The Data points
        /// </returns>
        public Collection<DataPoint> GetDataPoints(string graph, DateTime startTime, bool decending, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                int type = this.GetTypeIndex(graph);

                string order = "ASC";
                if (decending)
                {
                    order = "DESC";
                }

                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand2 + order))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(startTime), DbType.DateTime));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(
                                new DataPoint(
                                    graph, 
                                    (DateTime)reader["TIME"], 
                                    (double)reader["VALUE"], 
                                    ToInt(reader[this.LogIndex])));
                        }
                    }
                }

                return data;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(215, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="graph">
        /// The graph.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points 
        /// </returns>
        public IEnumerable GetDataPoints(string graph, int Controller)
        {
            try
            {
                var data = new Collection<DataItem>();
                int type = this.GetTypeIndex(graph);

                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand3))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new DataItem((DateTime)reader["TIME"], (double)reader["VALUE"]));
                        }
                    }
                }

                return data;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(216, "Unable to Read Data Points", ex);
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
        /// A list of Datapoints
        /// </returns>
        public Collection<DataPoint> GetDataPoints(DateTime startTime, DateTime endTime, string graph, int Controller)
        {
            try
            {
                var data = new Collection<DataPoint>();
                int type = this.GetTypeIndex(graph);

                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand4))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(startTime), DbType.DateTime));
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(endTime), DbType.DateTime));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(
                                new DataPoint(
                                    graph, 
                                    (DateTime)reader["TIME"], 
                                    (double)reader["VALUE"], 
                                    ToInt(reader[this.LogIndex])));
                        }
                    }
                }

                return data;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(217, "Unable to Read Data Points", ex);
            }
        }

        /// <summary>
        /// Gets the index of the type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// the index of the given type
        /// </returns>
        public int GetTypeIndex(string type)
        {
            try
            {
                using (var command = this.CreateCommand(this.SqlGetTypeIndexCommand))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.String));
                    var item = command.ExecuteScalar();
                    if (item == null)
                    {
                        this.InsertType(type);
                        item = command.ExecuteScalar();
                    }

                    return ToInt(item);
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(209, "Unable get the type index", ex);
            }
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns>
        /// Gets the types
        /// </returns>
        public override Dictionary<int, string> GetTypes()
        {
            try
            {
                var types = new Dictionary<int, string>();
                using (DbCommand command = this.CreateCommand(this.SqlGetTypesCommand))
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(ToInt(reader[this.TypeIndex]), (string)reader["TYPE"]);
                        }
                    }
                }

                return types;
            }
            catch (DbException ex)
            {
                throw new DataAccessException(204, "get the types from the database", ex);
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="valueTime">
        /// The value time.
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
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        public override void InsertItem(
            double value, DateTime valueTime, string type, bool optimize, int Controller, double? oldValue)
        {
            try
            {
                int typeindex = this.GetTypeIndex(type);
                if (optimize)
                {
                    if (!oldValue.HasValue)
                    {
                        // it is a new value we must add it
                        this.InsertItem(value, valueTime, typeindex, Controller);
                    }
                    else
                    {
                        if (oldValue.Value != value)
                        {
                            this.InsertItem(value, valueTime, typeindex, Controller);
                        }
                        else
                        {
                            try
                            {
                                this.DeleteLastItem(typeindex, Controller);
                            }
                            catch (DataAccessException)
                            {
                                // most likely that the object is not there just add the new item then
                            }

                            this.InsertItem(value, valueTime, typeindex, Controller);
                        }
                    }
                }
                else
                {
                    this.InsertItem(value, valueTime, typeindex, Controller);
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(212, "Unable To insert Item into database", ex);
            }
        }

        /// <summary>
        /// Gets the total Record count.
        /// </summary>
        /// <returns>
        /// the total number of records
        /// </returns>
        public override int RecordCount()
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.SqlCountCommand))
                {
                    return ToInt(command.ExecuteScalar());
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(218, "Unable to Get Count", ex);
            }
        }

        /// <summary>
        /// Removes the data point.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void RemoveDataPoint(int index)
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.SqlDeleteItemCommand))
                {
                    command.Parameters.Add(this.CreateParameter(index, DbType.Int32));
                    command.ExecuteNonQuery();
                    this.DeleteCount++;
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(224, "Unable to delete logs from database", ex);
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
                using (DbCommand command = this.CreateCommand(this.SqlRemoveDataSetCommand))
                {
                    command.Parameters.Add(this.CreateParameter(typeIndex, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    command.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(211, "Unable to delete old logs from database", ex);
            }
        }

        /// <summary>
        /// Removes the logs.
        /// </summary>
        /// <param name="timeFrom">
        /// The time from.
        /// </param>
        public void RemoveLogs(DateTime timeFrom)
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.SqlRemoveLogsCommand))
                {
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(timeFrom), DbType.DateTime));
                    command.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(210, "Unable to delete old logs from database", ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the milliseconds from the time allows Oledb to store the time.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// THe new time without the milliseconds
        /// </returns>
        protected static DateTime RemoveMilliseconds(DateTime time)
        {
            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="sql">
        /// The SQL.
        /// </param>
        /// <returns>
        /// Datatabase command
        /// </returns>
        protected abstract DbCommand CreateCommand(string sql);

        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// Database Parameter
        /// </returns>
        protected abstract DbParameter CreateParameter(object value, DbType type);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="dataTime">
        /// The data time.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <returns>
        /// a list of data points
        /// </returns>
        protected override Dictionary<int, double> GetDataPoints(DateTime dataTime, int Controller)
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.SqlGetDataPointsCommand5))
                {
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(dataTime), DbType.DateTime));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        var data = new Dictionary<int, double>();
                        while (reader.Read())
                        {
                            var type = (int)reader["TYPE"];
                            if (!data.ContainsKey(type))
                            {
                                data.Add(type, (double)reader["VALUE"]);
                            }
                        }

                        return data;
                    }
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(220, "Unable get a data point", ex);
            }
        }

        /// <summary>
        /// Gets the times.
        /// </summary>
        /// <returns>
        /// a list of times
        /// </returns>
        protected override IEnumerable<DateTime> GetTimes()
        {
            var times = new Collection<DateTime>();
            using (DbCommand command = this.CreateCommand(this.SqlTimesCommand))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        times.Add((DateTime)reader["TIME"]);
                    }
                }
            }

            return times;
        }

        /// <summary>
        /// Toes the int.
        /// </summary>
        /// <param name="var">
        /// The var.
        /// </param>
        /// <returns>
        /// The to int.
        /// </returns>
        private static int ToInt(object var)
        {
            var item = var as int?;
            if (!item.HasValue)
            {
                item = (int)(long)var;
            }

            return item.Value;
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="point">
        /// The point to delete from the database.
        /// </param>
        private void DeleteItem(DataPoint point)
        {
            this.RemoveDataPoint(point.Index);
        }

        /// <summary>
        /// The delete last item.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        /// <exception cref="DataAccessException">
        /// </exception>
        private void DeleteLastItem(int type, int Controller)
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.SqlDeleteLastItemCommand))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    command.ExecuteNonQuery();
                    this.DeleteCount++;
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(224, "Unable to delete logs from database", ex);
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
        /// <param name="Controller">
        /// The Controller.
        /// </param>
        private void InsertItem(double value, DateTime time, int type, int Controller)
        {
            try
            {
                using (DbCommand command = this.CreateCommand(this.InsertCommand))
                {
                    command.Parameters.Add(this.CreateParameter(RemoveMilliseconds(time), DbType.DateTime));
                    command.Parameters.Add(this.CreateParameter(value, DbType.Double));
                    command.Parameters.Add(this.CreateParameter(type, DbType.Int32));
                    command.Parameters.Add(this.CreateParameter(Controller, DbType.Int32));
                    command.ExecuteNonQuery();
                    this.WriteCount++;
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new DataAccessException(223, "Unable To insert Item into database", ex);
            }
            catch (DbException ex)
            {
                throw new DataAccessException(223, "Unable To insert Item into database", ex);
            }
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
                using (DbCommand command = this.CreateCommand(this.SqlInsertTypeCommand))
                {
                    command.Parameters.Add(this.CreateParameter(type, DbType.String));
                    command.ExecuteNonQuery();
                }
            }
            catch (DbException ex)
            {
                throw new DataAccessException(222, "Unable To insert type into database", ex);
            }
        }

        #endregion

        /// <summary>
        /// The data item.
        /// </summary>
        public class DataItem
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DataItem"/> class.
            /// </summary>
            /// <param name="time">The time.</param>
            /// <param name="value">The value.</param>
            public DataItem(DateTime time, double value)
            {
                this.Time = time;
                this.Value = value;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the time.
            /// </summary>
            /// <value>The time.</value>
            public DateTime Time { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            public double Value { get; set; }

            #endregion
        }
    }
}