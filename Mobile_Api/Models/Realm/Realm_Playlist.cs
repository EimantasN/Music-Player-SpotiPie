using Realms;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Realm_Playlist : RealmObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public string Gendres { get; set; }

        public IList<Realm_Songs> Songs { get; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset LastActiveTime { get; set; }

        public long Limit { get; set; }

        public long Total { get; set; }

        public long Popularity { get; set; }

        public string Image { get; set; }
        protected int Type { get; set; }
    }
}
