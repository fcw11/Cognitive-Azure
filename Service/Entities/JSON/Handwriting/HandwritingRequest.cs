using Newtonsoft.Json;

namespace Services.Entities.JSON.Handwriting
{
    public class HandwritingRequest
    {

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("operationlocation")]
        public string OperationLocation { get; set; }
    }
}