using Mobile_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class AlbumService : SharedService
    {
        public override string Controller { get; set; } = "Album";

        public async Task<Album> GetSong(int id)
        {
            return await Get<Album>(id);
        }

        public async Task<List<Album>> GetAll()
        {
            return await GetListAsync<Album>("Albums");
        }

        public async Task<List<Album>> GetRecent()
        {
            return await GetListAsync<Album>("Recent");
        }

        public async Task<List<Album>> GetPopular()
        {
            return await GetListAsync<Album>("Popular");
        }

        public async Task<List<Album>> GetOld()
        {
            return await GetListAsync<Album>("Old");
        }

        public async Task Update(int id)
        {
            await Update<Album>(id);
        }
    }
}
