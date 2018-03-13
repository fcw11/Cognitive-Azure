using Newtonsoft.Json;

namespace Services.Entities.JSON.Tags
{
    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("hint")]
        public string Hint { get; set; }
    }
}