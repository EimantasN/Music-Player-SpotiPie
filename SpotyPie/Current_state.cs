using Android.App;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using RestSharp;
using SpotyPie.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Current_state
    {
        public bool IsPlaying = false;

        public bool Start_music { get; set; } = false;

        public bool IsPlayerLoaded { get; set; } = false;

        public bool PlayerIsVisible { get; set; } = false;

        public float Progress { get; set; }

        public FragmentBase BackFragment { get; set; }

        private string Current_Player_Image { get; set; }

        public Album Current_Album { get; set; } = null;

        public Artist Current_Artist { get; set; } = null;

        public Song Current_Song { get; set; } = null;

        public Playlist Current_Playlist { get; set; } = null;

        public List<Song> Current_Song_List { get; set; } = new List<Song>();

        public RestClient client = new RestClient();

        Player.Player Player { get; set; }

        public Current_state(Player.Player player)
        {
            this.Player = player;
        }

        public void SetSong(Song song, bool refresh = false)
        {
            if (song.LocalUrl != null)
            {
                SetCurrentSong(song);
                Player.StartPlayMusic();
                //Current_Song_List.First(x => x.Id == Current_Song.Id).Playing = true;
                Start_music = true;
                PlayerIsVisible = true;
                UpdateCurrentInfo();

                if (MainActivity.MiniPlayer.Visibility == ViewStates.Gone)
                    MainActivity.MiniPlayer.Visibility = ViewStates.Visible;
            }
            else
            {
                //Toast.MakeText(context, "Please upload song", ToastLength.Short).Show();
            }
        }

        public void SetCurrentSong(Song song)
        {
            Current_Song = song;
            Current_Song.SetIsPlaying(true);
            Current_Song_List.Add(song);
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
                  //Picasso.With(Player.Player.context).Load(Current_Song.ImageUrl).Resize(900, 900).CenterCrop().Into(Player.Player.Player_Image);
              }
              MainActivity.PlayerContainer.TranslationX = 0;
              Player.CurretSongTimeText.Text = "0.00";
              Player.Player_song_name.Text = Current_Song.Name;
              MainActivity.SongTitle.Text = Current_Song.Name;
              MainActivity.ArtistName.Text = Current_Song.Name;
              Player.Player_artist_name.Text = Current_Artist.Name;
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
                Player.Player_playlist_name.Text = Current_Artist + " - " + Current_Album.Name;
            }, null);

            Task.Run(() => Update());
            async Task Update()
            {
                client.BaseUrl = new System.Uri("http://pie.pertrauktiestaskas.lt/api/album/update/" + album.Id);
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                IRestResponse response = await client.ExecuteTaskAsync(request);
            }
        }

        public void Music_play_toggle()
        {
            IsPlaying = !IsPlaying;

            if (IsPlaying)
            {
                MainActivity.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.player.Start();
            }
            else
            {
                MainActivity.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                Player.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                if (Player.player.IsPlaying)
                    Player.player.Pause();
            }
        }

        public void Player_visiblibity_toggle()
        {
            if (PlayerIsVisible)
            {
                PlayerIsVisible = false;
                MainActivity.PlayerContainer.TranslationX = MainActivity.widthInDp;
            }
            else
            {
                PlayerIsVisible = true;
                MainActivity.PlayerContainer.TranslationX = 0;
            }
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ShowHeaderNavigationButtons()
        {
            MainActivity.BackHeaderButton.Visibility = ViewStates.Visible;
            MainActivity.OptionsHeaderButton.Visibility = ViewStates.Visible;
        }

        public void HideHeaderNavigationButtons()
        {
            MainActivity.BackHeaderButton.Visibility = ViewStates.Gone;
            MainActivity.OptionsHeaderButton.Visibility = ViewStates.Gone;
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
                            SetSong(Current_Song_List[0]);
                        }
                        else
                        {
                            Current_Song_List[i + 1].SetIsPlaying(true);
                            SetSong(Current_Song_List[i + 1]);
                        }
                    }
                    else
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if (i == 0)
                        {
                            Current_Song_List[0].SetIsPlaying(true);
                            SetSong(Current_Song_List[0]);
                        }
                        else
                        {

                            Current_Song_List[i - 1].SetIsPlaying(true);
                            SetSong(Current_Song_List[i - 1]);
                        }
                    }
                    break;
                }
            }
        }
    }
}