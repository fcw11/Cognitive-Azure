using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Services.Entities.VerificationProfile
{
    public class VerifySpeakerResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("confidence")]
        public string Confidence { get; set; }

        [JsonProperty("phrase")]
        public string Phrase { get; set; }

        [JsonProperty("Error")]
        public Error Error { get; set; }
    }
}
