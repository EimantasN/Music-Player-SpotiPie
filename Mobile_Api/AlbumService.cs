using Mobile_Api.Models.Rv;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class AlbumService : SharedService
    {
        public override string Controller { get; set; } = "Album";

        public async Task<BlockWithImage> GetSong(int id)
        {
            return await Get<BlockWithImage>(id);
        }

        public async Task<List<BlockWithImage>> GetSongs()
        {
            return await GetListAsync<BlockWithImage>("Albums");
        }

        public async Task<List<BlockWithImage>> GetRecent()
        {
            return await GetListAsync<BlockWithImage>("Recent");
        }

        public async Task<List<BlockWithImage>> GetPopular()
        {
            return await GetListAsync<BlockWithImage>("Popular");
        }

        public async Task<List<BlockWithImage>> GetOld()
        {
            return await GetListAsync<BlockWithImage>("Old");
        }
    }
}
