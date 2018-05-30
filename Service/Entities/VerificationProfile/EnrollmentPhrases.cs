using Newtonsoft.Json;

namespace Services.Entities.VerificationProfile
{
    public class EnrollmentPhrases
    {
            [JsonProperty("phrase")]
            public string Phrase { get; set; }
    }
}
