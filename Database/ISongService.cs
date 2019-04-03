using Models.FrontEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface ISongService
    {
        Task<List<Song>> Search(string query);

        Task<List<Song>> GetAllAsync();

        Task<Song> GetAsync(int id);

        Task Remove(int id);

        Task GetRecent(int count = 10);

        Task GetSongsByAlbum(int albumId);

        Task UpdateAsync(int id);
    }
}
