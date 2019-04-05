using System;

namespace Mobile_Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public DateTimeOffset Birthdate { get; set; }

        public string Country { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Images { get; set; }
    }
}
