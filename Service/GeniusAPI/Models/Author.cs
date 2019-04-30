using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class Author
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("about_me_summary")]
        public string AboutMeSummary { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("header_image_url")]
        public Uri HeaderImageUrl { get; set; }

        [JsonProperty("human_readable_role_for_display")]
        public string HumanReadableRoleForDisplay { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("iq")]
        public long Iq { get; set; }

        [JsonProperty("is_meme_verified")]
        public bool IsMemeVerified { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("role_for_display")]
        public string RoleForDisplay { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("current_user_metadata")]
        public CurrentUserMetadata CurrentUserMetadata { get; set; }
    }
}
