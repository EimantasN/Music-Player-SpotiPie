using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Playlist : BaseModel
    {
        public override int Id { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public string Gendres { get; set; }

        public List<Songs> Songs { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActiveTime { get; set; }

        public long Limit { get; set; }

        public long Total { get; set; }

        public long Popularity { get; set; }

        public string Image { get; set; }
        protected override RvType Type { get; set; } = RvType.Playlist;
    }
}
