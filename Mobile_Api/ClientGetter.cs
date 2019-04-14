using Mobile_Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Mobile_Api
{
    public class ClientGetter
    {
        private object _GetterLock { get; set; }
        private object _ReleaseLock { get; set; }

        //Temp Fix
        private static List<Client> _ClientList { get; set; }

        public ClientGetter()
        {
            _GetterLock = new object();
            _ReleaseLock = new object();
            _ClientList = new List<Client>()
            {
                new Client()
            };
        }

        public CustomRestClient GetClient(string url)
        {
            lock (_GetterLock)
            {
                if (_ClientList == null || _ClientList.Count == 0)
                    _ClientList = new List<Client>() { new Client() };

                Client client = _ClientList.FirstOrDefault(x => x.GetState());
                if (client != null)
                {
                    client.LockClient();
                    return client.GetClient(url);
                }
                else
                {
                    client = new Client();
                    client.LockClient();
                    _ClientList.Add(client);
                    return client.GetClient(url);
                }
            }
        }

        public void ReleaseClient(CustomRestClient client)
        {
            lock (_ReleaseLock)
            {
                if (client == null)
                    return;

                if (!(_ClientList == null || _ClientList.Count == 0))
                {
                    _ClientList.First(x => x.GetId() == client.GetId()).ReleaseClient();
                }
            }
        }
    }
}
