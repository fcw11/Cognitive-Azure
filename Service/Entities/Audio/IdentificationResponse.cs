using Newtonsoft.Json;
using Services.Entities.VerificationProfile;

namespace Services.Entities.Audio
{
    public class IdentificationResponse
    {
        [JsonProperty("OperationLocation")]
        public string OperationLocation { get; set; }

        [JsonProperty("Error")]
        public Error Error { get; set; }
    }
}