using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Copyright
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string Type { get; set; }

        public List<Album> Albums { get; set; }
    }
}
