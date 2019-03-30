using Android.App;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Models;
using Square.Picasso;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public static class Current_state
    {
        public static bool IsPlaying = false;

        public static bool Start_music { get; set; } = false;

        public static bool IsPlayerLoaded { get; set; } = false;

        public static bool PlayerIsVisible { get; set; } = false;

        public static float Progress { get; set; }

        public static Android.Support.V4.App.Fragment BackFragment { get; set; }

        private static string Current_Player_Image { get; set; }

        public static Album Current_Album { get; set; } = null;

        public static Artist Current_Artist { get; set; } = null;

        public static Item Current_Song { get; set; } = null;

        public static Playlist Current_Playlist { get; set; } = null;

        public static List<Item> Current_Song_List { get; set; } = null;

        public static RestClient client = new RestClient();

        public static void SetSong(Item song, bool refresh = false)
        {
            if (song.LocalUrl != null)
            {
                Current_Song = song;
                Player.Player.StartPlayMusic();
                Current_Artist = JsonConvert.DeserializeObject<List<Artist>>(Current_Song.Artists).First();
                Current_Song.Playing = true;
                Current_Song_List.First(x => x.Id == Current_Song.Id).Playing = true;
                Start_music = true;
                PlayerIsVisible = true;
                UpdateCurrentInfo();
                Task.Run(() => Update());

                async Task Update()
                {
                    var client = new RestClient("http://pie.pertrauktiestaskas.lt/api/songs/1/update");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    IRestResponse response = await client.ExecuteTaskAsync(request);
                }

                if (MainActivity.MiniPlayer.Visibility == ViewStates.Gone)
                    MainActivity.MiniPlayer.Visibility = ViewStates.Visible;
            }
            else
            {
                Toast.MakeText(Player.Player.contextStatic, "Please upload song", ToastLength.Short).Show();
            }
        }

        public static void UpdateCurrentInfo()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (Current_Player_Image != Current_Song.ImageUrl)
                    {
                        Current_Player_Image = Current_Song.ImageUrl;
                        Picasso.With(Player.Player.contextStatic).Load(Current_Song.ImageUrl).Resize(900, 900).CenterCrop().Into(Player.Player.Player_Image);
                    }
                    MainActivity.PlayerContainer.TranslationX = 0;
                    Player.Player.CurretSongTimeText.Text = "0.00";
                    Player.Player.Player_song_name.Text = Current_Song.Name;
                    MainActivity.SongTitle.Text = Current_Song.Name;
                    MainActivity.ArtistName.Text = Current_Song.Name;
                    Player.Player.Player_artist_name.Text = Current_Artist.Name;
                }, null);
            });
        }

        public static void SetArtist(Artist art)
        {
            Current_Artist = art;
        }

        public static void SetAlbum(Album album)
        {
            Current_Album = album;
            Current_Artist = JsonConvert.DeserializeObject<List<Artist>>(Current_Album.Artists).First();
            Application.SynchronizationContext.Post(_ =>
            {
                Player.Player.Player_playlist_name.Text = Current_Artist.Name + " - " + Current_Album.Name;
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

        public static void Music_play_toggle()
        {
            Current_state.IsPlaying = !Current_state.IsPlaying;

            if (Current_state.IsPlaying)
            {
                MainActivity.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.Player.PlayToggle.SetImageResource(Resource.Drawable.pause);
                Player.Player.player.Start();
            }
            else
            {
                MainActivity.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                Player.Player.PlayToggle.SetImageResource(Resource.Drawable.play_button);
                if (Player.Player.player.IsPlaying)
                    Player.Player.player.Pause();
            }
        }

        public static void Player_visiblibity_toggle()
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

        public static void ShowHeaderNavigationButtons()
        {
            MainActivity.BackHeaderButton.Visibility = ViewStates.Visible;
            MainActivity.OptionsHeaderButton.Visibility = ViewStates.Visible;
        }

        public static void HideHeaderNavigationButtons()
        {
            MainActivity.BackHeaderButton.Visibility = ViewStates.Gone;
            MainActivity.OptionsHeaderButton.Visibility = ViewStates.Gone;
        }

        public static void ChangeSong(bool Foward)
        {
            for (int i = 0; i < Current_Song_List.Count; i++)
            {
                if (Current_Song_List[i].Playing)
                {
                    if (Foward)
                    {
                        Current_Song_List[i].Playing = false;
                        if ((i + 1) == Current_Song_List.Count)
                        {
                            Current_Song_List[0].Playing = true;
                            SetSong(Current_Song_List[0]);
                        }
                        else
                        {
                            Current_Song_List[i + 1].Playing = true;
                            SetSong(Current_Song_List[i + 1]);
                        }
                    }
                    else
                    {
                        Current_Song_List[i].Playing = false;
                        if (i == 0)
                        {
                            Current_Song_List[0].Playing = true;
                            SetSong(Current_Song_List[0]);
                        }
                        else
                        {

                            Current_Song_List[i - 1].Playing = true;
                            SetSong(Current_Song_List[i - 1]);
                        }
                    }
                    break;
                }
            }
        }

        public static string GetAlbumPhoto()
        {
            if (Current_Album != null && Current_Album.Images != null && Current_Album.Images.Count != 0)
                return Current_Album.Images.First().Url;
            return null;
        }

        public static string GetArtistPhoto()
        {
            if (Current_Artist != null && Current_Artist.Images != null && Current_Artist.Images.Count != 0)
                return Current_Artist.Images.First().Url;
            return null;
        }

        public static string GetSongPhoto()
        {
            if (Current_Song != null && Current_Song.ImageUrl != null)
                return Current_Song.ImageUrl;
            return null;
        }
    }
}