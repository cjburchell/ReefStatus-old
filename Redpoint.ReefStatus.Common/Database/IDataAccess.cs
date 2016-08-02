// <copyright file="IDataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections.Generic;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    /// <summary>
    /// Data Access interface
    /// </summary>
    public interface IDataAccess : IDisposable
    {
        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueTime">The value time.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="oldValue">old value</param>
        void InsertItem(double value, DateTime valueTime, string typeName, bool optimize, double? oldValue);

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
        IEnumerable<DataLog> GetDataPoints(string type, int count, bool descending);

        /// <summary>
        /// Gets the data points.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="descending">if set to <c>true</c> [descending].</param>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// A list of datapoints
        /// </returns>
        IEnumerable<DataLog> GetDataPoints(string type, bool descending);

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
        IEnumerable<DataLog> GetDataPoints(string type, DateTime dateTime, bool descending);

        /// <summary>
        /// Adds the log.
        /// </summary>
        /// <param name="sensors">The sensors.</param>
        /// <param name="now">The now.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="callback">The callback.</param>
        void AddLog(Controller controller, DateTime now, IUpdateProgress callback);

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
        IEnumerable<DataLog> GetDataPoints(DateTime startTime, DateTime endTime, string type);
    }
}
