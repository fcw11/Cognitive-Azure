using Newtonsoft.Json;

namespace Services.Entities.JSON
{
    public class Caption
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }
}