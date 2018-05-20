using Newtonsoft.Json;

namespace Services.Entities.JSON.Audio
{
    public class IdentificationProfile
    {
        [JsonProperty("identificationProfileId")]
        public string IdentificationProfileId { get; set; }
    }
}