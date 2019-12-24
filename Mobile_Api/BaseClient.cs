using Mobile_Api.Models;

namespace Mobile_Api
{
    public class BaseClient
    {
        public static string BaseUrl { get; set; } = "http://spotypie.endev.lt/";

        private static ClientGetter Clients { get; set; } = new ClientGetter();

        private static object _releaseLock { get; set; } = new object();

        private static object _getLock { get; set; } = new object();

        public void Release(CustomRestClient client)
        {
            lock (_releaseLock)
            {
                if (client != null)
                    Clients.ReleaseClient(client);
            }
        }

        public CustomRestClient GetClient(string endpoint)
        {
            lock (_getLock)
            {
                return Clients.GetClient(BaseUrl + endpoint);
            }
        }
    }
}
