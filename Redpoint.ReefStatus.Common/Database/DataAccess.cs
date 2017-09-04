// <copyright file="DataAccess.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.Database
{
    using System;
    using System.Collections.Generic;
    using ProfiLux;
    using RedPoint.ReefStatus.Common.ProfiLux.Data;

    /// <summary>
    /// Data Access comon methods
    /// </summary>
    public abstract class DataAccess
    {
        /// <summary>
        /// Gets or sets the write count.
        /// </summary>
        /// <value>The write count.</value>
        public int WriteCount { get; protected set; }

        /// <summary>
        /// Gets or sets the delete count.
        /// </summary>
        /// <value>The delete count.</value>
        public int DeleteCount { get; protected set; }

        /// <summary>
        /// Logs to file.
        /// </summary>
        /// <param name="controller">
        /// The items.
        /// </param>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void AddLog(IController controller, DateTime time, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Adding Logs to Database";
                var items = new List<BaseInfo>();
                items.AddRange(controller.Probes);
                progress.SetProgressSteps(items.Count);
            }

            try
            {
                foreach (var param in controller.Probes)
                {
                    if (progress != null)
                    {
                        progress.IncrementProgress();
                        progress.ProgressText = "Saving " + param.DisplayName;
                        if (progress.StopProcessing)
                        {
                            return;
                        }
                    }

                    this.InsertItem(param.Value, time, param.Id, param.OldValue);
                }
            }
            finally
            {
                if (progress != null)
                {
                    progress.DisplayProgress = false;
                }
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="type">The name.</param>
        /// <param name="oldValue">the old value</param>
        public abstract void InsertItem(double value, DateTime time, string type, double? oldValue = null);
    }
}
