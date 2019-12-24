using System;
using System.Collections.Generic;
using Android.Support.V4.Media;
using Mobile_Api;
using Mobile_Api.Models;
using SpotyPie.Music.Manager;

namespace SpotyPie.Music.Models
{
    public class MusicProvider
    {
        public int Id 
        { 
            get
            {
                return SongManager.SongId;
            }
        }

        volatile Enums.State currentState = Enums.State.NonInitialized;

        public MusicProvider()
        {
        }

        public int GetCurrentSong() => SongManager.SongId;

        public void SongPaused() => SongManager.Pause();

        public void SongResumed() => SongManager.Play();

        public string CurrentSongSource()
        {
            return $"{BaseClient.BaseUrl}api/stream/play/{SongManager.SongId}";
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
            finally
            {
                if (currentState != Enums.State.Initialized)
                    currentState = Enums.State.NonInitialized;
            }
        }

        private MediaMetadataCompat MetaData;

        public MediaMetadataCompat GetMetadata() => MetaData;

        public void BuildMetadata(Songs Currentsong)
        {
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