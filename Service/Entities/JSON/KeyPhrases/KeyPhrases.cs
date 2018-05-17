using Newtonsoft.Json;

namespace Services.Entities.JSON.KeyPhrases
{
    public class KeyPhrases
    {
        [JsonProperty("documents")]
        public Document[] Documents { get; set; }

        [JsonProperty("errors")]
        public object[] Errors { get; set; }
    }
}
