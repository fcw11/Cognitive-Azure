using Newtonsoft.Json;

namespace Services.Entities.JSON.Describe
{
    public class Describe
    {
        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}