using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Stats
    {
        [JsonProperty("hot")]
        public bool Hot { get; set; }

        [JsonProperty("unreviewed_annotations")]
        public long UnreviewedAnnotations { get; set; }

        [JsonProperty("pageviews", NullValueHandling = NullValueHandling.Ignore)]
        public long? Pageviews { get; set; }

        [JsonProperty("concurrents", NullValueHandling = NullValueHandling.Ignore)]
        public long? Concurrents { get; set; }
    }
}
