using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Mobile_Api.Models;
using Mobile_Api.Models.Realm;
using Realms;
using CurrentMusic = Mobile_Api.Models.Realm.Music;

namespace SpotyPie.Music.Models
{
    public class MusicProvider
    {
        //Current playing song id
        private string Id { get; set; }

        private API API { get; set; }

        private API GetApiService()
        {
            if (API == null)
                API = new API();
            return API;
        }

        volatile Enums.State currentState = Enums.State.NonInitialized;

        public MusicProvider()
        {
            ResetPlayingList();
        }

        private void ResetPlayingList()
        {
            using (Realm realm = Realm.GetInstance())
            {
                var songs = realm.All<CurrentMusic>();
                foreach (var x in songs)
                {
                    realm.Write(() => { x.IsPlaying = false; x.Song.IsPlaying = false; });
                }
            }
        }

        public string GetCurrentSong()
        {
            using (Realm realm = Realm.GetInstance())
            {
                var song = realm.All<CurrentMusic>().FirstOrDefault(x => x.IsPlaying);
                if (song != null)
                    return Id = $"{song.Song.Id}";
                else
                {
                    var songs = realm.All<CurrentMusic>().ToList();

                    return $"{songs[0].Id}";
                }
            }
        }

        public string GetCurrentSongId => Id;

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
            return $"https://pie.pertrauktiestaskas.lt/api/stream/play/{Id}";
        }

        public void SetFavorite(string musicId, bool favorite)
        {
        }

        public async Task ChangeSongAsync(bool Foward)
        {
            using (Realm realm = Realm.GetInstance())
            {
                try
                {
                    List<CurrentMusic> Songs = realm.All<CurrentMusic>().ToList();
                    if (Songs == null || Songs.Count == 0) return;

                    for (int i = 0; i < Songs.Count; i++)
                    {
                        if (Songs[i].IsPlaying)
                        {
                            UpdateCurrentSong(Songs[i], false);
                            if (Foward)
                            {
                                if ((i + 1) == Songs.Count)
                                {
                                    await NextSongFromServerAsync();
                                }
                                else
                                {
                                    UpdateCurrentSong(Songs[i + 1]);
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    UpdateCurrentSong(Songs[0]);
                                }
                                else
                                {
                                    UpdateCurrentSong(Songs[i - 1]);
                                }
                            }
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                }

                void UpdateCurrentSong(CurrentMusic song, bool status = true)
                {
                    realm.Write(() =>
                    {
                        song.IsPlaying = status;
                        song.Song.IsPlaying = status;
                        song.Song.LastActiveTime = DateTime.Now;
                    });
                }
            }
        }

        private async Task NextSongFromServerAsync()
        {
            Realm_Songs nextSong = await GetApiService().GetNextSongAsync();
            using (Realm innerRealm = Realm.GetInstance())
            {
                innerRealm.Write(() =>
                {
                    innerRealm.Add(new CurrentMusic(nextSong, true), true);
                });
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
                return currentState == Enums.State.Initialized;
            }
        }

        public void RetrieveMedia(Action<bool> callback)
        {
            if (currentState == Enums.State.Initialized)
            {
                callback(true);
                return;
            }

            RetrieveMedia();
            callback(currentState == Enums.State.Initialized);
        }

        void RetrieveMedia()
        {
            try
            {
                if (currentState == Enums.State.NonInitialized)
                {
                    currentState = Enums.State.Initializing;

                    currentState = Enums.State.Initialized;
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (currentState != Enums.State.Initialized)
                    currentState = Enums.State.NonInitialized;
            }
        }

        private MediaMetadataCompat MetaData;

        public MediaMetadataCompat GetMetadata() => MetaData;

        public void BuildMetadata()
        {
            using (var realm = Realm.GetInstance())
            {
                var song = realm.All<CurrentMusic>().FirstOrDefault(x => x.IsPlaying == true);
                if (song == null)
                    throw new Exception("Inconsistency detected");
                else
                {
                    var Currentsong = new Songs(song.Song);
                    MetaData = new MediaMetadataCompat.Builder()
                        .PutString(MediaMetadataCompat.MetadataKeyMediaId, Currentsong.Id.ToString())
                        .PutString(MediaMetadataCompat.MetadataKeyAlbum, Currentsong.AlbumName)
                        .PutString(MediaMetadataCompat.MetadataKeyArtist, Currentsong.ArtistName)
                        .PutLong(MediaMetadataCompat.MetadataKeyDuration, Currentsong.DurationMs)
                        .PutString(MediaMetadataCompat.MetadataKeyGenre, "Rock")
                        .PutString(MediaMetadataCompat.MetadataKeyAlbumArtUri, Currentsong.MediumImage)
                        .PutString(MediaMetadataCompat.MetadataKeyTitle, Currentsong.Name)
                        .PutLong(MediaMetadataCompat.MetadataKeyTrackNumber, Currentsong.TrackNumber)
                        .PutLong(MediaMetadataCompat.MetadataKeyNumTracks, 10)
                        .PutLong(MediaMetadataCompat.MetadataKeyDiscNumber, Currentsong.DiscNumber)
                        .PutString("SpotyPieImgUrl", Currentsong.MediumImage)
                        .Build();
                }
            }
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