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

        protected int Type { get; set; }
    }
}
