using Mobile_Api.Models;
using SpotyPie.Base;
using System.Collections.Generic;

namespace SpotyPie
{
    public class Current_state
    {
        public bool IsPlaying = false;

        public static int PrevId { get; set; }

        public bool Start_music { get; set; } = false;

        public bool IsPlayerLoaded { get; set; } = false;

        public bool PlayerIsVisible { get; set; } = false;

        public float Progress { get; set; }

        public ActivityBase Activity { get; set; }

        public string Current_Player_Image { get; set; }

        public Current_state(ActivityBase activity)
        {
            this.Activity = activity;
        }

        public void SetSong(List<Songs> songs, int position, bool refresh = false)
        {
            Activity?.StartPlayer();
        }

        public void SetArtist(Artist art)
        {
        }

        public void SetAlbum(Album album)
        {
        }

        internal void Dispose()
        {
        }
    }
}