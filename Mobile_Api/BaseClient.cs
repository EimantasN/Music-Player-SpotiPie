using Mobile_Api.Models;

namespace Mobile_Api
{
    public class BaseClient
    {
        private string BaseUrl { get; set; } = "https://pie.pertrauktiestaskas.lt/";

        private ClientGetter Clients { get; set; } = new ClientGetter();

        public void Release(CustomRestClient client)
        {
            if (client != null)
                Clients.ReleaseClient(client);
        }

        public CustomRestClient GetClient(string endpoint)
        {
            return Clients.GetClient(BaseUrl + endpoint);
        }
    }
}
