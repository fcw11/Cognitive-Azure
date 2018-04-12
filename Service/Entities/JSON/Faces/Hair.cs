using Newtonsoft.Json;

namespace Services.Entities.JSON.Faces
{
    public class Hair
    {
        [JsonProperty("bald")]
        public double Bald { get; set; }

        [JsonProperty("invisible")]
        public bool Invisible { get; set; }

        [JsonProperty("hairColor")]
        public HairColor[] HairColor { get; set; }
    }
}