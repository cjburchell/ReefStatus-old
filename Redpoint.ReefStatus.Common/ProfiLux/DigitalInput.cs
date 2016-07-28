namespace RedPoint.ReefStatus.Common.ProfiLux
{
    /// <summary>
    /// The digital input.
    /// </summary>
    public class DigitalInput : SensorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalInput"/> class.
        /// </summary>
        public DigitalInput()
            : base("strDigitalInputs")
        {
            SensorType = SensorType.DigitalInput;
            this.DefaultUnits = Language.GetResource("strState");
        }

        /// <summary>
        /// Gets the double value.
        /// </summary>
        /// <value>The double value.</value>
        public override double DoubleValue
        {
            get
            {
                return this.Value != null ? (int)this.Value : 0;
            }
        }

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


        private DigitalInputFunction function;

        /// <summary>
        /// Gets or sets the function.
        /// </summary>
        /// <value>The function.</value>
        public DigitalInputFunction Function
        {
            get
            {
                return this.function;
            }

            set
            {
                if (this.function != value)
                {
                    this.OnPropertyChanged(() => this.Function);
                    this.function = value;
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
                return Language.GetFrendyName(this.Function);
            }
        }

        /// <summary>
        /// Gets the value with units.
        /// </summary>
        /// <value>The value with units.</value>
        public override string ValueWithUnits
        {
            get { return Language.GetFrendyName(this.Value); }
        }
    }
}