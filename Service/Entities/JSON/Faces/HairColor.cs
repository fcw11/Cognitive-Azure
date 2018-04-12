using Newtonsoft.Json;

namespace Services.Entities.JSON.Faces
{
    public class HairColor
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }
}