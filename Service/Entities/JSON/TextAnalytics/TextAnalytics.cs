using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Services.Entities.JSON.TextAnalytics
{
    public class TextAnalytics
    {
        [JsonProperty("documents")]
        public Document[] Documents { get; set; }

        [JsonProperty("errors")]
        public object[] Errors { get; set; }
    }
}