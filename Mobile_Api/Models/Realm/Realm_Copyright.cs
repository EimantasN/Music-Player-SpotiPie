using Realms;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Realm_Copyright : RealmObject
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Type { get; set; }

        public IList<Realm_Album> Albums { get; }
    }
}
