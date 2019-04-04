using Mobile_Api;
using Mobile_Api.Models.Rv;

namespace SpotyPie.Models
{
    public class TwoBlockWithImage : BaseModel
    {
        public override int Id { get; set; } = -1;
        public bool Full { get; set; }
        public BlockWithImage Left { get; set; }
        public BlockWithImage Right { get; set; }
    }
}