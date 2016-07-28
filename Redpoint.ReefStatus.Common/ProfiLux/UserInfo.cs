// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Redpoint Apps">
// Copyright (c) Redpoint Apps. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    using System;
    using System.Drawing;
    using System.Xml.Serialization;

    using RedPoint.ReefStatus.Common.ViewModel;

    /// <summary>
    /// User Info
    /// </summary>
    public class UserInfo : BaseInfo, IRangeInfo
    {
        #region Constants and Fields

        /// <summary>
        /// The center value.
        /// </summary>
        private double centerValue;

        /// <summary>
        /// The high range colour value.
        /// </summary>
        private int highRangeColourValue;

        /// <summary>
        /// The low range colour value.
        /// </summary>
        private int lowRangeColourValue;

        /// <summary>
        /// The max range.
        /// </summary>
        private double maxRange;

        /// <summary>
        /// The min range.
        /// </summary>
        private double minRange;

        /// <summary>
        /// The nominal colour value.
        /// </summary>
        private int nominalColourValue;

        /// <summary>
        /// The show center.
        /// </summary>
        private bool showCenter;

        /// <summary>
        /// The show max.
        /// </summary>
        private bool showMax;

        /// <summary>
        /// The show min.
        /// </summary>
        private bool showMin;

        /// <summary>
        /// The time.
        /// </summary>
        private DateTime time;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        public UserInfo()
            : base("strOtherParam")
        {
            this.Value = 0.0;
            this.HighRangeColour = Color.Green;
            this.LowRangeColour = Color.Brown;
            this.NominalColour = Color.Purple;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets CenterValue.
        /// </summary>
        public double CenterValue
        {
            get
            {
                return this.centerValue;
            }

            set
            {
                if (this.centerValue == value)
                {
                    return;
                }

                this.centerValue = value;
                this.OnPropertyChanged(() => this.CenterValue);
            }
        }

        /// <summary>
        /// Gets ConvertedValue.
        /// </summary>
        public override object ConvertedValue
        {
            get
            {
                return this.Value;
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
                return this.Value != null ? (double)this.Value : 0.0;
            }
        }

        /// <summary>
        /// Gets Graph.
        /// </summary>
        [XmlIgnore]
        public override GraphViewModel Graph
        {
            get
            {
                return this.graph ?? (this.graph = new ProbeGraphViewModel(this));
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
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

        /// <summary>
        /// Gets or sets HighRangeMediaColour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color HighRangeMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(
                    this.HighRangeColour.A, this.HighRangeColour.R, this.HighRangeColour.G, this.HighRangeColour.B);
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
        [XmlIgnore]
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
        /// Gets or sets LowRangeMediaColour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color LowRangeMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(
                    this.LowRangeColour.A, this.LowRangeColour.R, this.LowRangeColour.G, this.LowRangeColour.B);
            }

            set
            {
                this.LowRangeColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets or sets MaxRange.
        /// </summary>
        public double MaxRange
        {
            get
            {
                return this.maxRange;
            }

            set
            {
                if (this.maxRange == value)
                {
                    return;
                }

                this.maxRange = value;
                this.OnPropertyChanged(() => this.MaxRange);
            }
        }

        /// <summary>
        /// Gets or sets MinRange.
        /// </summary>
        public double MinRange
        {
            get
            {
                return this.minRange;
            }

            set
            {
                if (this.minRange == value)
                {
                    return;
                }

                this.minRange = value;
                this.OnPropertyChanged(() => this.MinRange);
            }
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
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
        /// Gets or sets NominalMediaColour.
        /// </summary>
        [XmlIgnore]
        public System.Windows.Media.Color NominalMediaColour
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(
                    this.NominalColour.A, this.NominalColour.R, this.NominalColour.G, this.NominalColour.B);
            }

            set
            {
                this.NominalColour = Color.FromArgb(value.A, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// Gets OldDoubleValue.
        /// </summary>
        public override double? OldDoubleValue
        {
            get
            {
                return this.OldValue != null ? (double?)((double)this.OldValue) : null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowCenter.
        /// </summary>
        public bool ShowCenter
        {
            get
            {
                return this.showCenter;
            }

            set
            {
                if (this.showCenter == value)
                {
                    return;
                }

                this.showCenter = value;
                this.OnPropertyChanged(() => this.ShowCenter);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowMax.
        /// </summary>
        public bool ShowMax
        {
            get
            {
                return this.showMax;
            }

            set
            {
                if (this.showMax == value)
                {
                    return;
                }

                this.showMax = value;
                this.OnPropertyChanged(() => this.ShowMax);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ShowMin.
        /// </summary>
        public bool ShowMin
        {
            get
            {
                return this.showMin;
            }

            set
            {
                if (this.showMin == value)
                {
                    return;
                }

                this.showMin = value;
                this.OnPropertyChanged(() => this.ShowMin);
            }
        }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time
        {
            get
            {
                return this.time;
            }

            set
            {
                this.time = value;
                this.OnPropertyChanged(() => this.Time);
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IRangeInfo

        /// <summary>
        /// The convert value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The convert value.
        /// </returns>
        public double ConvertValue(double value)
        {
            return value;
        }

        #endregion

        #endregion
    }
}