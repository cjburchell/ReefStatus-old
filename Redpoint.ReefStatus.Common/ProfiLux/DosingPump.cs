// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DosingPump.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   The dosing pump.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux
{
    /// <summary>
    /// The dosing pump.
    /// </summary>
    public class DosingPump : BaseInfo
    {
        /// <summary>
        /// The channel.
        /// </summary>
        private int channel;

        /// <summary>
        /// The number.
        /// </summary>
        private int perDay;

        /// <summary>
        /// The rate.
        /// </summary>
        private int rate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DosingPump"/> class.
        /// </summary>
        public DosingPump()
            : base("strDosing")
        {
            this.DefaultUnits = Language.GetResource("strDosingUnits");
        }

        /// <summary>
        /// Gets DoubleValue.
        /// </summary>
        public override double DoubleValue
        {
            get { return this.Value != null ? (int)this.Value : 0; }
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
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public int Channel
        {
            get 
            {
                return this.channel;
            }

            set 
            {
                if (this.channel != value)
                {
                    this.channel = value;
                    this.OnPropertyChanged(() => this.Channel);
                }
            }
        }

        /// <summary>
        /// Gets or sets Rate.
        /// </summary>
        public int Rate
        {
            get
            {
                return this.rate;
            }

            set
            {
                if (this.rate != value)
                {
                    this.rate = value;
                    this.OnPropertyChanged(() => this.Rate);
                    this.Value = this.rate * this.PerDay;
                }
            }
        }

        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        public int PerDay
        {
            get
            {
                return this.perDay;
            }

            set
            {
                if (this.perDay != value)
                {
                    this.perDay = value;
                    this.OnPropertyChanged(() => this.PerDay);
                    this.Value = this.Rate * this.perDay;
                }
            }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public TimerSettings Settings {get; set;}
    }
}
