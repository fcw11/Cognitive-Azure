using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Services.Entities.Audio
{
    public class IdentificationStatus
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("lastActionDateTime")]
        public string LastActionDateTime { get; set; }

        [JsonProperty("processingResult")]
        public ProcessingResult ProcessingResult { get; set; }
    }
}