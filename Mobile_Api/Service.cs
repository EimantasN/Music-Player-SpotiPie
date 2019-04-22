using Mobile_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api
{
    public class Service : BaseService
    {
        public async Task<T> GetById<T>(int id)
        {
            return await Get<T>(id);
        }

        public async Task<List<T>> GetAll<T>()
        {
            return await GetListAsync<T>("All");
        }

        public async Task<List<T>> GetRecent<T>()
        {
            return await GetListAsync<T>("Recent");
        }

        public async Task<List<T>> GetPopular<T>()
        {
            return await GetListAsync<T>("Popular");
        }

        public async Task<List<T>> GetOld<T>()
        {
            return await GetListAsync<T>("Old");
        }

        #region ARTIST
        public async Task<Artist> GetWithAlbums(int id)
        {
            return await GetOne<Artist>($"{id.ToString()}/Albums");
        }
        public async Task<List<Artist>> GetRelated(int id)
        {
            return await GetListAsync<Artist>($"Related/{id}");
        }

        public async Task<List<Songs>> GetTopTrackByArtistId(int id)
        {
            return await GetListAsync<Songs>($"{id}/top-tracks", "Artist");
        }

        public async Task<List<Album>> GetArtistAlbums(int id)
        {
            return await GetListAsync<Album>($"Albums/{id}", "Artist");
        }

        #endregion

        #region ALBUM
        public async Task Update(int id)
        {
            await Update<Album>(id);
        }
        #endregion

        #region SONG
        public async Task<Songs> SetSongDuration(int id, int duration)
        {
            return await Post<Songs>("SetSongLenght", $"id={id}&lenght={duration}");
        }
        #endregion
    }
}
