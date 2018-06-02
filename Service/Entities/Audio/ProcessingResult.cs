using Newtonsoft.Json;

namespace Services.Entities.Audio
{
    public class ProcessingResult
    {
        [JsonProperty("identifiedProfileId")]
        public string IdentifiedProfileId { get; set; }

        [JsonProperty("confidence")]
        public string Confidence { get; set; }
    }
}