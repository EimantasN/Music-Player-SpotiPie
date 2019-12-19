using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Album : BaseModel, IBaseInterface<Album>, IDisposable
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
        public DateTimeOffset LastActiveTime { get; set; }

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

        //public Album(Realm_Album album)
        //{
        //    Id = album.Id;
        //    SpotifyId = album.SpotifyId;
        //    LargeImage = album.LargeImage;
        //    MediumImage = album.MediumImage;
        //    SmallImage = album.SmallImage;
        //    Name = album.Name;
        //    ReleaseDate = album.ReleaseDate;
        //    Songs = new List<Songs>();
        //    Popularity = album.Popularity;
        //    IsPlayable = album.IsPlayable;
        //    LastActiveTime = album.LastActiveTime.ToUniversalTime();
        //    Tracks = album.Tracks;
        //    Type = (RvType)album.Type;
        //}

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

        public bool Equals(Album obj)
        {
            if (Id == obj.Id)
                return true;
            return false;
        }
    }
}
