using Newtonsoft.Json;

namespace Services.Entities.JSON.Tags
{
    public class Tags
    {
        [JsonProperty("tags")]
        public Tag[] TagsTags { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}
