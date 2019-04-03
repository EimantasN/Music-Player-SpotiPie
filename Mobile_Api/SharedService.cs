using Mobile_Api.Models;
using RestSharp;
using System;
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

        /// PVZ api/album/Recent
        public async Task<List<T>> GetListAsync<T>(string type)
        {
            CustomRestClient Client = GetClient($"api/{Controller}/" + type);
            return await GetList<T>(Client);
        }

        private async Task<List<T>> GetList<T>(CustomRestClient client)
        {
            try
            {
                return await client.CustomGetList<T>(Method.GET);
            }
            catch (Exception e)
            {
                return new List<T>();
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
            catch (Exception e)
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
