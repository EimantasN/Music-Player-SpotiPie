using System;

namespace Mobile_Api.Models
{
    public class Song : BaseModel
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

        public DateTime LastActiveTime { get; set; }

        public DateTime UploadTime { get; set; }

        public long Size { get; set; }

        public bool IsPlaying { get; set; }

        public bool IsPlayingNow() { return IsPlaying; }
        public void SetIsPlaying(bool state) { IsPlaying = state; }

        public Song(bool fake)
        {
            Id = 10;
            Name = "Testas";
            LocalUrl = "";
            LargeImage = "";
        }
    }
}
