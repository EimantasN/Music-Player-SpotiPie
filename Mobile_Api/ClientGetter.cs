using Mobile_Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Mobile_Api
{
    public class ClientGetter
    {
        private List<Client> _ClientList { get; set; }

        public ClientGetter()
        {
            _ClientList = new List<Client>()
            {
                new Client()
            };
        }

        public CustomRestClient GetClient(string url)
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

        public void ReleaseClient(CustomRestClient client)
        {
            if (client == null)
                return;

            if (!(_ClientList == null || _ClientList.Count == 0))
            {
                _ClientList.FirstOrDefault(x => x.GetId() == client.GetId())?.ReleaseClient();
            }
        }
    }
}
