using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Models
{
    public class Playlist : BaseModel
    {
        public override int Id { get; set; }

        public string Name { get; set; }

        public List<Song> Items { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActiveTime { get; set; }

        public long Limit { get; set; }

        public long Total { get; set; }

        public int Popularity { get; set; }

        public string ImageUrl { get; set; }
    }
}
