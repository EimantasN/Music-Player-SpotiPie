using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Hit
    {
        [JsonProperty("highlights")]
        public List<Highlight> Highlights { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("result")]
        public Result Result { get; set; }
    }
}
