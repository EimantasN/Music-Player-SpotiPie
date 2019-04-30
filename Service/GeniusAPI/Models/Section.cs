using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Section
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("hits")]
        public List<Hit> Hits { get; set; }
    }
}
