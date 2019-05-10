using Mobile_Api.Models.Enums;

namespace Mobile_Api.Models
{
    public class SongTag : BaseModel
    {
        public override int Id { get; set; }

        public string FilePath { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public int Bitrate { get; set; }

        public string Gendre { get; set; }

        public int TrackNumber { get; set; }

        protected override RvType Type { get; set; } = RvType.SongBindList;
    }
}
