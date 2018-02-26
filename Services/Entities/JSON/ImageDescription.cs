using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Services.Entities.JSON
{
    public partial class ImageDescription
    {
        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public partial class ImageDescription
    {
        public static ImageDescription FromJson(string json) => JsonConvert.DeserializeObject<ImageDescription>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ImageDescription self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.AssumeUniversal,
                },
            },
        };
    }
}