using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class ReleaseDateComponents
    {
        [JsonProperty("year")]
        public long Year { get; set; }

        [JsonProperty("month")]
        public long? Month { get; set; }

        [JsonProperty("day")]
        public long? Day { get; set; }
    }
}
