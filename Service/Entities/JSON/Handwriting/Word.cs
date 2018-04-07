using Newtonsoft.Json;

namespace Services.Entities.JSON.Handwriting
{
    public class Word
    {
        [JsonProperty("boundingBox")]
        public long[] BoundingBox { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
