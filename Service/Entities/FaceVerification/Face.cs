using System;
using Newtonsoft.Json;
using Services.Entities.JSON.Faces;

namespace Services.Entities.FaceVerification
{
    public class Face
    {
        [JsonProperty("faceId")]
        public Guid FaceId { get; set; }

        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
    }
}
