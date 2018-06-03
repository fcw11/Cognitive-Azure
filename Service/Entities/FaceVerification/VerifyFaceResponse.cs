using Newtonsoft.Json;
using Services.Entities.VerificationProfile;

namespace Services.Entities.FaceVerification
{
    public class VerifyFaceResponse
    {
        [JsonProperty("isIdentical")]
        public bool IsIdentical { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("error")]
        public Error Error { get; set; }
    }
}