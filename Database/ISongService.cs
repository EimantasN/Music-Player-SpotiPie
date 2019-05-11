using Microsoft.AspNetCore.Http;
using Models.BackEnd;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Database
{
    public interface ISongService
    {
        Task SetStateAsync(int songId, int artistId, int albumId, int playlistId);

        Task<CurrentSong> GetState();

        Task<Song> GetNextSong();

        Task<List<Song>> SearchAsync(string query);

        Task<List<Song>> GetAllAsync(int count = 30);

        Task<Song> GetAsync(int id);

        Task RemoveAsync(int id);

        Task<List<Song>> GetRecentAsync(int count = 10);

        Task<List<Song>> GetPopularAsync(int count = 10);

        Task<List<Song>> GetSongsByAlbumAsync(int albumId);

        Task<Song> UpdateAsync(int id);

        Task<List<Song>> SongByArtistId(int artistId);

        Task<List<AudioBindError>> BindData();

        Task<List<AudioBindError>> AddAudioToLibrary(IFormFile file);

        Task<Song> SetLenght(int id, long lenght);

        Task SetCorruptedAsync(int id);

        Task<List<Image>> GetNewImageAsync(int id);

        List<SongTag> UnbindedSongs();

        Task<List<Song>> GetSongToBindAsync(string songTitle, int songCof, string album, int albumCof, string artist, int artistCof);

        Task<dynamic> GetBindingStatistics();

        string DeleteLocalSongFile(string localUrl);

        Task<Song> BindSongWithFileAsync(string localUrl, int songId);
    }
}
