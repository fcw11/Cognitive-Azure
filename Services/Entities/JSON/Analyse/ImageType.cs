using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class ImageType
    {
        [JsonProperty("clipArtType")]
        public long ClipArtType { get; set; }

        [JsonProperty("lineDrawingType")]
        public long LineDrawingType { get; set; }
    }
}