using Models.BackEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface IArtistService
    {
        Task<List<Artist>> SearchAsync(string query);

        Task<Artist> GetArtistAsync(int id);

        Task<Artist> UpdateArtist(int id);

        Task<List<Artist>> GetArtistsAsync(int count = int.MaxValue);

        Task<List<Artist>> GetRecentArtistsAsync();

        Task<List<Artist>> GetPopularArtistsAsync();

        Task<List<Artist>> GetOldArtistsAsync();

        Task<List<Song>> GetArtistTopTracksAsync(int id);

        Task<List<Artist>> GetRelatedArtistsAsync(int id);

        Task<List<Album>> GetArtistAlbum(int id);
    }
}
