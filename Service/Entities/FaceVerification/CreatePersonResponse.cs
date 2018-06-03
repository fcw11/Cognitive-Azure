using System;
using Newtonsoft.Json;

namespace Services.Entities.FaceVerification
{
    public class CreatePersonResponse
    {
        [JsonProperty("personId")]
        public Guid PersonId { get; set; }
    }
}