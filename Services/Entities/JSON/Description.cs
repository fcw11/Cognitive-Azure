using Newtonsoft.Json;

namespace Services.Entities.JSON
{
    public class Description
    {
        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("captions")]
        public Caption[] Captions { get; set; }
    }
}