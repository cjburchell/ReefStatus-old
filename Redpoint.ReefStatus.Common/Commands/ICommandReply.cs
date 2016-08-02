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
        /// Called when [error in connection].
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="isError">if set to <c>true</c> [is error].</param>
        void OnErrorInConnection(Exception exception, bool isError);
    }
}
