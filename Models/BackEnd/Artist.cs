using System;
using System.Collections.Generic;

namespace Models.BackEnd
{
    public class Artist
    {
        public int Id { get; set; }

        public string SpotifyId { get; set; }

        public string Genres { get; set; }

        public string Name { get; set; }

        public long Popularity { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public List<Album> Albums { get; set; }

        public DateTime LastActiveTime { get; set; }

        public List<Quote> Quotes { get; set; }


    }
}
