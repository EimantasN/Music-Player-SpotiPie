using Realms;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Realm_Album : RealmObject
    {
        public int Id { get; set; }

        public string SpotifyId { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public IList<Realm_Songs> Songs { get; }

        public int Popularity { get; set; }

        public bool IsPlayable { get; set; }

        public DateTimeOffset LastActiveTime { get; set; }

        public int Tracks { get; set; }

        public int Type { get; set; }

        public int AlbumListType { get; set; }

        public Realm_Album()
        {

        }

        public Realm_Album(Album x, int type)
        {
            Id = x.Id;
            SpotifyId = x.SpotifyId;
            LargeImage = x.LargeImage;
            MediumImage = x.MediumImage;
            SmallImage = x.SmallImage;
            Name = x.Name;
            ReleaseDate = x.ReleaseDate;
            Songs = new List<Realm_Songs>();
            Popularity = x.Popularity;
            IsPlayable = x.IsPlayable;
            LastActiveTime = x.LastActiveTime;
            Tracks = x.Tracks;
            Type = (int)x.GetModelType();
            AlbumListType = type;
        }

        public void Update(Album x, int type)
        {
            this.SpotifyId = x.SpotifyId;
            this.LargeImage = x.LargeImage;
            this.MediumImage = x.MediumImage;
            this.SmallImage = x.SmallImage;
            this.Name = x.Name;
            this.ReleaseDate = x.ReleaseDate;
            this.Popularity = x.Popularity;
            this.IsPlayable = x.IsPlayable;
            this.LastActiveTime = x.LastActiveTime;
            this.Tracks = x.Tracks;
            this.Type = (int)x.GetModelType();
            this.AlbumListType = type;
        }
    }
}
