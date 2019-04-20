using Mobile_Api.Models;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public abstract class SharedService : BaseService
    {
        public abstract string Controller { get; set; }

        public async Task<T> Get<T>(int id)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/" + id);
            return await GetData<T>(Client);
        }

        public async Task<T> GetOne<T>(string endPoint)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/" + endPoint);
            return await GetData<T>(Client);
        }

        public async Task Update<T>(int id)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/Update/{id.ToString()}");
            await PostData<T>(Client);
        }

        /// PVZ api/album/Recent
        public async Task<List<T>> GetListAsync<T>(string type)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/{type}");
            return await GetList<T>(Client);
        }

        public async Task<T> Post<T>(string endPoint, string paramerters)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/{endPoint}");
            return await Client.PostCustomObject<T>(paramerters);
        }

        public async Task<List<T>> PostList<T>(string endPoint, string paramerters)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/{endPoint}");
            return await Client.PostCustomObjectList<T>(paramerters);
        }

        private async Task<List<T>> GetList<T>(CustomRestClient client)
        {
            try
            {
                return await client.CustomGetList<T>(Method.GET);
            }
            catch
            {
                return new List<T>();
            }
            finally
            {
                Release(client);
            }
        }

        private async Task PostData<T>(CustomRestClient client)
        {
            try
            {
                await client.CustomGetObject<T>(Method.POST);
            }
            catch
            {
            }
            finally
            {
                Release(client);
            }
        }

        private async Task<T> GetData<T>(CustomRestClient client)
        {
            try
            {
                return await client.CustomGetObject<T>(Method.GET);
            }
            catch
            {
                return default(T);
            }
            finally
            {
                Release(client);
            }
        }
    }
}
