using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;
using Realms;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Realm_Songs : RealmObject
    {
        public int Id { get; set; }

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

        public bool IsPlaying { get; set; }

        public string ArtistName { get; set; }

        public string AlbumName { get; set; }

        public int AlbumId { get; set; }

        public int ArtistId { get; set; }

        public IList<Realm_Image> Images { get; }

        public int Type { get; set; }

    }
}
