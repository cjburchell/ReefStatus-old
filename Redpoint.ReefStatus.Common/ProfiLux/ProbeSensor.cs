// <copyright file="ProbeSensor.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using RedPoint.ReefStatus.Common.ViewModel;

    /// <summary>
    /// Probe Sensor
    /// </summary>
    public class Probe : SensorInfo, IRangeInfo
    {
        private double nominalValue;
        private double alarmDeviation;
        private bool alarmEnable;
        private SensorMode sensorMode;
        private int operationHours;

        public Probe()
            : base("strProbes")
        {
            HighRangeColour = Color.Green;
            LowRangeColour = Color.Brown;
            NominalColour = Color.Purple;
        }


        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int HighRangeColourValue
        {
            get
            {
                return this.highRangeColourValue;
            }

            set
            {
                if (this.highRangeColourValue != value)
                {
                    this.highRangeColourValue = value;
                    this.OnPropertyChanged(() => this.HighRangeMediaColour);
                    this.OnPropertyChanged(() => this.HighRangeColourValue);
                    this.OnPropertyChanged(() => this.HighRangeColour);
                }
            }
        }

        public override string DisplayNameValue
        {
            get
            {
                return base.DisplayNameValue;
            }

            set
            {
                if (value != base.DisplayNameValue)
                {
                    this.Commands.SendProbeText(this, value);
                    base.DisplayNameValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [System.Xml.Serialization.XmlIgnore]
        public Color HighRangeColour
        {
            get
            {
                return Color.FromArgb(this.HighRangeColourValue);
            }

            set
            {
                this.HighRangeColourValue = value.ToArgb();
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Windows.Media.Color HighRangeMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(this.HighRangeColour.A, this.HighRangeColour.R, this.HighRangeColour.G, this.HighRangeColour.B);
            }

            set
            {
                this.HighRangeColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int LowRangeColourValue
        {
            get
            {
                return this.lowRangeColourValue;
            }

            set
            {
                if (this.lowRangeColourValue != value)
                {
                    this.lowRangeColourValue = value;
                    this.OnPropertyChanged(() => this.LowRangeMediaColour);
                    this.OnPropertyChanged(() => this.LowRangeColourValue);
                    this.OnPropertyChanged(() => this.LowRangeColour);
                }
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [System.Xml.Serialization.XmlIgnore]
        public Color LowRangeColour
        {
            get
            {
                return Color.FromArgb(this.LowRangeColourValue);
            }

            set
            {
                this.LowRangeColourValue = value.ToArgb();
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Windows.Media.Color LowRangeMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(this.LowRangeColour.A, this.LowRangeColour.R, this.LowRangeColour.G, this.LowRangeColour.B);
            }

            set
            {
                this.LowRangeColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public int NominalColourValue
        {
            get
            {
                return this.nominalColourValue;
            }

            set
            {
                if (this.nominalColourValue != value)
                {
                    this.nominalColourValue = value;
                    this.OnPropertyChanged(() => this.NominalMediaColour);
                    this.OnPropertyChanged(() => this.NominalColourValue);
                    this.OnPropertyChanged(() => this.NominalColour);
                }
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [System.Xml.Serialization.XmlIgnore]
        public Color NominalColour
        {
            get
            {
                return Color.FromArgb(this.NominalColourValue);
            }

            set
            {
                this.NominalColourValue = value.ToArgb();
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public System.Windows.Media.Color NominalMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(this.NominalColour.A, this.NominalColour.R, this.NominalColour.G, this.NominalColour.B);
            }

            set
            {
                this.NominalColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public override GraphViewModel Graph
        {
            get
            {
                return this.graph ?? (this.graph = new ProbeGraphViewModel(this));
            }
        }

        /// <summary>
        /// Gets or sets the nominal value.
        /// </summary>
        /// <value>The nominal value.</value>
        public double NominalValue
        {
            get
            {
                return this.nominalValue;
            }

            set
            {
                this.nominalValue = value;
                this.OnPropertyChanged(() => this.NominalValue);
                this.OnPropertyChanged(() => this.CenterValue);
                this.OnPropertyChanged(() => this.MinRange);
                this.OnPropertyChanged(() => this.MaxRange);
            }
        }

        /// <summary>
        /// Gets or sets the sensor mode.
        /// </summary>
        /// <value>The sensor mode.</value>
        public SensorMode SensorMode
        {
            get
            {
                return this.sensorMode;
            }

            set
            {
                this.sensorMode = value;
                this.OnPropertyChanged(() => this.SensorMode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [alarm enable].
        /// </summary>
        /// <value><c>true</c> if [alarm enable]; otherwise, <c>false</c>.</value>
        public bool AlarmEnable
        {
            get
            {
                return this.alarmEnable;
            }

            set
            {
                this.alarmEnable = value;
                this.OnPropertyChanged(() => this.AlarmEnable);
                this.OnPropertyChanged(() => this.ShowMin);
                this.OnPropertyChanged(() => this.ShowMax);
            }
        }

        /// <summary>
        /// Gets or sets the alarm deviation.
        /// </summary>
        /// <value>The alarm deviation.</value>
        public double AlarmDeviation
        {
            get
            {
                return this.alarmDeviation;
            }

            set
            {
                this.alarmDeviation = value;
                this.OnPropertyChanged(() => this.AlarmDeviation);
                this.OnPropertyChanged(() => this.MinRange);
                this.OnPropertyChanged(() => this.MaxRange);
            }
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get { return this.Value != null ? (double)this.Value : 0; }
        }

        /// <summary>
        /// Gets the old double value.
        /// </summary>
        public override double? OldDoubleValue
        {
            get
            {
                return this.OldValue != null ? (double?)((double)this.OldValue) : null;
            }
        }

        /// <summary>
        /// Gets or sets the operation hours.
        /// </summary>
        /// <value>The operation hours.</value>
        public int OperationHours
        {
            get
            {
                return this.operationHours;
            }

            set
            {
                if (this.operationHours != value)
                {
                    this.operationHours = value;
                    this.OnPropertyChanged(() => this.OperationHours);
                }
            }
        }

        private int maxOperationHours;

        /// <summary>
        /// Gets or sets the max operation hours.
        /// </summary>
        /// <value>The max operation hours.</value>
        public int MaxOperationHours
        {
            get
            {
                return this.maxOperationHours;
            }

            set
            {
                if (this.maxOperationHours != value)
                {
                    this.maxOperationHours = value;
                    this.OnPropertyChanged(() => this.MaxOperationHours);
                    this.OnPropertyChanged(() => this.IsOverMaxOperationHours);
                }
            }
        }

        private bool enableMaxOperationHours;

        /// <summary>
        /// Gets or sets a value indicating whether [enable max operation hours].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable max operation hours]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMaxOperationHours
        {
            get
            {
                return this.enableMaxOperationHours;
            }

            set
            {
                if (this.enableMaxOperationHours != value)
                {
                    this.enableMaxOperationHours = value;
                    this.OnPropertyChanged(() => this.EnableMaxOperationHours);
                    this.OnPropertyChanged(() => this.IsOverMaxOperationHours);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is over max operation hours.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is over max operation hours; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverMaxOperationHours
        {
            get
            {
                return this.EnableMaxOperationHours && this.MaxOperationHours < (this.OperationHours / 60.0);
            }
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(int value)
        {
            var probeValue = this.ConvertFromInt(value);

            if (this.SensorType != SensorType.AirTemperature || probeValue != 60.0)
            {
                this.OldValue = this.Value;
                this.Value = probeValue;
            }
        }

        public override object ConvertedValue
        {
            get
            {
                return this.ConvertValue(this.Value != null ? (double)this.Value : 0);
            }
        }

        public double ConvertFromInt(int value)
        {
            switch (SensorType)
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

        public int Digits
        {
            get
            {
                switch (SensorType)
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
                        if (Format == 1)
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

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the converted value</returns>
        public double ConvertValue(double value)
        {
            int digits = this.Digits;
            if (this.UseCustomCoversion)
            {
                var temp = ((value + this.ConversionControl1) * this.ConversionControl2) + this.ConversionControl3;
                return Math.Round(temp, digits);    
            }

            switch (SensorType)
            {
                case SensorType.AirTemperature:
                case SensorType.Temperature:
                    {
                        if (Format == 1)
                        {
                            // convert temperature to Fahrenheit
                            return Math.Round(Math.Round((1.8 * value) + 32, 2), digits);
                        }
                    }
                    break;

                case SensorType.Conductivity:
                    if (Format == 1)
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


        public bool UseCustomCoversion
        {
            get
            {
                return this.useCustomCoversion;
            }

            set
            {
                if (this.useCustomCoversion != value)
                {
                    this.useCustomCoversion = value;
                    this.OnPropertyChanged(() => this.UseCustomCoversion);
                    this.OnPropertyChanged(() => this.ConvertedValue);
                    this.OnPropertyChanged(() => this.ValueWithUnits);
                }
            }
        }

        private double conversionControl1;

        public double ConversionControl1
        {
            get
            {
                return this.conversionControl1;
            }

            set
            {
                if (this.conversionControl1 != value)
                {
                    this.conversionControl1 = value;
                    this.OnPropertyChanged(() => this.ConversionControl1);
                    this.OnPropertyChanged(() => this.ConvertedValue);
                    this.OnPropertyChanged(() => this.ValueWithUnits);
                }
            }
        }

        private double conversionControl2 = 1;

        public double ConversionControl2
        {
            get
            {
                return this.conversionControl2;
            }

            set
            {
                if (this.conversionControl2 != value)
                {
                    this.conversionControl2 = value;
                    this.OnPropertyChanged(() => this.ConversionControl2);
                    this.OnPropertyChanged(() => this.ConvertedValue);
                    this.OnPropertyChanged(() => this.ValueWithUnits);
                }
            }
        }

        private double conversionControl3;

        private bool useCustomCoversion;

        private int nominalColourValue;

        private int highRangeColourValue;

        private int lowRangeColourValue;

        public double ConversionControl3
        {
            get
            {
                return this.conversionControl3;
            }

            set
            {
                if (this.conversionControl3 != value)
                {
                    this.conversionControl3 = value;
                    this.OnPropertyChanged(() => this.ConversionControl3);
                    this.OnPropertyChanged(() => this.ConvertedValue);
                    this.OnPropertyChanged(() => this.ValueWithUnits);
                }
            }
        }

        public double CenterValue
        {
            get
            {
                return this.ConvertValue(this.NominalValue);
            }
        }

        public double MaxRange
        {
            get
            {
                return this.ConvertValue(this.NominalValue + this.AlarmDeviation);
            }
        }

        public double MinRange
        {
            get
            {
                return this.ConvertValue(this.NominalValue - this.AlarmDeviation);
            }
        }

        public bool ShowMin
        {
            get
            {
                return this.AlarmEnable;
            }
        }

        public bool ShowMax
        {
            get
            {
                return this.AlarmEnable;
            }
        }

        public bool ShowCenter
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Convers to salinity.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <returns>the converted value</returns>
        private static double ConverToSalinity(double cond)
        {
            Dictionary<double, double> conversionTable = new Dictionary<double, double>
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

            double salinity =
                (from value in conversionTable.Keys where value >= cond select cond * (conversionTable[value] / value)).
                    FirstOrDefault();

            if (salinity == 0)
            {
                salinity = cond * (conversionTable[60.0] / 60.0);
            }

            return Math.Round(salinity, 1);
        }

        /// <summary>
        /// Convers to SG.
        /// </summary>
        /// <param name="cond">The cond.</param>
        /// <param name="offset">if set to <c>true</c> [offset].</param>
        /// <returns>the converted value</returns>
        private static double ConverToSg(double cond, bool offset)
        {
            Dictionary<double, double> conversionTable = new Dictionary<double, double>
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

            double sg =
                (from value in conversionTable.Keys
                 where value >= cond
                 select cond * ((conversionTable[value] - 1.0) / value)).FirstOrDefault();

            if (sg == 0)
            {
                sg = cond * ((conversionTable[60] - 1.0) / 60.0);
            }

            return offset ? Math.Round(sg, 4) : Math.Round(sg, 4) + 1.0;
        }

        ////private static double ConverToSalinity2(double cond)
        ////{
        ////    double r = cond / 4.2914;
        ////    const double temp = 25;

        ////    const double c0 = 0.6766097;
        ////    const double c1 = 2.00564E-2;
        ////    const double c2 = 1.104259E-4;
        ////    const double c3 = -6.9698E-7;
        ////    const double c4 = 1.0031E-9;

        ////    double rsubt = c0 + (c1 * temp) + c2 * Math.Pow(temp, 2) + c3 * Math.Pow(temp, 3) +
        ////                   c4 * Math.Pow(temp, 4);

        ////    const double e1 = 2.070E-5;
        ////    const double e2 = -6.370E-10;
        ////    const double e3 = 3.989E-15;

        ////    const double d1 = 3.426E-2;
        ////    const double d2 = 4.464E-4;
        ////    const double d3 = 4.215E-1;
        ////    const double d4 = -3.107E-3;

        ////    const double p = 10.1325;

        ////    double Rsubp = 1 + p * (e1 + e2 * p + e3 * Math.Pow(p, 2)) / (1 + d1 * temp + d2 * Math.Pow(temp, 2) + (d3 + d4 * temp)) * r;

        ////    double Rsubt = r / (Rsubp * rsubt);

        ////    const double k = 0.0162;

        ////    const double b0 = 0.0005;
        ////    const double b1 = -0.0056;
        ////    const double b2 = -0.0066;
        ////    const double b3 = -0.0375;
        ////    const double b4 = 0.0636;
        ////    const double b5 = -0.0144;

        ////    double S = (temp - 15) / (1 + k * (temp - 15)) *
        ////               (b0 + b1 * Math.Sqrt(Rsubt) + b2 * Rsubt + b3 * Math.Pow(Rsubt, (3 / 2)) +
        ////                b4 * Math.Pow(Rsubt, 2) + b5 * Math.Pow(Rsubt, (5 / 2)));

        ////    const double a0 = 0.0080;
        ////    const double a1 = -0.1692;
        ////    const double a2 = 25.3851;
        ////    const double a3 = 14.0941;
        ////    const double a4 = -7.0261;
        ////    const double a5 = 2.7081;

        ////    double salinity = a0 + a1 * Math.Sqrt(Rsubt) + a2 * Rsubt + a3 * Math.Pow(Rsubt, (3 / 2)) + a4 * Math.Pow(Rsubt, 2) + a5 * Math.Pow(Rsubt, (5 / 2)) + S;

        ////    return salinity;
        ////}
    }
}
