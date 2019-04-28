using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;
using Realms;
using System;

namespace Mobile_Api.Models
{
    public class Songs : RealmObject, IBaseInterface
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

        public RvType Type { get; set; }

        public bool IsPlayingNow() { return IsPlaying; }

        public void SetIsPlaying(bool state) { IsPlaying = state; }

        public Songs()
        {
        }

        public int GetId()
        {
            return Id;
        }

        public RvType GetModelType()
        {
            return Type;
        }

        public void SetModelType(RvType type)
        {
            Type = type;
        }
    }
}
