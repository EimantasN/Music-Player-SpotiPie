using Mobile_Api.Enum;
using Mobile_Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public abstract class BaseService : SharedService
    {
        public async Task<List<T>> Search<T>(string query)
        {
            return await PostList<T>(Endpoints.Search, $"query={query}");
        }

        public async Task Report(Exception e)
        {
        }

        public async Task SetState(int songId = 0, int albumId = 0, int artistId = 0, int playlistId = 0)
        {
            await Post("Info", "SetState", $"songId={songId}&artistId={artistId}&albumId={albumId}&playlistId={playlistId}");
        }

        public async Task<dynamic> GetState()
        {
            CustomRestClient Client = GetClient($"api/Info/GetState");
            return await GetData<dynamic>(Client);
        }
    }
}
