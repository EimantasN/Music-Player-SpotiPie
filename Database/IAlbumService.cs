using Models.FrontEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface IAlbumService
    {
        Task<List<BlockWithImage>> Search(string query);

        Task<BlockWithImage> GetAlbumAsync(int id);

        Task<List<BlockWithImage>> GetAlbumsAsync(int count = 10);

        Task<List<BlockWithImage>> GetRecentAlbumsAsync();

        Task<List<BlockWithImage>> GetPopularAlbumsAsync();

        Task<List<BlockWithImage>> GetOldAlbumsAsync();

        Task<List<BlockWithImage>> GetAlbumsByArtistAsync(int id);
    }
}
