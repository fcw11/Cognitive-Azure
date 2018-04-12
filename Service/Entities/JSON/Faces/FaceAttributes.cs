using Newtonsoft.Json;

namespace Services.Entities.JSON.Faces
{
    public class FaceAttributes
    {
        [JsonProperty("smile")]
        public double Smile { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("age")]
        public double Age { get; set; }

        [JsonProperty("facialHair")]
        public FacialHair FacialHair { get; set; }

        [JsonProperty("glasses")]
        public string Glasses { get; set; }

        [JsonProperty("emotion")]
        public Emotion Emotion { get; set; }

        [JsonProperty("hair")]
        public Hair Hair { get; set; }
    }
}