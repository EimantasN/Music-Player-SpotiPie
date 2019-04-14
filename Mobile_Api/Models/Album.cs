using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Album : BaseModel
    {
        [JsonProperty("id")]
        public override int Id { get; set; }

        [JsonProperty("spotifyId")]
        public string SpotifyId { get; set; }

        [JsonProperty("largeImage")]
        public string LargeImage { get; set; }

        [JsonProperty("mediumImage")]
        public string MediumImage { get; set; }

        [JsonProperty("smallImage")]
        public string SmallImage { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("songs")]
        public List<Song> Songs { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("isPlayable")]
        public bool IsPlayable { get; set; }

        [JsonProperty("lastActiveTime")]
        public DateTime LastActiveTime { get; set; }

        [JsonProperty("tracks")]
        public int Tracks { get; set; }

        private RvType Type { get; set; } = RvType.Album;

        public RvType GetModelType()
        {
            return Type;
        }

        public void SetModelType(RvType type)
        {
            Type = type;
        }

        public Album()
        {

        }
    }
}
