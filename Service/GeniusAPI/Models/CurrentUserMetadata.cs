using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.GeniusAPI.Models
{
    public class CurrentUserMetadata
    {
        [JsonProperty("permissions")]
        public List<dynamic> Permissions { get; set; }

        [JsonProperty("excluded_permissions")]
        public List<string> ExcludedPermissions { get; set; }

        [JsonProperty("interactions", NullValueHandling = NullValueHandling.Ignore)]
        public Interactions Interactions { get; set; }

        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> Features { get; set; }
    }
}
