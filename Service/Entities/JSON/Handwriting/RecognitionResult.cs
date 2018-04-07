using Newtonsoft.Json;

namespace Services.Entities.JSON.Handwriting
{
    public class RecognitionResult
    {
        [JsonProperty("lines")]
        public Line[] Lines { get; set; }
    }
}
