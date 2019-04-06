using Android.App;
using Android.Views;
using Mobile_Api.Models;
using Square.Picasso;
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

        private string Current_Player_Image { get; set; }

        public Album Current_Album { get; set; } = null;

        public Song Current_Song { get; set; } = null;

        public List<Song> Current_Song_List { get; set; } = new List<Song>();

        private Player.Player Player { get; set; }

        public Current_state(Player.Player player, MainActivity activity)
        {
            this.Player = player;
            this.Activity = activity;
        }

        public void SetSong(List<Song> song, int position = 0, bool refresh = false)
        {
            Activity.TogglePlayer(true);
            SetCurrentSong(song, position);
            UpdateCurrentInfo();

            if (Activity.MiniPlayer.Visibility == ViewStates.Gone)
                Activity.MiniPlayer.Visibility = ViewStates.Visible;
        }

        public void SetCurrentSong(List<Song> song, int position)
        {
            Current_Song = song[position];
            Current_Song.SetIsPlaying(true);
            Current_Song_List = song;

            if (Id != song[position].Id)
            {
                PrevId = Id;
                Id = song[position].Id;
                Start_music = true;
                PlayerIsVisible = true;
                Player.PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                Player.PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                Player.StartPlayMusic();
            }
        }

        public void SetCurrentSongList(List<Song> songs)
        {
            if (songs != null && songs.Count > 0)
            {
                songs.First().SetIsPlaying(true);
                Current_Song_List.AddRange(songs);
            }
        }

        public void UpdateSongList(Song song)
        {
            Current_Song_List.First(x => x.Id == Current_Song.Id).SetIsPlaying(true);
        }

        public void UpdateCurrentInfo()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (Current_Player_Image != Current_Song.LargeImage)
                    {
                        Current_Player_Image = Current_Song.LargeImage;
                        Picasso.With(Activity.ApplicationContext).Load(Current_Song.LargeImage).Into(Player.Player_Image);
                    }
                    Player.CurretSongTimeText.Text = "0.00";
                    Player.Player_song_name.Text = Current_Song.Name;
                    Activity.SongTitle.Text = Current_Song.Name;
                    Activity.ArtistName.Text = Current_Song.Name;
                    Player.Player_artist_name.Text = "Muse";
                }, null);
            });
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
                Player.Player_playlist_name.Text = "Muse" + " - " + Current_Album.Name;
            }, null);
        }

        public void Music_play_toggle()
        {
            IsPlaying = !IsPlaying;

            if (IsPlaying)
            {
                Activity.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.MusicPlayer.Start();
            }
            else
            {
                Activity.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                Player.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                if (Player.MusicPlayer.IsPlaying)
                    Player.MusicPlayer.Pause();
            }
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

        public void ChangeSong(bool Foward)
        {
            if (Current_Song_List == null) return;

            for (int i = 0; i < Current_Song_List.Count; i++)
            {
                if (Current_Song_List[i].IsPlayingNow())
                {
                    if (Foward)
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if ((i + 1) == Current_Song_List.Count)
                        {
                            Current_Song_List[0].SetIsPlaying(true);
                            SetSong(Current_Song_List, 0);
                        }
                        else
                        {
                            Current_Song_List[i + 1].SetIsPlaying(true);
                            SetSong(Current_Song_List, i + 1);
                        }
                    }
                    else
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if (i == 0)
                        {
                            Current_Song_List[0].SetIsPlaying(true);
                            SetSong(Current_Song_List, 0);
                        }
                        else
                        {

                            Current_Song_List[i - 1].SetIsPlaying(true);
                            SetSong(Current_Song_List, i - 1);
                        }
                    }
                    break;
                }
            }
        }
    }
}