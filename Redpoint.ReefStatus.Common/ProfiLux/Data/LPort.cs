// <copyright file="LPort.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// L Port status
    /// </summary>
    public class LPort : DeviceInfo
    {
        private const double LValueMin = 18.0;
        private const double LValueMax = 255.00;

        /// <summary>
        /// Initializes a new instance of the <see cref="LPort"/> class.
        /// </summary>
        public LPort()
            : base("LPort")
        {
            this.DefaultUnits = "%";
        }

        public double Value { get; set; }

        [JsonIgnore]
        public double? OldValue { get; set; }

        /// <summary>
        /// Updates the mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="controller">The controller.</param>
        public void UpdateMode(PortMode mode, Controller controller)
        {
            this.DeviceMode = mode.DeviceMode;
            this.Port = mode.Port;
            this.Blackout = 0;
            this.Invert = false;
            this.ModeItem = this.GetAssociatedModeItem(controller);
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(int value)
        {
            this.OldValue = this.Value;
            if (value < LValueMin)
            {
                this.Value = 0;
            }
            else
            {
                this.Value = Math.Round(((value - LValueMin) / (LValueMax - LValueMin)) * 100.0, 0); // LPort value is 18-255
            }
        }
    }
}
