using Mobile_Api.Models.Rv;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class SongService : SharedService
    {
        public override string Controller { get; set; } = "Songs";

        public async Task<Song> GetSong(int id)
        {
            return await Get<Song>(id);
        }

        public async Task<List<Song>> GetSongs()
        {
            return await GetListAsync<Song>("Songs");
        }

        public async Task<List<Song>> GetRecent()
        {
            return await GetListAsync<Song>("Recent");
        }

        public async Task<List<Song>> GetPopular()
        {
            return await GetListAsync<Song>("Popular");
        }
    }
}
