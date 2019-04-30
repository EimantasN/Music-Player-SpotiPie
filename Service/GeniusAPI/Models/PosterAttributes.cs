using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class PosterAttributes
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }
}
