using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Sponsorship
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("sponsor_image")]
        public object SponsorImage { get; set; }

        [JsonProperty("sponsor_image_style")]
        public string SponsorImageStyle { get; set; }

        [JsonProperty("sponsor_link")]
        public string SponsorLink { get; set; }

        [JsonProperty("sponsor_phrase")]
        public string SponsorPhrase { get; set; }

        [JsonProperty("sponsored")]
        public bool Sponsored { get; set; }
    }
}
