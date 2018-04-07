using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Metadata
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}