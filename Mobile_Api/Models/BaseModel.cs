using Mobile_Api.Interfaces;
using Mobile_Api.Models.Enums;

namespace Mobile_Api.Models
{
    public abstract class BaseModel : IBaseInterface
    {
        public abstract int Id { get; set; }

        protected abstract RvType Type { get; set; }

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
