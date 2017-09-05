namespace RedPoint.ReefStatus.Common.ProfiLux.Data
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class LogicFunction
    {
        public bool Invert1 { get; set; }

        public bool Invert2 { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogicMode LogicMode { get; set; }
    }
}
