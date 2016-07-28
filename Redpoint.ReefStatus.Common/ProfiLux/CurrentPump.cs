
namespace RedPoint.ReefStatus.Common.ProfiLux
{
    public class CurrentPump : BaseInfo
    {
        public CurrentPump()
            : base("strCurrentPumps")
        {
            this.DefaultUnits = "%";
        }

        /// <summary>
        /// Gets DoubleValue.
        /// </summary>
        public override double DoubleValue
        {
            get { return this.Value != null ? (int)this.Value : 0; }
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

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }
    }
}
