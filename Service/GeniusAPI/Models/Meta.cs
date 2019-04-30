using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Meta
    {
        [JsonProperty("status")]
        public long Status { get; set; }
    }
}
