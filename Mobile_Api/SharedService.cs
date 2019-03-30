using Mobile_Api.Models;
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
        public async Task<List<Album>> GetAlbumsAsync(AlbumType type)
        {
            try
            {
                return await GetClient("api/album/" + type).CustomExecuteTaskAsync<Album>(Method.GET);
            }
            catch (Exception e)
            {
                return new List<Album>();
            }
        }
    }
}
