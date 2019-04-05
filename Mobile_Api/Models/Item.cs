using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile_Api.Models
{
    public class Song : BaseModel
    {
        public override int Id { get; set; }

        // JSON string
        public string Artists { get; set; }

        public long DiscNumber { get; set; }

        public long DurationMs { get; set; }

        public bool Explicit { get; set; }

        public bool IsLocal { get; set; }

        public bool IsPlayable { get; set; }

        public string Name { get; set; }

        public long TrackNumber { get; set; }

        public string LocalUrl { get; set; }

        public string LargeImage { get; set; }

        public DateTime LastActiveTime { get; set; }

        public bool Playing { get; set; }

        //public decimal Bitrate { get; set; }

        //public int Frequency { get; set; }

        //public int Channels { get; set; }

        public Song(bool fake)
        {
            Id = 10;
            Artists = "";
            Name = "Testas";
            LocalUrl = "";
            LargeImage = "";
        }
    }
}
