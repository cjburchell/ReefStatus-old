namespace RedPoint.ReefStatus.Common.ProfiLux
{
    /// <summary>
    /// Light Timer
    /// </summary>
    public class Light : BaseInfo
    {
        /// <summary>
        /// the channle
        /// </summary>
        private int channel;

        /// <summary>
        /// if the light is dimmable
        /// </summary>
        private bool isDimmable;

        private int operationHours;

        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        public Light()
            : base("strLights")
        {
            DefaultUnits = "%";
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get
            {
                if (Value != null)
                {
                    return (double)Value;
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
                return this.OldValue != null ? (double?)((double)this.OldValue) : null;
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
                    this.Commands.SendLightText(this, value);
                    base.DisplayNameValue = value;
                }
            }
        }

        public override object ConvertedValue
        {
            get { return this.Value; }
        }

        /// <summary>
        /// Gets or sets the light value.
        /// </summary>
        /// <value>The light value.</value>
        [System.Xml.Serialization.XmlIgnore]
        public double LightValue
        {
            get
            {
                return this.DoubleValue;
            }

            set
            {
                if (this.DoubleValue != value)
                {
                    this.Commands.SendLightState(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is light on.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is light on; otherwise, <c>false</c>.
        /// </value>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsLightOn
        {
            get
            {
                return this.DoubleValue != 0;
            }

            set
            {
                if ((this.DoubleValue != 0) != value)
                {
                    this.Commands.SendLightState(this, value ? 100 : 0);
                }
            }
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
        /// Gets or sets a value indicating whether this instance is dimmable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is dimmable; otherwise, <c>false</c>.
        /// </value>
        public bool IsDimmable
        {
            get 
            {
                return this.isDimmable;
            }

            set
            {
                if (this.isDimmable != value)
                {
                    this.isDimmable = value;
                    this.OnPropertyChanged(() => this.IsDimmable);
                }
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
                    this.OnPropertyChanged(() => this.IsOverMaxOperationHours);
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
    }
}
