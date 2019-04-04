using System;

namespace Models.BackEnd
{
    public class Item
    {
        public int Id { get; set; }

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

        public string ImageUrl { get; set; }

        public DateTime LastActiveTime { get; set; }

        public long Popularity { get; set; }

        public string Image { get; set; }

        public string Genres { get; set; }

        //public decimal Bitrate { get; set; }

        //public int Frequency { get; set; }

        //public int Channels { get; set; }
    }
}
