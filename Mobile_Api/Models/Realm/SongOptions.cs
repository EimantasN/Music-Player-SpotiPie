using Realms;

namespace Mobile_Api.Models
{
    public class Realm_SongOptions : RealmObject
    {
        public int Id { get; set; }

        public Enums.SongOptions ItemType { get; }

        public string Value { get; set; }

        protected int Type { get; set; }

    }
}
