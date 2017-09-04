// <copyright file="IDataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections.Generic;

    using RedPoint.ReefStatus.Common.ProfiLux;
    using RedPoint.ReefStatus.Common.Settings;

    /// <summary>
    /// Data Access interface
    /// </summary>
    public interface IDataAccess
    {
        void AddLog(IController controller, DateTime now, IUpdateProgress callback);

        void InsertItem(double value, DateTime time, string type, double? oldValue = null);

        IEnumerable<DataLog> GetDataPoints(string type, int? count = null, bool? descending = null);

        IEnumerable<string> GetRawDataPoints(string type, int? count = null, bool? descending = null);

        MailSettings LoadMailSettings();
        ConnectionSettings LoadConnectionSettings();
        LoggingSettings LoadLoggingSettings();

        void SaveSettings(MailSettings settings);
        void SaveSettings(ConnectionSettings settings);
        void SaveSettings(LoggingSettings settings);
    }
}
