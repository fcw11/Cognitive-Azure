﻿using Newtonsoft.Json;

namespace Services.Entities.JSON.Analyse
{
    public class Caption
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }
}