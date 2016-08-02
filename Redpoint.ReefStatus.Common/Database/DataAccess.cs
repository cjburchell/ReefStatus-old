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
        public void AddLog(Controller controller, DateTime time, IUpdateProgress progress)
        {
            if (progress != null)
            {
                progress.StopProcessing = false;
                progress.DisplayProgress = true;
                progress.ProgressText = "Update Database";
                var items = new List<BaseInfo>();
                items.AddRange(controller.LevelSensors);
                items.AddRange(controller.DosingPumps);
                items.AddRange(controller.LPorts);
                items.AddRange(controller.Lights);
                items.AddRange(controller.Probes);
                items.AddRange(controller.Pumps);
                items.AddRange(controller.SPorts);
                items.AddRange(controller.DigitalInputs);
                progress.SetProgressSteps(items.Count);
            }

            try
            {
                foreach (var param in controller.LevelSensors)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }

                foreach (var param in controller.DosingPumps)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }

                foreach (var param in controller.LPorts)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }

                foreach (var param in controller.Lights)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }


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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }

                foreach (var param in controller.Pumps)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                }

                foreach (var param in controller.SPorts)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
                    this.InsertItem(param.Current, time, param.Id + "_Current", true, param.OldCurrentValue);
                }

                foreach (var param in controller.DigitalInputs)
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

                    this.InsertItem(param.Value, time, param.Id, true, param.OldValue);
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

        private void InsertItem(CurrentState value, DateTime time, string name, bool optimize, CurrentState? oldValue)
        {
            var val = value == CurrentState.On ? 1 : 0;

            double? oldVal = null;
            if (oldValue.HasValue)
            {
                oldVal = oldValue == CurrentState.On ? 1 : 0;
            }

            this.InsertItem(val, time, name, optimize, oldVal);
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="time">The time.</param>
        /// <param name="name">The name.</param>
        /// <param name="optimize">if set to <c>true</c> [optimize].</param>
        /// <param name="oldValue">the old value</param>
        public abstract void InsertItem(double value, DateTime time, string name, bool optimize, double? oldValue);
    }
}
