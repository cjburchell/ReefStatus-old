// <copyright file="SensorInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Setnsor status
    /// </summary>
    public abstract class SensorInfo : BaseInfo
    {
        /// <summary>
        /// the format of the sensor data
        /// </summary>
        private int format;

        /// <summary>
        /// Sensor Type
        /// </summary>
        private SensorType sensorType;

        protected SensorInfo(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>The format.</value>
        public int Format
        {
            get
            {
                return this.format;
            }

            set
            {
                this.format = value;
                this.UpdateUnits();
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorType SensorType
        {
            get
            {
                return this.sensorType;
            }

            set
            {
                this.sensorType = value;
                this.UpdateUnits();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is alarm on.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is alarm on; otherwise, <c>false</c>.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrentState AlarmState { get; set; }

        /// <summary>
        /// Updates the units.
        /// </summary>
        private void UpdateUnits()
        {
            switch (this.SensorType)
            {
                case SensorType.Level:
                    this.DefaultUnits = "State";
                    break;
                case SensorType.PH:
                    this.DefaultUnits = "PH";
                    break;
                case SensorType.AirTemperature:
                case SensorType.Temperature:
                    {
                        this.DefaultUnits = this.Format == 1 ? "°F" : "°C";
                    }

                    break;
                case SensorType.ConductivityF:
                    this.DefaultUnits = "μS";
                    break;
                case SensorType.Conductivity:
                    if (this.Format == 1)
                    {
                        this.DefaultUnits = "ppt/PSU";
                    }
                    else if (this.Format == 2)
                    {
                        this.DefaultUnits = "SG";
                    }
                    else
                    {
                        this.DefaultUnits = "mS";
                    }

                    break;
                case SensorType.Redox:
                    this.DefaultUnits = "mV";
                    break;
                case SensorType.Oxygen:
                case SensorType.Humidity:
                    this.DefaultUnits = "%";
                    break;
                case SensorType.Voltage:
                    this.DefaultUnits = "V";
                    break;
            }

            this.Units = this.DefaultUnits;
        }
    }
}
