using Newtonsoft.Json;

namespace Services.Entities.JSON.OCR
{
    public class OCR
    {
        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("orientation")]
        public string Orientation { get; set; }

        [JsonProperty("textAngle")]
        public long TextAngle { get; set; }

        [JsonProperty("regions")]
        public object[] Regions { get; set; }
    }
}