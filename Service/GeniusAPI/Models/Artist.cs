using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Artist
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("image_url")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("index_character")]
        public string IndexCharacter { get; set; }

        [JsonProperty("is_meme_verified")]
        public bool IsMemeVerified { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("iq", NullValueHandling = NullValueHandling.Ignore)]
        public long? Iq { get; set; }
    }
}
