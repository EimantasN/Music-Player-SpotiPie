using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Highlight
    {
        [JsonProperty("property")]
        public string Property { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("snippet")]
        public bool Snippet { get; set; }

        [JsonProperty("ranges")]
        public List<Range> Ranges { get; set; }
    }
}
