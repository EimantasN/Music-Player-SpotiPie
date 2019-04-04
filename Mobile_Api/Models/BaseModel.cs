using Mobile_Api.Interfaces;

namespace Mobile_Api
{
    public abstract class BaseModel : IBaseInterface
    {
        public abstract int Id { get; set; }

        public int GetId()
        {
            return Id;
        }
    }
}
