// <copyright file="ProbeSensor.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    ///     Probe Sensor
    /// </summary>
    public class Probe : SensorInfo
    {
        public Probe()
            : base("Probe")
        {
        }

        /// <summary>
        ///     Gets or sets the nominal value.
        /// </summary>
        /// <value>The nominal value.</value>
        public double NominalValue { get; set; }

        /// <summary>
        ///     Gets or sets the sensor mode.
        /// </summary>
        /// <value>The sensor mode.</value>
        [JsonConverter(typeof(StringEnumConverter))]
        public SensorMode SensorMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [alarm enable].
        /// </summary>
        /// <value><c>true</c> if [alarm enable]; otherwise, <c>false</c>.</value>
        public bool AlarmEnable { get; set; }

        /// <summary>
        ///     Gets or sets the alarm deviation.
        /// </summary>
        /// <value>The alarm deviation.</value>
        public double AlarmDeviation { get; set; }

        /// <summary>
        ///     Gets or sets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public double Value { get; set; }

        /// <summary>
        ///     Gets or sets the old double value.
        /// </summary>
        [JsonIgnore]
        public double? OldValue { get; set; }

        /// <summary>
        ///     Gets or sets the operation hours.
        /// </summary>
        /// <value>The operation hours.</value>
        public int OperationHours { get; set; }

        /// <summary>
        ///     Gets or sets the max operation hours.
        /// </summary>
        /// <value>The max operation hours.</value>
        public int MaxOperationHours { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [enable max operation hours].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable max operation hours]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMaxOperationHours { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is over max operation hours.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is over max operation hours; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverMaxOperationHours => this.EnableMaxOperationHours && this.MaxOperationHours < this.OperationHours / 60.0;

        public int Digits
        {
            get
            {
                switch (this.SensorType)
                {
                    case SensorType.PH:
                    case SensorType.AirTemperature:
                    case SensorType.Temperature:
                    case SensorType.ConductivityF:
                    case SensorType.Oxygen:
                    case SensorType.Humidity:
                        {
                            return 2;
                        }
                    case SensorType.Conductivity:
                        if (this.Format == 1)
                        {
                            return 2;
                        }

                        if (this.Format == 2)
                        {
                            return 4;
                        }

                        return 2;
                    default:
                        return 0;
                }
            }
        }

        public bool UseCustomCoversion { get; set; }

        public double ConversionControl1 { get; set; }

        public double ConversionControl2 { get; set; } = 1;

        public double ConversionControl3 { get; set; }

        public double ConvertedValue => this.ConvertValue(this.Value);

        public double CenterValue => this.ConvertValue(this.NominalValue);

        public double MaxRange => this.ConvertValue(this.NominalValue + this.AlarmDeviation);

        public double MinRange => this.ConvertValue(this.NominalValue - this.AlarmDeviation);

        /// <summary>
        ///     Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the converted value</returns>
        public double ConvertValue(double value)
        {
            var digits = this.Digits;
            if (this.UseCustomCoversion)
            {
                var temp = ((value + this.ConversionControl1) * this.ConversionControl2) + this.ConversionControl3;
                return Math.Round(temp, digits);
            }

            switch (this.SensorType)
            {
                case SensorType.AirTemperature:
                case SensorType.Temperature:
                    {
                        if (this.Format == 1)
                        {
                            // convert temperature to Fahrenheit
                            return Math.Round(Math.Round(1.8 * value + 32, 2), digits);
                        }
                    }
                    break;

                case SensorType.Conductivity:
                    if (this.Format == 1)
                    {
                        return Math.Round(ConverToSalinity(value), digits);
                    }

                    if (this.Format == 2)
                    {
                        return Math.Round(ConverToSg(value, false), digits);
                    }
                    break;
            }

            return Math.Round(value, digits);
        }

        /// <summary>
        ///     Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(int value)
        {
            var probeValue = this.ConvertFromInt(value);

            if (this.SensorType != SensorType.AirTemperature || Math.Abs(probeValue - 60.0) > double.Epsilon)
            {
                this.OldValue = this.Value;
                this.Value = probeValue;
            }
        }

        public double ConvertFromInt(int value)
        {
            switch (this.SensorType)
            {
                case SensorType.PH:
                    return value * 0.01;
                case SensorType.AirTemperature:
                case SensorType.Temperature:
                    {
                        return value * 0.1;
                    }

                case SensorType.ConductivityF:
                    return value;
                case SensorType.Conductivity:
                    return value * 0.1;

                case SensorType.Oxygen:
                case SensorType.Humidity:
                    return value * 0.1;
                default:
                    return value;
            }
        }

        /// <summary>
        ///     Convers to salinity.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <returns>the converted value</returns>
        private static double ConverToSalinity(double cond)
        {
            var conversionTable = new Dictionary<double, double>
                                      {
                                          { 40, 25.5 },
                                          { 40.5, 25.9 },
                                          { 41, 26.2 },
                                          { 41.5, 26.6 },
                                          { 42, 26.9 },
                                          { 42.5, 27.3 },
                                          { 43, 27.7 },
                                          { 43.5, 28 },
                                          { 44, 28.4 },
                                          { 44.5, 28.7 },
                                          { 45, 29.1 },
                                          { 45.5, 29.5 },
                                          { 46, 29.8 },
                                          { 46.5, 30.2 },
                                          { 47, 30.5 },
                                          { 47.5, 30.9 },
                                          { 48, 31.3 },
                                          { 48.5, 31.6 },
                                          { 49, 32 },
                                          { 49.5, 32.4 },
                                          { 50, 32.7 },
                                          { 50.5, 33.1 },
                                          { 51, 33.5 },
                                          { 51.5, 33.8 },
                                          { 52, 34.2 },
                                          { 52.5, 34.6 },
                                          { 53, 34.9 },
                                          { 53.5, 35.3 },
                                          { 54, 35.7 },
                                          { 54.5, 36.1 },
                                          { 55, 36.4 },
                                          { 55.5, 36.8 },
                                          { 56, 37.2 },
                                          { 56.5, 37.6 },
                                          { 57, 37.9 },
                                          { 57.5, 38.3 },
                                          { 58, 38.7 },
                                          { 58.5, 39.1 },
                                          { 59, 39.6 },
                                          { 59.5, 39.8 },
                                          { 60, 40.2 }
                                      };

            var salinity = (from value in conversionTable.Keys where value >= cond select cond * (conversionTable[value] / value)).FirstOrDefault();

            if (Math.Abs(salinity) < double.Epsilon)
            {
                salinity = cond * (conversionTable[60.0] / 60.0);
            }

            return Math.Round(salinity, 1);
        }

        /// <summary>
        ///     Convers to SG.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <param name="offset">if set to <c>true</c> [offset].</param>
        /// <returns>the converted value</returns>
        private static double ConverToSg(double cond, bool offset)
        {
            var conversionTable = new Dictionary<double, double>
                                      {
                                          { 40, 1.0187 },
                                          { 40.5, 1.019 },
                                          { 41, 1.0193 },
                                          { 41.5, 1.0195 },
                                          { 42, 1.0198 },
                                          { 42.5, 1.0201 },
                                          { 43, 1.0204 },
                                          { 43.5, 1.0206 },
                                          { 44, 1.0209 },
                                          { 44.5, 1.0212 },
                                          { 45, 1.0214 },
                                          { 45.5, 1.0217 },
                                          { 46, 1.022 },
                                          { 46.5, 1.0223 },
                                          { 47, 1.0225 },
                                          { 47.5, 1.0228 },
                                          { 48, 1.0231 },
                                          { 48.5, 1.0234 },
                                          { 49, 1.0236 },
                                          { 49.5, 1.0239 },
                                          { 50, 1.0242 },
                                          { 50.5, 1.0245 },
                                          { 51, 1.0248 },
                                          { 51.5, 1.025 },
                                          { 52, 1.0253 },
                                          { 52.5, 1.0256 },
                                          { 53, 1.0259 },
                                          { 53.5, 1.0262 },
                                          { 54, 1.0264 },
                                          { 54.5, 1.0267 },
                                          { 55, 1.027 },
                                          { 55.5, 1.0273 },
                                          { 56, 1.0276 },
                                          { 56.5, 1.0278 },
                                          { 57, 1.0281 },
                                          { 57.5, 1.0284 },
                                          { 58, 1.0287 },
                                          { 58.5, 1.029 },
                                          { 59, 1.0293 },
                                          { 59.5, 1.0296 },
                                          { 60, 1.0299 }
                                      };

            var sg = (from value in conversionTable.Keys where value >= cond select cond * ((conversionTable[value] - 1.0) / value)).FirstOrDefault();

            if (Math.Abs(sg) < double.Epsilon)
            {
                sg = cond * ((conversionTable[60] - 1.0) / 60.0);
            }

            return offset ? Math.Round(sg, 4) : Math.Round(sg, 4) + 1.0;
        }
    }
}