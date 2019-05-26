using Realms;

namespace Mobile_Api.Models
{
    public class Realm_Image : RealmObject
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string LocalUrl { get; set; }

        public string Base64 { get; set; }

        public long Height { get; set; }

        public long Width { get; set; }
    }

}
