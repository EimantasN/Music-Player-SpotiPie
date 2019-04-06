using Models.BackEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface IAlbumService
    {
        Task<List<Album>> Search(string query);

        Task<Album> GetAlbumAsync(int id);

        Task<List<Album>> GetAlbumsAsync(int count = int.MaxValue);

        Task<List<Album>> GetRecentAlbumsAsync();

        Task<List<Album>> GetPopularAlbumsAsync();

        Task<List<Album>> GetOldAlbumsAsync();

        Task<List<Album>> GetAlbumsByArtistAsync(int id);
    }
}
