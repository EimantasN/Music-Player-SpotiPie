using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Range
    {
        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }
}
