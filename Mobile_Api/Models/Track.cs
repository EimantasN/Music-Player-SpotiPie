using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Tracks : BaseModel
    {
        public override int Id { get; set; }
        public List<Song> Items { get; set; }

        public long Total { get; set; }
    }
}
