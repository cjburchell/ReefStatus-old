namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Light Timer
    /// </summary>
    public class Light : BaseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Light"/> class.
        /// </summary>
        public Light()
            : base("Light")
        {
            this.DefaultUnits = "%";
            this.Units = this.DefaultUnits;
        }

        public double Value { get; set; }

        [JsonIgnore]
        public double? OldValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is light on.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is light on; otherwise, <c>false</c>.
        /// </value>
        public bool IsLightOn => Math.Abs(this.Value) > double.Epsilon;

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public int Channel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is dimmable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is dimmable; otherwise, <c>false</c>.
        /// </value>
        public bool IsDimmable { get; set; }

        /// <summary>
        /// Gets or sets the operation hours.
        /// </summary>
        /// <value>The operation hours.</value>
        public int OperationHours { get; set; }

        /// <summary>
        /// Gets or sets the max operation hours.
        /// </summary>
        /// <value>The max operation hours.</value>
        public int MaxOperationHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable max operation hours].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [enable max operation hours]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMaxOperationHours { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is over max operation hours.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is over max operation hours; otherwise, <c>false</c>.
        /// </value>
        public bool IsOverMaxOperationHours => this.EnableMaxOperationHours && this.MaxOperationHours < (this.OperationHours / 60.0);
    }
}
