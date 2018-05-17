using Newtonsoft.Json;

namespace Services.Entities.JSON.KeyPhrases
{
    public class Document
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("keyPhrases")]
        public string[] KeyPhrases { get; set; }
    }
}
