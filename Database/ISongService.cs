using Microsoft.AspNetCore.Http;
using Models.BackEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface ISongService
    {
        Task<List<Song>> SearchAsync(string query);

        Task<List<Song>> GetAllAsync(int count = 30);

        Task<Song> GetAsync(int id);

        Task RemoveAsync(int id);

        Task<List<Song>> GetRecentAsync(int count = 10);

        Task<List<Song>> GetPopularAsync(int count = 10);

        Task<List<Song>> GetSongsByAlbumAsync(int albumId);

        Task UpdateAsync(int id);

        Task<List<AudioBindError>> BindData();
        Task<List<AudioBindError>> AddAudioToLibrary(IFormFile file);
    }
}
