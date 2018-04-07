using Newtonsoft.Json;

namespace Services.Entities.JSON.Handwriting
{
    public class Handwriting
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("recognitionResult")]
        public RecognitionResult RecognitionResult { get; set; }
    }
}
