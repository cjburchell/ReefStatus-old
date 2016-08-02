// <copyright file="SPort.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Socket Status
    /// </summary>
    public class SPort : DeviceInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SPort" /> class.
        /// </summary>
        public SPort()
            : base("SPort")
        {
        }

        /// <summary>
        ///     Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState Value { get; set; }

        /// <summary>
        ///     Gets the old double value.
        /// </summary>
        [JsonIgnore]
        public CurrentState? OldValue { get; set; }

        /// <summary>
        ///     Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int CurrentColourValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive => this.Value == CurrentState.On;

        /// <summary>
        ///     Gets or sets the current.
        /// </summary>
        /// <value>
        ///     The current.
        /// </value>
        public double Current { get; set; }

        /// <summary>
        ///     Gets or sets the old current value.
        /// </summary>
        /// <value>
        ///     The old current value.
        /// </value>
        public double? OldCurrentValue { get; set; }

        /// <summary>
        ///     Updates the mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="items">The items.</param>
        /// <param name="logics">The logics.</param>
        public void UpdateMode(PortMode mode, Controller controller)
        {
            this.DeviceMode = mode.DeviceMode;
            this.Port = mode.Port;
            this.Blackout = mode.BlackOut;
            this.Invert = mode.Invert;
            this.DefaultUnits = "State";
            this.ModeItem = this.GetAssociatedModeItem(controller);
        }

        /// <summary>
        ///     Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value)
        {
            this.OldValue = this.Value;
            this.Value = (CurrentState)value;
        }
    }
}