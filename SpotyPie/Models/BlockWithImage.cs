using SpotyPie.Models.Enum;

namespace SpotyPie.Models
{
    public class BlockWithImage
    {
        public int Id { get; set; }
        public RvType Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Image { get; set; }
    }
}