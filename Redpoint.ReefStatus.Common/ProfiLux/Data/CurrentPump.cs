
namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;

    public class CurrentPump : BaseInfo
    {
        public CurrentPump()
            : base("CurrentPump")
        {
            this.DefaultUnits = "%";
            this.Units = this.DefaultUnits;
        }

        public int Value { get; set; }

        [JsonIgnore]
        public int? OldValue { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }
    }
}
