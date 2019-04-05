using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Playlist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public string Gendres { get; set; }

        public List<Song> Songs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActiveTime { get; set; }

        public long Limit { get; set; }

        public long Total { get; set; }

        public long Popularity { get; set; }

        public string Image { get; set; }
    }
}
