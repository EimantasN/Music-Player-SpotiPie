using Android.Support.V4.App;
using Android.Support.V7.App;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using Realms;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrentSong = Mobile_Api.Models.Realm.Music;

namespace SpotyPie
{
    public class API
    {
        private static Object _currentStateGetter = new Object();

        private bool GettingState { get; set; } = false;

        private dynamic State { get; set; }

        private DateTime StateGettinTime { get; set; }

        private Mobile_Api.Service _service;

        private AppCompatActivity _activity;

        private List<Album> _Albums { get; set; }

        private List<Artist> _Artists { get; set; }

        private List<Songs> _Songs { get; set; }

        #region Locks

        private Object _album_Lock { get; } = new Object();
        private Object _artist_Lock { get; } = new Object();
        private Object _song_Lock { get; } = new Object();

        #endregion

        public void SetState(int songId)
        {
            Task.Run(() => _service.SetState(songId));
        }

        public void Report(Exception e)
        {
            Task.Run(() => _service.Report(e));
        }
        public void Corrupted(int songId)
        {
            Task.Run(() => _service.Corruped(songId));
        }

        public async Task<Songs> GetNextSongAsync(int? songId = null)
        {
            return await _service.GetNextSong(songId);
        }

        private Songs UpdateCurrentSongList(Songs song)
        {
            Task.Run(() =>
            {
                using (var realm = Realm.GetInstance())
                {
                    Realm_Songs old = realm.All<Realm_Songs>().AsQueryable()
                    .FirstOrDefault(x => x.IsPlaying == true);
                    Realm_Songs real_song = new Realm_Songs(song);
                    real_song.IsPlaying = true;
                    realm.Write(() =>
                    {
                        if (old != null)
                            old.IsPlaying = false;
                        realm.Add(real_song);
                    });
                }
            });
            return song;
        }

        public API(Mobile_Api.Service service, AppCompatActivity activity)
        {
            _service = service;
            _activity = activity;
            TryToGetRealm();
        }

        public API()
        {
            _service = new Mobile_Api.Service();
            TryToGetRealm();
        }

        private void TryToGetRealm()
        {
            try
            {
                Realm realm = Realm.GetInstance();
                realm.Dispose();
            }
            catch (Exception) //Exeption is raised because migration was make and old realm can't be used
            {
                //Recreate real if real class changed
                Realm.DeleteRealm(RealmConfiguration.DefaultConfiguration);
                Realm realm = Realm.GetInstance();
                realm.Dispose();
            }
        }

        #region Current State

        private async Task GetCurrentState()
        {
            GettingState = true;
            StateGettinTime = DateTime.Now;
            State = await _service.GetState();
        }

        public async Task<Songs> GetCurrentSong()
        {
            await GetCurrentState();
            return await GetSongAsync(854);//(int)State.songId);
        }

        public async Task<dynamic> GetSystemInfo()
        {
            return await _service.Get<dynamic>("Info", "SystemInformation");
        }

        #endregion

        #region Library

        public async Task GetAll<T>(RvList<T> RvList, Action action, RvType type) where T : IBaseInterface<T>
        {
                List<T> AlbumsData = new List<T>() { default(T) };
                RvList.AddList(AlbumsData);

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

        internal async Task<string> DeleteSongAsync(string filePath)
        {
            return await _service.DeleteSongAsync(filePath);
        }

        #endregion

        #region MainFragment

        public async Task<List<Album>> GetOldAlbumsAsync()
        {
                List<Album> AlbumData = await _service.GetOld<Album>();
                using (Realm realm = Realm.GetInstance())
                {
                    Realm_Album temp;
                    foreach (var newAblum in AlbumData)
                    {
                        temp = realm.All<Realm_Album>().FirstOrDefault(x => x.Id == newAblum.Id);
                        if (temp == null)
                        {
                            realm.Write(() => { realm.Add(new Realm_Album(newAblum, 3)); });
                        }
                        else
                        {
                            realm.Write(() =>
                            {
                                temp.SpotifyId = newAblum.SpotifyId;
                                temp.LargeImage = newAblum.LargeImage;
                                temp.MediumImage = newAblum.MediumImage;
                                temp.SmallImage = newAblum.SmallImage;
                                temp.Name = newAblum.Name;
                                temp.ReleaseDate = newAblum.ReleaseDate;
                                temp.Popularity = newAblum.Popularity;
                                temp.IsPlayable = newAblum.IsPlayable;
                                temp.LastActiveTime = newAblum.LastActiveTime;
                                temp.Tracks = newAblum.Tracks;
                                temp.Type = (int)newAblum.GetModelType();
                                temp.AlbumListType += 3;
                                temp.Update(newAblum, 3);
                            });
                        }
                    }
                }
                return AlbumData;
        }

        public async Task<List<Album>> GetRecentAlbumsAsync()
        {
                List<Album> AlbumData = await _service.GetRecent<Album>();
                using (Realm realm = Realm.GetInstance())
                {
                    Realm_Album temp;
                    foreach (var newAblum in AlbumData)
                    {
                        temp = realm.All<Realm_Album>().FirstOrDefault(x => x.Id == newAblum.Id);
                        if (temp == null)
                        {
                            realm.Write(() => { realm.Add(new Realm_Album(newAblum, 1)); });
                        }
                        else
                        {
                            realm.Write(() => { temp.Update(newAblum, 1); });
                        }
                    }
                }
                return AlbumData;
        }

        public async Task<List<Album>> GetPolularAlbumsAsync()
        {
                List<Album> AlbumData = await _service.GetPopular<Album>();
                using (Realm realm = Realm.GetInstance())
                {
                    Realm_Album temp;
                    foreach (var newAblum in AlbumData)
                    {
                        temp = realm.All<Realm_Album>().FirstOrDefault(x => x.Id == newAblum.Id);
                        if (temp == null)
                        {
                            realm.Write(() =>
                            {
                                realm.Add(new Realm_Album(newAblum, 2));
                            });
                        }
                        else
                        {
                            realm.Write(() =>
                            {
                                temp.Update(newAblum, 2);
                            });
                        }
                    }
                }
                return AlbumData;
        }

        internal async Task<dynamic> GetBindedStatisticsAsync()
        {
            return await _service.GetBindedStatisticsAsync();
        }

        internal async Task SetSongDurationAsync(int id, int duration)
        {
            await _service.SetSongDuration(id, duration);
        }

        internal async Task UpdateAsync<T>(int id)
        {
            await _service.Update<T>(id);
        }

        #endregion

        #region AlbumView

        internal async Task<List<Songs>> GetSongsByAlbumAsync(Album currentALbum)
        {
            try
            {
                if (currentALbum == null)
                    throw new Exception("Album can't be null");

                Album album = await GetAlbumByIdAsync(currentALbum.Id);
                //using (Realm realm = Realm.GetInstance())
                //{

                //    var a = realm.All<Realm_Songs>().AsQueryable().ToList().Count;

                //    Realm_Songs temp;
                //    foreach (var song in album.Songs)
                //    {
                //        temp = realm.All<Realm_Songs>().FirstOrDefault(x => x.Id == song.Id);
                //        if (temp == null)
                //        {
                //            realm.Write(() =>
                //            {
                //                realm.Add(new Realm_Songs(song));
                //            });
                //        }
                //        else
                //        {
                //            realm.Write(() =>
                //            {
                //                temp.Update(song);
                //            });
                //        }
                //    }
                //}
                return album.Songs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal async Task<List<Artist>> GetRelatedAsync(int id)
        {
            return await _service.GetRelated(id);
        }

        internal async Task<List<Album>> GetArtistAlbumsAsync(int id)
        {
            return await _service.GetArtistAlbums(id);
        }

        internal async Task<List<Songs>> GetTopTrackByArtistIdAsync(int id)
        {
            return await _service.GetTopTrackByArtistId(id);
        }

        internal async Task<List<Songs>> GetArtistSongsAsync(Artist artist)
        {
            try
            {
                if (artist == null)
                    throw new Exception("Artist can't be null");
                List<Songs> songs = await _service.GetSongsByArtistAsync(artist);
                return songs;
            }
            catch (Exception e)
            {
                return new List<Songs>();
            }
        }

        #endregion

        #region Getters

        private async Task<Album> GetAlbumByIdAsync(int id)
        {
            Album al = await _service.GetById<Album>(id);
            return UpdateAlbums(al);
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

        private async Task<Songs> GetSongAsync(int id)
        {
            Songs song;
            if (_Songs == null) _Songs = new List<Songs>();
            if (_Songs.Count > 0)
            {
                song = _Songs.FirstOrDefault(x => x.Id == id);
                if (song != null)
                    return song;
            }

            song = await _service.Get<Songs>(id);
            if (song != null)
            {
                _Songs.Add(song);
                return song;
            }

            throw new Exception($"Can't find song with id -> {id}");
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
                await _service.Update<Songs>(id);
                UpdateSongs(id);
        }

        internal async Task SongCorrupted(int id)
        {
                await _service.Corruped(id);
        }
        #endregion

        public async Task<List<Songs>> GetSongToBind(string songTitle, int songCof, string album, int albumCof, string artist, int artistCof)
        {
            return await _service.GetSongToBind(songTitle, songCof, album, albumCof, artist, artistCof);
        }

        public async Task<List<SongTag>> GetUnbindedSongList()
        {
            return await _service.GetListAsync<SongTag>("UnbindedSongs", "Songs");
        }

        public void SetCurrentList(List<Songs> songs)
        {
                //TODO ADD MODEL CASTING
                var realm = Realm.GetInstance();
                songs.ForEach(x =>
                {
                    realm.Write(() =>
                    {
                        //realm.Add<Realm_Songs>(x);
                    });
                });
        }

        public List<Songs> GetCurrentList()
        {
                //TODO add song list getting from database
                return new List<Songs>();
        }

        public List<Songs> GetCurrentListLive()
        {
            try
            {
                return _Songs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void InvokeOnMainThread(Action action)
        {
            _activity.RunOnUiThread(() =>
            {
                action.Invoke();
            });
        }

        internal async Task<List<Image>> GetNewImageForSongAsync(int id)
        {
            return await _service.GetNewImageForSong(id);
        }

        internal async Task<List<T>> SearchAsync<T>(string query) where T : IBaseInterface<T>
        {
            return await _service.Search<T>(query);
        }
    }
}