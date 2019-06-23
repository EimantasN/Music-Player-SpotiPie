using Mobile_Api.Models.Enums;

namespace Mobile_Api.Interfaces
{
    public interface IBaseInterface<T>
    {
        int GetId();

        void SetModelType(RvType type);

        bool Equals(T obj);
    }
}
