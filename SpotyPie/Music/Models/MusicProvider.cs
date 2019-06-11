using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Mobile_Api.Models;
using Realms;

namespace SpotyPie.Music.Models
{
    public class MusicProvider
    {
        private API API { get; set; }

        private Songs CurrentSong { get; set; } = new Songs();

        enum State
        {
            NonInitialized,
            Initializing,
            Initialized
        };

        private API GetApiService()
        {
            if (API == null)
                API = new API();
            return API;
        }

        volatile State currentState = State.NonInitialized;

        public MusicProvider()
        {
        }

        public string GetCurrentSong()
        {
            Realm realm = Realm.GetInstance();
            var song = realm.All<Realm_Songs>().FirstOrDefault(x => x.IsPlaying == true);
            if (song == null)
            {
                song = realm.All<Realm_Songs>().FirstOrDefault();
                if (CurrentSong == null)
                {
                    CurrentSong = new Songs();
                }
                CurrentSong = new Songs(song);
                BuildMetadata();
            }
            else
            {
                CurrentSong = new Songs(song);
                BuildMetadata();
            }
            realm.Dispose();
            return CurrentSong.Id.ToString();
        }

        public string GetCurrentSongId => CurrentSong.Id.ToString();

        public async Task GetNextSongAsync()
        {
            BuildMetadata();
        }

        public void SongPaused()
        {
            GetApiService().SongPaused();
        }

        public void SongResumed()
        {
            GetApiService().SongResumed();
        }

        public string CurrentSongSource()
        {
            return $"https://pie.pertrauktiestaskas.lt/api/stream/play/{GetCurrentSong()}";
        }

        public void SetFavorite(string musicId, bool favorite)
        {

        }

        public async Task ChangeSongAsync(bool Foward)
        {
            Realm realm = Realm.GetInstance();
            try
            {
                var Current_Song_List = realm.All<Realm_Songs>().AsQueryable().ToList();
                if (Current_Song_List == null) return;

                for (int i = 0; i < Current_Song_List.Count; i++)
                {
                    if (Current_Song_List[i].IsPlaying)
                    {
                        UpdateCurrentSong(Current_Song_List[i], false);
                        if (Foward)
                        {
                            if ((i + 1) == Current_Song_List.Count)
                            {
                                var r_song = await SongListEndCheckForAutoPlayAsync();

                                using (Realm innerRealm = Realm.GetInstance())
                                {
                                    innerRealm.Write(() =>
                                    {
                                        r_song.IsPlaying = true;
                                        innerRealm.Add<Realm_Songs>(r_song);
                                    });
                                }
                            }
                            else
                            {
                                UpdateCurrentSong(Current_Song_List[i + 1]);
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                UpdateCurrentSong(Current_Song_List[0]);
                            }
                            else
                            {
                                UpdateCurrentSong(Current_Song_List[i - 1]);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                realm.Dispose();
                BuildMetadata();
            }

            void UpdateCurrentSong(Realm_Songs song, bool status = true)
            {
                if (realm != null)
                {
                    realm.Write(() =>
                    {
                        song.IsPlaying = status;
                    });
                }
            }
        }

        private async Task<Realm_Songs> SongListEndCheckForAutoPlayAsync()
        {
            try
            {
                Songs song = await GetApiService().GetNextSongAsync();
                song.SetModelType(Mobile_Api.Models.Enums.RvType.Song);
                song.IsPlaying = true;
                return new Realm_Songs(song);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool IsFavorite(string musicId)
        {
            return false;
        }

        public bool IsInitialized
        {
            get
            {
                return currentState == State.Initialized;
            }
        }

        public async Task RetrieveMediaAsync(Action<bool> callback)
        {
            if (currentState == State.Initialized)
            {
                callback(true);
                return;
            }

            await RetrieveMediaAsync();
            callback(currentState == State.Initialized);
        }

        async Task RetrieveMediaAsync()
        {
            try
            {
                if (currentState == State.NonInitialized)
                {
                    currentState = State.Initializing;

                    currentState = State.Initialized;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (currentState != State.Initialized)
                    currentState = State.NonInitialized;
            }
        }

        private MediaMetadataCompat MetaData;

        public MediaMetadataCompat GetMetadata() => MetaData;

        public void BuildMetadata()
        {
            MetaData = new MediaMetadataCompat.Builder()
                .PutString(MediaMetadataCompat.MetadataKeyMediaId, CurrentSong.Id.ToString())
                .PutString(MediaMetadataCompat.MetadataKeyAlbum, CurrentSong.AlbumName)
                .PutString(MediaMetadataCompat.MetadataKeyArtist, CurrentSong.ArtistName)
                .PutLong(MediaMetadataCompat.MetadataKeyDuration, CurrentSong.DurationMs)
                .PutString(MediaMetadataCompat.MetadataKeyGenre, "Rock")
                .PutString(MediaMetadataCompat.MetadataKeyAlbumArtUri, CurrentSong.MediumImage)
                .PutString(MediaMetadataCompat.MetadataKeyTitle, CurrentSong.Name)
                .PutLong(MediaMetadataCompat.MetadataKeyTrackNumber, CurrentSong.TrackNumber)
                .PutLong(MediaMetadataCompat.MetadataKeyNumTracks, 10)
                .PutLong(MediaMetadataCompat.MetadataKeyDiscNumber, CurrentSong.DiscNumber)
                .PutString("SpotyPieImgUrl", CurrentSong.MediumImage)
                .Build();
        }

        internal List<string> GetCurrentSongListGendres()
        {
            return new List<string>() { "moder rock", "rock", "alternative rock" };
        }

        public IEnumerable<MediaMetadataCompat> GetCurrentSongList()
        {
            IEnumerable<MediaMetadataCompat> tracks = new List<MediaMetadataCompat>()
            {
                GetMetadata(),
                GetMetadata(),
                GetMetadata(),
                GetMetadata(),
                GetMetadata(),
                GetMetadata(),
                GetMetadata(),
            };
            return tracks;
        }
    }
}