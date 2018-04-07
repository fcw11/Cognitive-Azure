using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Adult
    {
        [JsonProperty("isAdultContent")]
        public bool IsAdultContent { get; set; }

        [JsonProperty("adultScore")]
        public double AdultScore { get; set; }

        [JsonProperty("isRacyContent")]
        public bool IsRacyContent { get; set; }

        [JsonProperty("racyScore")]
        public double RacyScore { get; set; }
    }
}