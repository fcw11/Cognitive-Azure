using Newtonsoft.Json;

namespace Services.Entities.FaceVerification
{
    public class PersonGroups
    {
        [JsonProperty("personGroupId")]
        public string PersonGroupId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userData")]
        public object UserData { get; set; }
    }
}
