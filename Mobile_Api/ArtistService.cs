using Mobile_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class ArtistService : SharedService
    {
        public override string Controller { get; set; } = "Artist";

        public async Task<Artist> GetSong(int id)
        {
            return await Get<Artist>(id);
        }

        public async Task<List<Artist>> GetAll()
        {
            return await GetListAsync<Artist>("Artists");
        }

        public async Task<List<Artist>> GetPopular()
        {
            return await GetListAsync<Artist>("Popular");
        }

        public async Task<Artist> GetWithAlbums(int id)
        {
            return await GetOne<Artist>(id.ToString() + "/Albums");
        }

        public async Task<List<Artist>> GetRelated(int id)
        {
            return await GetListAsync<Artist>("Related/" + id);
        }
    }
}