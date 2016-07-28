// <copyright file="IDataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.UI;
 
    /// <summary>
    /// Data Access interface
    /// </summary>
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// Gets the write count.
        /// </summary>
        /// <value>The write count.</value>
        int WriteCount { get; }

        /// <summary>
        /// Gets the delete count.
        /// </summary>
        /// <value>The delete count.</value>
        int DeleteCount { get; }

        /// <summary>
        /// Gets the index of the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>the index of the type</returns>
        int GetTypeIndex(string typeName);

        /// <summary>
        /// Removes the logs.
        /// </summary>
        /// <param name="timeFrom">The time from.</param>
        void RemoveLogs(DateTime timeFrom);

        /// <summary>
        /// Removes the data set.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="Controller">The Controller.</param>
        void RemoveDataSet(string typeName, int Controller);

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueTime">The value time.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="Controller">The Controller.</param>
        void InsertItem(double value, DateTime valueTime, string typeName, bool optimize, int Controller, double? oldValue);

        /// <summary>
        /// Import the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        void Import(string fileName, IProgressCallback callback, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        Collection<DataPoint> GetDataPoints(string type, int count, bool descending, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        Collection<DataPoint> GetDataPoints(string type, bool descending, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        Collection<DataPoint> GetDataPoints(string type, DateTime dateTime, bool descending, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="typeIndex">Index of the type.</param>
        /// <param name="newTypeIndex">New index of the type.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        Collection<DataLog> GetDataPoints(int typeIndex, int newTypeIndex, IProgressCallback callback, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        IEnumerable GetDataPoints(string type, int Controller);

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <returns>A list of types</returns>
        Dictionary<int, string> GetTypes();

        /// <summary>
        /// Exports the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        void Export(string fileName, IProgressCallback callback, int Controller);

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="Controller">The Controller.</param>
        void AddLog(Collection<ItemDataRow> data, IProgressCallback callback, int Controller);

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="sensors">The sensors.</param>
        /// <param name="now">The now.</param>
        /// <param name="Controller">The Controller.</param>
        /// <param name="callback">The callback.</param>
        void AddLog(IList sensors, DateTime now, int Controller, IUpdateProgress callback);

        /// <summary>
        /// Gets the total Record count.
        /// </summary>
        /// <returns>the total number of records</returns>
        int RecordCount();

        /// <summary>
        /// Cleanups the data set.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="Controller">The Controller.</param>
        void CleanupDataSet(string type, int Controller);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="type">The type.</param>
        /// <param name="Controller">The Controller.</param>
        /// <returns>
        /// a list of data points for the range
        /// </returns>
        Collection<DataPoint> GetDataPoints(DateTime startTime, DateTime endTime, string type, int Controller);

        /// <summary>
        /// Backups the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="archiveLocation">The archive location.</param>
        void Backup(string connectionString, string archiveLocation);

        /// <summary>
        /// Removes the data point.
        /// </summary>
        /// <param name="index">The index.</param>
        void RemoveDataPoint(int index);
    }
}
