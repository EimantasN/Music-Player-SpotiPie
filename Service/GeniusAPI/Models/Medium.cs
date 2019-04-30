using Newtonsoft.Json;
using System;

namespace Service.GeniusAPI.Models
{
    public class Medium
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("bounding_box")]
        public PosterAttributes BoundingBox { get; set; }
    }
}
