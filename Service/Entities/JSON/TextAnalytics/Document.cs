using Newtonsoft.Json;

namespace Services.Entities.JSON.TextAnalytics
{
    public class Document
    {
        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}