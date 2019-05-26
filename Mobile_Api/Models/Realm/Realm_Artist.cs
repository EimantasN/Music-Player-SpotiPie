using Realms;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Realm_Artist : RealmObject
    {
        public int Id { get; set; }

        public string SpotifyId { get; set; }

        public string Genres { get; set; }

        public string Name { get; set; }

        public long Popularity { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public IList<Realm_Album> Albums { get; }

        public DateTimeOffset LastActiveTime { get; set; }

        protected int Type { get; set; }
    }
}
