using Android.App;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class API
    {
        private Mobile_Api.Service _service;
        private MainActivity _activity;

        private List<Album> _Albums { get; set; }
        private List<Artist> _Artists { get; set; }
        private List<Songs> _Songs { get; set; }

        #region Locks
        private Object _album_Lock { get; } = new Object();
        private Object _artist_Lock { get; } = new Object();
        private Object _song_Lock { get; } = new Object();

        #endregion

        public API(Mobile_Api.Service service, MainActivity activity)
        {
            _service = service;
            _activity = activity;
        }

        #region Library

        public async Task GetAll<T>(RvList<T> RvList, Action action, RvType type) where T : IBaseInterface
        {
            try
            {
                List<T> AlbumsData = new List<T>() { default(T) };
                InvokeOnMainThread(() => RvList.AddList(AlbumsData));

                AlbumsData = await _service.GetAll<T>();

                AlbumsData.ForEach(x => x.SetModelType(type));

                InvokeOnMainThread(() =>
                {
                    RvList.AddList(AlbumsData);
                    if (action != null)
                    {
                        action.Invoke();
                    }
                });
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region MainFragment

        public async Task GetRecentAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetRecent<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPolularAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetPopular<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetOldAlbumsAsync(RvList<Album> RvList, Action action)
        {
            try
            {
                List<Album> AlbumData = await _service.GetOld<Album>();
                InvokeOnMainThread(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                    RvList.AddList(AlbumData);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region AlbumView

        internal async Task GetSongsByAlbumAsync(Album currentALbum, RvList<Songs> rvData, Action p)
        {
            try
            {
                if (currentALbum == null)
                    throw new Exception("Album can't be null");
                List<Songs> songs = await GetSongsByAlbumAsync(currentALbum);
                InvokeOnMainThread(() =>
                {
                    rvData.AddList(songs);
                    if (p != null)
                    {
                        p.Invoke();
                    }
                });
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region Getters
        private async Task<List<Songs>> GetSongsByAlbumAsync(Album al)
        {
            if (_Albums == null)
                _Albums = new List<Album>() { al };

            if (al.Songs != null && al.Songs.Count != 0)
                return al.Songs;
            else
            {
                al = await GetAlbumByIdAsync(al.Id);
                return al.Songs;
            }
        }

        private async Task<Album> GetAlbumByIdAsync(int id)
        {
            Album al = await _service.GetById<Album>(id);
            return UpdateAlbums(al);
        }

        private async Task<List<Songs>> GetSongsByArtistAsync(Album al)
        {
            //if (_Artists == null)
            //    _Artists = new List<Artist>() { al };

            //if (al.Songs != null && al.Songs.Count != 0)
            //    return al.Songs;
            //else
            //{
            //    al = await GetAlbumByIdAsync(al.Id);
            //    return al.Songs;
            //}
            return null;
        }

        private async Task<Artist> GetArtistByIdAsync(int id)
        {
            Artist ar = await _service.GetById<Artist>(id);
            lock (_artist_Lock)
            {
                if (_Artists == null)
                {
                    _Artists = new List<Artist>() { ar };
                    return ar;
                }
                else
                {
                    if (!_Artists.Any(x => x.Id == ar.Id))
                    {
                        _Artists.Add(ar);
                        return ar;
                    }
                    else
                    {
                        return ar;
                    }
                }
            }
        }

        private Album UpdateAlbums(Album al)
        {
            lock (_album_Lock)
            {
                if (_Albums == null)
                {
                    _Albums = new List<Album>() { al };
                    return al;
                }
                else
                {
                    if (!_Albums.Any(x => x.Id == al.Id))
                    {
                        _Albums.Add(al);
                        return al;
                    }
                    else
                    {
                        return al;
                    }
                }
            }
        }
        #endregion

        #region Setters
        public void UpdateSongs(int id)
        {
            if (_Songs == null && _Songs.Count == 0) return;

            Songs song = _Songs.FirstOrDefault(x => x.Id == id);
            song.Popularity++;

        }
        #endregion

        #region Updates
        internal async Task UpdateSongPopularity(int id)
        {
            try
            {
                await _service.Update<Songs>(id);
                UpdateSongs(id);
            }
            catch (Exception e)
            {

            }
        }
        #endregion

        private void InvokeOnMainThread(Action action)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                action.Invoke();
            }, null);
        }
    }
}