using Realms;
using System;

namespace Mobile_Api.Models
{
    public class Realm_Quote : RealmObject
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTimeOffset Created { get; set; }
    }
}
