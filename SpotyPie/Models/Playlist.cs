using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotyPie
{
    public class Playlist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Item> Items { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActiveTime { get; set; }

        public long Limit { get; set; }

        public long Total { get; set; }

        public int Popularity { get; set; }

        public string ImageUrl { get; set; }
    }
}
