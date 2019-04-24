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
    }
}