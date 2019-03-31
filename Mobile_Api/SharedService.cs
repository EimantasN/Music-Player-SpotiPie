using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class SharedService : BaseService
    {

        ///api/album/Recent
        public async Task<List<T>> GetListAsync<T>(AlbumType type)
        {
            CustomRestClient Client = GetClient("api/album/" + type);
            return await GetList<T>(Client);
        }

        public async Task<List<T>> GetListAsync<T>(ArtistType type)
        {
            CustomRestClient Client = GetClient("api/artist/" + type);
            return await GetList<T>(Client);
        }

        public async Task<List<T>> GetListAsync<T>(PlaylistType type)
        {
            CustomRestClient Client = GetClient("api/playlist/" + type);
            return await GetList<T>(Client);
        }

        private async Task<List<T>> GetList<T>(CustomRestClient client)
        {
            try
            {
                return await client.CustomExecuteTaskAsync<T>(Method.GET);
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
    }
}
