// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DosingPump.cs" company="Redpoint Apps">
//   2010
// </copyright>
// <summary>
//   The dosing pump.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;

    /// <summary>
    /// The dosing pump.
    /// </summary>
    public class DosingPump : BaseInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DosingPump"/> class.
        /// </summary>
        public DosingPump()
            : base("Dosing")
        {
            this.DefaultUnits = "ml/day";
        }

        [JsonIgnore]
        public int Value => this.Rate * this.PerDay;

        [JsonIgnore]
        public int? OldValue { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public int Channel { get; set; }

        /// <summary>
        /// Gets or sets Rate.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        public int PerDay { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public TimerSettings Settings { get; set; }
    }
}
