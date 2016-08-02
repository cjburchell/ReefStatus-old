namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Timer Settings
    /// </summary>
    public struct TimerSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether [feed pause if active].
        /// </summary>
        /// <value><c>true</c> if [feed pause if active]; otherwise, <c>false</c>.</value>
        public bool FeedPauseIfActive { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public TimerMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the day mode.
        /// </summary>
        /// <value>The day mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public DayMode DayMode { get; set; }

        /// <summary>
        /// Gets or sets the switching count.
        /// </summary>
        /// <value>The switching count.</value>
        public int SwitchingCount { get; set; }
    }
}