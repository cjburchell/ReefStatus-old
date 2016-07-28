// <copyright file="LevelSensor.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    /// <summary>
    /// Level Sensor
    /// </summary>
    public class LevelSensor : SensorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelSensor"/> class.
        /// </summary>
        public LevelSensor()
            : base("strLevelSensors")
        {
            SensorType = SensorType.Level;
            this.DefaultUnits = Language.GetResource("strState");
        }

        /// <summary>
        /// the opertation mode
        /// </summary>
        private LevelSensorOpertationMode opertationMode;

        private WaterMode waterMode;

        /// <summary>
        /// Gets or sets the opertation mode.
        /// </summary>
        /// <value>The opertation mode.</value>
        public LevelSensorOpertationMode OpertationMode
        {
            get
            {
                return this.opertationMode;
            }

            set
            {
                if (this.opertationMode != value)
                {
                    this.opertationMode = value;
                    this.OnPropertyChanged(() => this.OpertationMode);
                    this.OnPropertyChanged(() => this.CanDoWaterChange);
                }
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public override string Mode
        {
            get
            {
                return Language.GetFrendyName(this.OpertationMode);
            }
        }

        /// <summary>
        /// Gets the value with units.
        /// </summary>
        /// <value>The value with units.</value>
        public override string ValueWithUnits
        {
            get { return Language.GetFrendyName(Value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can water change.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance can water change; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnore]
        public bool CanDoWaterChange
        {
            get
            {
                return this.OpertationMode == LevelSensorOpertationMode.WaterChange ||
                       this.OpertationMode == LevelSensorOpertationMode.WaterChangeAndAutoTopOff;
            }
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get
            {
                if (this.Value != null)
                {
                    return (int)this.Value;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the old double value.
        /// </summary>
        public override double? OldDoubleValue
        {
            get
            {
                return this.OldValue != null ? (double?)((int)this.OldValue) : null;
            }
        }

        public override object ConvertedValue
        {
            get { return this.Value; }
        }

        /// <summary>
        /// Gets or sets the water mode.
        /// </summary>
        /// <value>
        /// The water mode.
        /// </value>
        public WaterMode WaterMode
        {
            get
            {
                return waterMode;
            }
            set
            {
                if (this.waterMode != value)
                {
                    this.waterMode = value;
                    this.OnPropertyChanged(() => this.WaterMode);
                }
            }
        }
    }
}
