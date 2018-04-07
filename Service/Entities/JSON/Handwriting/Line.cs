using Newtonsoft.Json;

namespace Services.Entities.JSON.Handwriting
{
    public class Line
    {
        [JsonProperty("boundingBox")]
        public long[] BoundingBox { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("words")]
        public Word[] Words { get; set; }
    }
}
