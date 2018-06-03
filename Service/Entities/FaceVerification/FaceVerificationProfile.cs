using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Services.Entities.FaceVerification
{
    public class FaceVerificationProfile : TableEntity
    {
        public string PersonGroupId { get; set; }

        [Required, MinLength(4), Display(Name = "Profile name")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("personId")]
        public Guid PersonId { get; set; }

        [JsonProperty("persistedFaceIds")]
        public List<Guid> PersistedFaceIds { get; set; }

        [JsonProperty("userData")]
        public string UserData { get; set; }
    }
}
