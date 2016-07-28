// <copyright file="LPort.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            : base("strLPorts")
        {
            DefaultUnits = "%";
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get
            {
                return this.Value != null ? (double)this.Value : 0;
            }
        }

        public override double? OldDoubleValue
        {
            get
            {
                return this.OldValue != null ? (double?)((double)this.OldValue) : null;
            }
        }

        public override object ConvertedValue
        {
            get { return this.Value; }
        }


        /// <summary>
        /// Updates the mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="items">The items.</param>
        public void UpdateMode(PortMode mode, IEnumerable<BaseInfo> items)
        {
            this.DeviceMode = mode.DeviceMode;
            this.Port = mode.Port;
            this.Blackout = 0;
            this.Invert = false;
            this.ModeString = this.LPortModeString(items);
            this.ModeItem = this.GetAssociatedModeItem(items);
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
                this.Value = (double)0;
            }
            else
            {
                this.Value = Math.Round(((value - LValueMin) / (LValueMax - LValueMin)) * 100.0, 0); // LPort value is 18-255
            }
        }

        /// <summary>
        /// Converts the Mode to a string
        /// </summary>
        /// <param name="probes">The probes.</param>
        /// <returns>the string value of the the mode</returns>
        private string LPortModeString(IEnumerable<BaseInfo> items)
        {

            var deviceMode = Language.GetFrendyName(this.DeviceMode);
            if (this.IsUsePort)
            {
                return deviceMode + " " + this.Port;
            }

            if (this.IsProbe)
            {
                var probe = items.OfType<Probe>().FirstOrDefault(p => p.Index == (this.Port - 1));
                if (probe != null)
                {
                    return probe.Id;
                }

                return Language.GetResource("strSensor") + this.Port;
            }

            return deviceMode;
        }
    }
}
