using Mobile_Api.Models;

namespace Mobile_Api
{
    public class BaseService
    {
        private string BaseUrl { get; set; } = "http://pie.pertrauktiestaskas.lt/";

        private ClientGetter Clients { get; set; } = new ClientGetter();

        public CustomRestClient GetClient(string endpoint)
        {
            return Clients.GetClient(BaseUrl + endpoint);
        }
    }
}
