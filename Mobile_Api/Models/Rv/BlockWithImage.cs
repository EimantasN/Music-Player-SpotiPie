using Mobile_Api.Models.Enums;

namespace Mobile_Api.Models.Rv
{
    public class BlockWithImage : BaseModel
    {
        public override int Id { get; set; }
        public RvType Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Image { get; set; }
    }
}
