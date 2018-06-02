using Newtonsoft.Json;

namespace Services.Entities.VerificationProfile
{
    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
