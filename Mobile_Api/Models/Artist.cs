using Mobile_Api.Models.Enums;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Artist : BaseModel
    {
        public override int Id { get; set; }

        public string SpotifyId { get; set; }

        public string Genres { get; set; }

        public string Name { get; set; }

        public long Popularity { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public List<Album> Albums { get; set; }

        public DateTime LastActiveTime { get; set; }

        protected override RvType Type { get; set; } = RvType.Artist;
    }
}
