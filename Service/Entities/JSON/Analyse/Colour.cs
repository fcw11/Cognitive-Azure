using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Colour
    {
        [JsonProperty("dominantColorForeground")]
        public string DominantColorForeground { get; set; }

        [JsonProperty("dominantColorBackground")]
        public string DominantColorBackground { get; set; }

        [JsonProperty("dominantColors")]
        public string[] DominantColors { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; }

        [JsonProperty("isBwImg")]
        public bool IsBwImg { get; set; }
    }
}