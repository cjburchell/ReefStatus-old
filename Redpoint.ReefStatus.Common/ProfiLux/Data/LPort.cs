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
            this.Units = this.DefaultUnits;
        }

        public double Value { get; set; }

        [JsonIgnore]
        public double? OldValue { get; set; }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(int value)
        {
            this.OldValue = this.Value;
            this.Value = value < LValueMin ? 0 : Math.Round(((value - LValueMin) / (LValueMax - LValueMin)) * 100.0, 0);
        }
    }
}
