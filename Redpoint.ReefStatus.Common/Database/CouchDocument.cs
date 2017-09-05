
namespace RedPoint.ReefStatus.Common.Database
{
    using Newtonsoft.Json;

    public class CouchDocument
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_rev")]
        public string Rev { get; set; }
    }
}
