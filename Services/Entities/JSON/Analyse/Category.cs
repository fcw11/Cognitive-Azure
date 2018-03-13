using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }
    }
}