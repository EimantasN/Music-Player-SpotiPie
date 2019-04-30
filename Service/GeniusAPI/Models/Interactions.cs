using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Interactions
    {
        [JsonProperty("following")]
        public bool Following { get; set; }
    }
}
