// <copyright file="WebData.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.WebServer
{
    using ProfiLux;

    /// <summary>
    /// Web interface data
    /// </summary>
    public class WebData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebData"/> class.
        /// </summary>
        public WebData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebData"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WebData(BaseInfo item)
        {
            this.Name = item.Id;
            this.DisplayName = item.DisplayName;
            this.Units = item.Units;
            this.ValueString = item.ValueWithUnits;
            this.Value = item.ConvertedValue != null ? item.ConvertedValue.ToString() : string.Empty;
            this.Mode = item.Mode;
            if (item is SensorInfo)
            {
                this.IsAlarmOn = ((SensorInfo)item).IsAlarmOn;
            }
            else
            {
                this.IsAlarmOn = CurrentState.Off;
            }

            if (item is DosingPump)
            {
                DosingPump dosingPump = (DosingPump)item;
                this.PerDay = dosingPump.PerDay;
                this.Rate = dosingPump.Rate;
            }

            this.ModeValue = "0";
            if (item is LevelSensor)
            {
                this.ModeValue = ((int)((LevelSensor)item).OpertationMode).ToString();
            }
        }

        /// <summary>
        /// Gets or sets the mode value.
        /// </summary>
        /// <value>The mode value.</value>
        public string ModeValue { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the data.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the units.
        /// </summary>
        /// <value>The units.</value>
        public string Units { get; set; }

        /// <summary>
        /// Gets or sets the value string.
        /// </summary>
        /// <value>The value string.</value>
        public string ValueString { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the device.
        /// </summary>
        /// <value>The device.</value>
        public string Mode { get; set; }


        /// <summary>
        /// Gets or sets the is alarm on.
        /// </summary>
        /// <value>The is alarm on.</value>
        public CurrentState IsAlarmOn { get; set; }

        /// <summary>
        /// Gets or sets the per day.
        /// </summary>
        /// <value>The per day.</value>
        public int PerDay { get; set; }

        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        /// <value>The rate.</value>
        public int Rate { get; set; }

    }
}