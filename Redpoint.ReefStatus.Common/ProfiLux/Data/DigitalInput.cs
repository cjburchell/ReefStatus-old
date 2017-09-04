namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The digital input.
    /// </summary>
    public class DigitalInput : SensorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalInput"/> class.
        /// </summary>
        public DigitalInput()
            : base("DigitalInput")
        {
            this.SensorType = SensorType.DigitalInput;
            this.DefaultUnits = "State";
            this.Units = this.DefaultUnits;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState Value { get; set; }

        [JsonIgnore]
        public CurrentState? OldValue { get; set; }

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        [JsonIgnore]
        public DigitalInputFunction Function { get; set; }
    }
}