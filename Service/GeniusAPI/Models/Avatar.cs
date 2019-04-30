using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Avatar
    {
        [JsonProperty("tiny")]
        public Medium Tiny { get; set; }

        [JsonProperty("thumb")]
        public Medium Thumb { get; set; }

        [JsonProperty("small")]
        public Medium Small { get; set; }

        [JsonProperty("medium")]
        public Medium Medium { get; set; }
    }
}
