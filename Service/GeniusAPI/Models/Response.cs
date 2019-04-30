using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Response
    {
        [JsonProperty("sections")]
        public List<Section> Sections { get; set; }
    }
}
