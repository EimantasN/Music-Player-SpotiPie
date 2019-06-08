using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V4.Media;
using Mobile_Api.Models;

namespace SpotyPie.Music.Models
{
    public class MusicProvider
    {
        private Mobile_Api.Service API { get; set; }

        private Songs CurrentSong { get; set; } = new Songs();

        enum State
        {
            NonInitialized,
            Initializing,
            Initialized
        };

        private Mobile_Api.Service GetApiService()
        {
            if (API == null)
                API = new Mobile_Api.Service();
            return API;
        }

        volatile State currentState = State.NonInitialized;

        public MusicProvider()
        {
        }

        public string GetCurrentSongId => CurrentSong.Id.ToString();

        public async Task GetNextSongAsync()
        {
            CurrentSong = await GetApiService().GetNextSong();
        }

        public string CurrentSongSource()
        {
            return $"https://pie.pertrauktiestaskas.lt/api/stream/play/{CurrentSong.Id}";
        }

        public void SetFavorite(string musicId, bool favorite)
        {

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

        public MediaMetadataCompat GetMetadata()
        {
            return new MediaMetadataCompat.Builder()
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