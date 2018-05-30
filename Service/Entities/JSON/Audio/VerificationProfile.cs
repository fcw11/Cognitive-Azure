using Newtonsoft.Json;

namespace Services.Entities.JSON.Audio
{
    public class VerificationProfile
    {
        [JsonProperty("verificationProfileId")]
        public string IdentificationProfileId { get; set; }
    }
}