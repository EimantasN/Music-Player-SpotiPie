using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Tracks : BaseModel<Tracks>
    {
        public int Id { get; set; }
        public List<Item> Items { get; set; }

        public long Total { get; set; }
    }
}
