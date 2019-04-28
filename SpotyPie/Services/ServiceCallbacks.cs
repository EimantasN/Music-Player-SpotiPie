using System.Collections.Generic;
using Android.Media;
using Mobile_Api.Models;

namespace SpotyPie.Services
{
    public interface ServiceCallbacks
    {
        int? GetSongId();

        int? GetArtistId();

        int? GetAlbumId();

        int? GetPlaylistId();

        List<Songs> GetSongList();

        void NextSongPlayer();
        void PrevSongPlayer();

        void Play();

        void PlayerPrepared(int duration);

        void Music_play();

        void Music_pause();

        void SongChangeStarted(List<Songs> song, int position = 0);

        void SongLoadStarted(List<Songs> newSongList, int position);

        void SongLoadEnded();

        void SongEnded();

        void SongStopped();

        void SetSeekBarProgress(int progress, string text);

        int GetViewLoadState();

        void SetViewLoadState();
    }
}