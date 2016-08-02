namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The Port Mode
    /// </summary>
    public class PortMode
    {
        /// <summary>
        /// Device Mode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceMode DeviceMode { get; set; }

        /// <summary>
        /// Port number of the divice
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Blackout Time
        /// </summary>
        public int BlackOut { get; set; }

        /// <summary>
        /// If the port is inverted
        /// </summary>
        public bool Invert { get; set; }
    }
}