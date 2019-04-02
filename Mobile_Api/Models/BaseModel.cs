namespace Mobile_Api.Models
{
    public class BaseModel<T>
    {
        public string GetCurrentType()
        {
            return typeof(T).ToString();
        }
    }
}
