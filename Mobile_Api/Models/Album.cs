using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Album : BaseModel
    {

        public override int Id { get; set; }

        public string SpotifyId { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public List<Song> Songs { get; set; }

        public int Popularity { get; set; }

        public bool IsPlayable { get; set; }

        public DateTime LastActiveTime { get; set; }

        public int Tracks { get; set; }

        public Album()
        {

        }
    }
}
