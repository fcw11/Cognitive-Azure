using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Analyse
    {
        [JsonProperty("categories")]
        public Category[] Categories { get; set; }

        [JsonProperty("tags")]
        public Tag[] Tags { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("faces")]
        public Face[] Faces { get; set; }

        [JsonProperty("adult")]
        public Adult Adult { get; set; }

        [JsonProperty("color")]
        public Colour Colour { get; set; }

        [JsonProperty("imageType")]
        public ImageType ImageType { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}