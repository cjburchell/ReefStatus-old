// <copyright file="ICommandReply.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Commands
{
    using System;
    using System.Drawing;

    using RedPoint.ReefStatus.Common.ProfiLux;

    /// <summary>
    /// Reply interface
    /// </summary>
    public interface ICommandReply
    {
        /// <summary>
        /// Updates the display text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="viewText">The view text.</param>
        /// <param name="statusDisplay">The status display.</param>
        /// <param name="Controller">The Controller.</param>
        void UpdateDisplayText(string status, string viewText, Image statusDisplay, Controller Controller);

        /// <summary>
        /// Checks the alarm.
        /// </summary>
        /// <param name="Controller">The Controller.</param>
        void UpdateStatus(Controller Controller);

        /// <summary>
        /// Logs the data.
        /// </summary>
        /// <param name="now">The now.</param>
        /// <param name="Controller">The Controller.</param>
        void LogData(DateTime now, Controller Controller, IUpdateProgress progress);

        /// <summary>
        /// Called when [error in connection].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="isError">if set to <c>true</c> [is error].</param>
        void OnErrorInConnection(Exception exception, bool isError);
    }
}
