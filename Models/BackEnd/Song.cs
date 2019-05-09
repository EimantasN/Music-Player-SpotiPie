using Models.BackEnd.Enumerators;
using System;
using System.Collections.Generic;

namespace Models.BackEnd
{
    public class Song
    {
        public int Id { get; set; }

        public virtual int AlbumId { get; set; }

        public virtual int ArtistId { get; set; }

        public string SpotifyId { get; set; }

        public long DiscNumber { get; set; }

        public long DurationMs { get; set; }

        public bool Explicit { get; set; }

        public bool IsLocal { get; set; }

        public string Name { get; set; }

        public long TrackNumber { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public string LocalUrl { get; set; }

        public int Popularity { get; set; }

        public bool IsPlayable { get; set; }

        public DateTimeOffset LastActiveTime { get; set; }

        public DateTimeOffset UploadTime { get; set; }

        public long Size { get; set; }

        public int Corrupted { get; set; }

        //Just for quick access
        public string ArtistName { get; set; }

        public string AlbumName { get; set; }

        public List<Image> Images { get; set; }

        public SongType Type { get; set; }
    }
}
