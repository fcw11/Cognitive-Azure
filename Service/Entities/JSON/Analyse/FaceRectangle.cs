using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class FaceRectangle
    {
        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }
}