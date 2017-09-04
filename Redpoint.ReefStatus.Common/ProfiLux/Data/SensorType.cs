namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    /// <summary>
    /// Type of sensor
    /// </summary>
    public enum SensorType
    {
        /// <summary>
        /// No Sensor
        /// </summary>
        None = 0,

        /// <summary>
        /// Temperature sensor
        /// </summary>
        Temperature = 1,

        /// <summary>
        /// PH sensor
        /// </summary>
        PH = 2,

        /// <summary>
        /// Redox sensor
        /// </summary>
        Redox = 3,

        /// <summary>
        /// Freshwater Conductivity sensor
        /// </summary>
        ConductivityF = 4,

        /// <summary>
        /// Saltwater Conductivity sensor
        /// </summary>
        Conductivity = 5,

        /// <summary>
        /// Free sensor
        /// </summary>
        Free = 6,

        /// <summary>
        /// Humidity sensor
        /// </summary>
        Humidity = 7,

        /// <summary>
        /// Air Temperature sensor
        /// </summary>
        AirTemperature = 8,

        /// <summary>
        /// Oxygen sensor
        /// </summary>
        Oxygen = 9,

        /// <summary>
        /// Voltage Sensor
        /// </summary>
        Voltage = 10,

        /// <summary>
        /// Level sensor
        /// </summary>
        Level = 30,

        DigitalInput = 31
    }
}