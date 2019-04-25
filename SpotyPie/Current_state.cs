using Android.App;
using Android.Views;
using Mobile_Api;
using Mobile_Api.Models;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Current_state
    {
        public bool IsPlaying = false;

        public static int PrevId { get; set; }

        public static int Id { get; set; }

        public bool Start_music { get; set; } = false;

        public bool IsPlayerLoaded { get; set; } = false;

        public bool PlayerIsVisible { get; set; } = false;

        public float Progress { get; set; }

        public MainActivity Activity { get; set; }

        public string Current_Player_Image { get; set; }

        public Album Current_Album { get; set; } = null;

        public Artist Current_Artist { get; set; } = null;

        public Playlist Current_Playlist { get; set; } = null;

        public Songs Current_Song { get; set; } = null;

        public List<Songs> Current_Song_List { get; set; } = new List<Songs>();

        private Player.Player Player { get; set; }

        public Current_state(MainActivity activity)
        {
            this.Activity = activity;
            Player = new Player.Player();
            this.Activity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.player_frame, Player)
                    .Commit();
        }

        public Player.Player GetPlayer()
        {
            return Player;
        }

        public void SetSong(List<Songs> song, int position = 0, bool refresh = false)
        {
            GetPlayer().SongChangeStarted(song, position);
        }

        public void SetCurrentSongList(List<Songs> songs)
        {
            if (songs != null && songs.Count > 0)
            {
                songs.First().SetIsPlaying(true);
                Current_Song_List.AddRange(songs);
            }
        }

        public void UpdateSongList(Songs song)
        {
            Current_Song_List.First(x => x.Id == Current_Song.Id).SetIsPlaying(true);
        }

        public void SetArtist(Artist art)
        {
            //Current_Artist = art;
        }

        public void SetAlbum(Album album)
        {
            Current_Album = album;
            //Current_Artist = JsonConvert.DeserializeObject<List<Artist>>(Current_Album.Artists).First();
            Application.SynchronizationContext.Post(_ =>
            {
                GetPlayer().Player_playlist_name.Text = "Muse" + " - " + Current_Album.Name;
            }, null);
        }

        public void Player_visiblibity_toggle()
        {
            if (PlayerIsVisible)
            {
                PlayerIsVisible = false;
                Activity.TogglePlayer(false);
            }
            else
            {
                Activity.TogglePlayer(true);
                PlayerIsVisible = true;
            }
        }

        internal void Dispose()
        {
        }

        public void ShowHeaderNavigationButtons()
        {
            Activity.BackHeaderButton.Visibility = ViewStates.Visible;
            Activity.OptionsHeaderButton.Visibility = ViewStates.Visible;
        }

        public void HideHeaderNavigationButtons()
        {
            Activity.BackHeaderButton.Visibility = ViewStates.Gone;
            Activity.OptionsHeaderButton.Visibility = ViewStates.Gone;
        }

        internal void SetSongDuration(int Duration)
        {
            try
            {
                if (Current_Song.DurationMs != Duration)
                {
                    var API = Activity.GetService();
                    var a = API.SetSongDuration(Current_Song.Id, Duration);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void ToggleBotNav(bool show)
        {
            if (show)
            {
                this.Activity.bottomNavigation.Visibility = ViewStates.Visible;
            }
            else
            {
                this.Activity.bottomNavigation.Visibility = ViewStates.Gone;
            }
        }

        public void ToggleMiniPlayer(bool show)
        {
            if (show)
            {
                this.Activity.MiniPlayer.Visibility = ViewStates.Visible;
            }
            else
            {
                this.Activity.MiniPlayer.Visibility = ViewStates.Gone;
            }
        }
    }
}