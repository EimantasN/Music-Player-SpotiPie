using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;
using Realms;
using System;
using System.Collections.Generic;

namespace Mobile_Api.Models
{
    public class Songs : BaseModel, IBaseInterface<Songs>, IDisposable
    {
        public override int Id { get; set; }

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

        public List<Image> Images { get; set; }

        protected override RvType Type { get; set; } = RvType.Song;

        public bool IsPlayingNow() { return IsPlaying; }

        public void SetIsPlaying(bool state) { IsPlaying = state; }

        public Songs()
        {
        }

        public Songs(Realm_Songs song)
        {
            Id = song.Id;
            SpotifyId = song.SpotifyId;
            DiscNumber = song.DiscNumber;
            DurationMs = song.DurationMs;
            Explicit = song.Explicit;
            IsLocal = song.IsLocal;
            Name = song.Name;
            TrackNumber = song.TrackNumber;
            LargeImage = song.LargeImage;
            MediumImage = song.MediumImage;
            SmallImage = song.SmallImage;
            LocalUrl = song.LocalUrl;
            Popularity = song.Popularity;
            IsPlayable = song.IsPlayable;
            LastActiveTime = song.LastActiveTime;
            UploadTime = song.UploadTime;
            Size = song.Size;
            IsPlaying = song.IsPlaying;
            ArtistName = song.ArtistName;
            AlbumName = song.AlbumName;
            AlbumId = song.AlbumId;
            ArtistId = song.ArtistId;
            Type = RvType.Song;
            Images = new List<Image>();
        }

        public void Dispose()
        {
            Images = null;
            SpotifyId = null;
            Name = null;
            LargeImage = null;
            MediumImage = null;
            SmallImage = null;
            LocalUrl = null;
            ArtistName = null;
            AlbumName = null;
        }

        public bool Equals(Songs obj)
        {
            if (Id == obj.Id &&
                Name == obj.Name &&
                IsPlayable == obj.IsPlayable &&
                IsPlaying == obj.IsPlaying)
                return true;
            return false;
        }
    }
}
