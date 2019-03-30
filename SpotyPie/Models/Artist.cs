using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpotyPie
{
    public class Artist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //[Json] List<string>
        public string Genres { get; set; }

        public List<Image> Images { get; set; }

        public List<Item> Songs { get; set; }

        public List<Album> Albums { get; set; }

        public long Popularity { get; set; }
    }
}
