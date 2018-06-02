using Newtonsoft.Json;

namespace Services.Entities.Audio
{
    public class EnrollmentStatus
    {
        [JsonProperty("identificationProfileId")]
        public string IdentificationProfileId { get; set; }

        [JsonProperty("verificationProfileId")]
        public string VerificationProfileId {get;set;}

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("enrollmentSpeechTime")]
        public double EnrollmentSpeechTime { get; set; }

        [JsonProperty("remainingEnrollmentSpeechTime")]
        public long RemainingEnrollmentSpeechTime { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public string LastActionDateTime { get; set; }

        [JsonProperty("enrollmentStatus")]
        public string EnrollmentStatusEnrollmentStatus { get; set; }
    }
}
