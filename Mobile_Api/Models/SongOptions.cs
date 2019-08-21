using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;

namespace Mobile_Api.Models
{
    public class SongOptions : BaseModel, IBaseInterface<SongOptions>
    {
        public override int Id { get; set; }

        public Enums.SongOptions ItemType { get; set; }

        public string Value { get; set; }

        protected override RvType Type { get; set; } = RvType.SongOptions;

        public bool Equals(SongOptions obj)
        {
            if (Id == obj.Id)
                return true;
            return false;
        }
    }
}
