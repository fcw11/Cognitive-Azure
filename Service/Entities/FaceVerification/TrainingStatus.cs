using Newtonsoft.Json;
using Services.Entities.VerificationProfile;

namespace Services.Entities.FaceVerification
{
    public class TrainingStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public string LastActionDateTime { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}