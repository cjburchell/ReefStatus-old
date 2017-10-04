// <copyright file="LevelSensor.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Level Sensor
    /// </summary>
    public class LevelSensor : SensorInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LevelSensor" /> class.
        /// </summary>
        public LevelSensor()
            : base("LevelSensor")
        {
            this.SensorType = SensorType.Level;
            this.DefaultUnits = "State";
            this.Units = this.DefaultUnits;
        }

        /// <summary>
        ///     Gets or sets the opertation mode.
        /// </summary>
        /// <value>The opertation mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public LevelSensorOpertationMode OpertationMode { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState Value { get; set; }

        [JsonIgnore]
        public CurrentState? OldValue { get; set; }

        /// <summary>
        ///     Gets or sets the water mode.
        /// </summary>
        /// <value>
        ///     The water mode.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public WaterMode WaterMode { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState SecondSensor { get; set; }

        public bool HasTwoInputs => OpertationMode == LevelSensorOpertationMode.AutoTopOffWith2Sensors ||
                                    OpertationMode == LevelSensorOpertationMode.WaterChangeAndAutoTopOff ||
                                    OpertationMode == LevelSensorOpertationMode.WaterChange ||
                                    OpertationMode == LevelSensorOpertationMode.MinMaxControl;

        public bool HasWaterChange => OpertationMode == LevelSensorOpertationMode.WaterChangeAndAutoTopOff ||
                                      OpertationMode == LevelSensorOpertationMode.WaterChange;
    }
}