using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Album : BaseModel, IDisposable
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
        public List<Songs> Songs { get; set; }

        [JsonProperty("popularity")]
        public int Popularity { get; set; }

        [JsonProperty("isPlayable")]
        public bool IsPlayable { get; set; }

        [JsonProperty("lastActiveTime")]
        public DateTime LastActiveTime { get; set; }

        [JsonProperty("tracks")]
        public int Tracks { get; set; }

        protected override RvType Type { get; set; } = RvType.Album;

        public Album()
        {
        }

        public Album(Realm_Album x)
        {
            Id = x.Id;
            SpotifyId = x.SpotifyId;
            LargeImage = x.LargeImage;
            MediumImage = x.MediumImage;
            SmallImage = x.SmallImage;
            Name = x.Name;
            ReleaseDate = x.ReleaseDate;
            Songs = new List<Songs>();
            Popularity = x.Popularity;
            IsPlayable = x.IsPlayable;
            LastActiveTime = x.LastActiveTime.DateTime;
            Tracks = x.Tracks;
            Type = (RvType)x.Type;
        }

        public void Dispose()
        {
            Songs = null;
            SpotifyId = null;
            LargeImage = null;
            MediumImage = null;
            SmallImage = null;
            Name = null;
            ReleaseDate = null;
        }
    }
}
