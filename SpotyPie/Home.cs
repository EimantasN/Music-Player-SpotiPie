using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Models;
using SpotyPie.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace SpotyPie
{
    public class Home : FragmentBase
    {
        //Recent albums
        public List<Album> RecentAlbumsData;
        public RecycleViewList<BlockWithImage> RecentAlbums;
        private RecyclerView.LayoutManager RecentAlbumsLayoutManager;
        private RecyclerView.Adapter RecentAlbumsAdapter;
        private RecyclerView RecentAlbumsRecyclerView;

        //Best albums
        public List<Album> BestAlbumsData;
        public RecycleViewList<BlockWithImage> BestAlbums;
        private RecyclerView.LayoutManager BestAlbumsLayoutManager;
        private RecyclerView.Adapter BestAlbumsAdapter;
        private RecyclerView BestAlbumsRecyclerView;

        //Best artists
        public List<Artist> BestArtistList;
        public RecycleViewList<BlockWithImage> BestArtists;
        private RecyclerView.LayoutManager BestArtistsLayoutManager;
        private RecyclerView.Adapter BestArtistsAdapter;
        private RecyclerView BestArtistsRecyclerView;

        //Jump back albums
        public List<Album> JumpBackData;
        public RecycleViewList<BlockWithImage> JumpBack;
        private RecyclerView.LayoutManager JumpBackLayoutManager;
        private RecyclerView.Adapter JumpBackAdapter;
        private RecyclerView JumpBackRecyclerView;

        //Top playlist
        public List<Playlist> TopPlaylistData;
        public RecycleViewList<BlockWithImage> TopPlaylist;
        private RecyclerView.LayoutManager TopPlaylistLayoutManager;
        private RecyclerView.Adapter TopPlaylistAdapter;
        private RecyclerView TopPlaylistRecyclerView;

        protected override void InitView()
        {
            base.InitView();

            RecentAlbums = new RecycleViewList<BlockWithImage>();
            BestAlbums = new RecycleViewList<BlockWithImage>();
            BestArtists = new RecycleViewList<BlockWithImage>();
            JumpBack = new RecycleViewList<BlockWithImage>();
            TopPlaylist = new RecycleViewList<BlockWithImage>();
            //RECENT ALBUMS
            RecentAlbumsLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            RecentAlbumsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.recent_rv);
            RecentAlbumsRecyclerView.SetLayoutManager(RecentAlbumsLayoutManager);
            RecentAlbumsAdapter = new HorizontalRV(RecentAlbums, RecentAlbumsRecyclerView, this.Context);
            RecentAlbums.Adapter = RecentAlbumsAdapter;
            RecentAlbumsRecyclerView.SetAdapter(RecentAlbumsAdapter);

            RecentAlbumsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (RecentAlbumsRecyclerView != null && RecentAlbumsRecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new AlbumFragment();
                    Current_state.SetAlbum(RecentAlbumsData[position]);
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                        .Commit();
                }
            });

            //MOST POLULAR ALL TIME ALBUMS
            BestAlbumsLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            BestAlbumsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.best_albums_rv);
            BestAlbumsRecyclerView.SetLayoutManager(BestAlbumsLayoutManager);
            BestAlbumsAdapter = new HorizontalRV(BestAlbums, BestAlbumsRecyclerView, this.Context);
            BestAlbums.Adapter = BestAlbumsAdapter;
            BestAlbumsRecyclerView.SetAdapter(BestAlbumsAdapter);

            BestAlbumsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (BestAlbumsRecyclerView != null && BestAlbumsRecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new AlbumFragment();
                    Current_state.SetAlbum(BestAlbumsData[position]);
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                    .Commit();
                }
            });



            //MOST POPULAR ARTISTS
            BestArtistsLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            BestArtistsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.best_artists_rv);
            BestArtistsRecyclerView.SetLayoutManager(BestArtistsLayoutManager);
            BestArtistsAdapter = new HorizontalRV(BestArtists, BestArtistsRecyclerView, this.Context);
            BestArtists.Adapter = BestArtistsAdapter;
            BestArtistsRecyclerView.SetAdapter(BestArtistsAdapter);

            BestArtistsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (BestArtistsRecyclerView != null && BestArtistsRecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new ArtistFragment();
                    Current_state.SetArtist(BestArtistList[position]);
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                    .Commit();
                }
            });

            //OLD ALBUMS AND SONGS
            JumpBackLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            JumpBackRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.albums_old_rv);
            BestAlbumsRecyclerView.SetLayoutManager(JumpBackLayoutManager);
            JumpBackAdapter = new HorizontalRV(JumpBack, JumpBackRecyclerView, this.Context);
            JumpBack.Adapter = JumpBackAdapter;
            JumpBackRecyclerView.SetAdapter(JumpBackAdapter);

            JumpBackRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (JumpBackRecyclerView != null && JumpBackRecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new AlbumFragment();
                    Current_state.SetAlbum(JumpBackData[position]);
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                    .Commit();
                }
            });


            //MOST POLUPAR USER PLAYLISTS
            TopPlaylistLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            TopPlaylistRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.playlist_rv);
            BestAlbumsRecyclerView.SetLayoutManager(TopPlaylistLayoutManager);
            TopPlaylistAdapter = new HorizontalRV(TopPlaylist, TopPlaylistRecyclerView, this.Context);
            TopPlaylist.Adapter = TopPlaylistAdapter;
            TopPlaylistRecyclerView.SetAdapter(TopPlaylistAdapter);

            TopPlaylistRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (TopPlaylistRecyclerView != null && TopPlaylistRecyclerView.ChildCount != 0)
                {
                    MainActivity.Fragment.TranslationX = 0;
                    MainActivity.CurrentFragment = new Playlist_view();
                    Current_state.Current_Playlist = TopPlaylistData[position];
                    MainActivity.mSupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.song_options, MainActivity.CurrentFragment)
                    .Commit();
                }
            });
        }

        public override void OnResume()
        {
            base.OnResume();
            Task.Run(() => GetRecentAlbumsAsync(this.Context));

            Task.Run(() => GetPolularAlbumsAsync(this.Context));

            Task.Run(() => GetPolularArtistsAsync(this.Context));

            Task.Run(() => GetOldAlbumsAsync(this.Context));

            Task.Run(() => GetPlaylists(this.Context));

            //if (TopPlaylist == null || TopPlaylist.Count == 0)
            //Todo add playlist call
        }

        public async Task GetRecentAlbumsAsync(Context cnt)
        {
            try
            {
                var albums = await GetService().GetAlbumsAsync(AlbumType.Recent);
                InvokeOnMainThread(() =>
                {
                    foreach (var x in albums)
                    {
                        RecentAlbumsData.Add(x);
                        RecentAlbums.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                    }
                });
            }
            catch (Exception)
            {

            }
        }

        public async Task GetPolularAlbumsAsync(Context cnt)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/album/popular");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Album> album = JsonConvert.DeserializeObject<List<Album>>(response.Content);
                    Application.SynchronizationContext.Post(_ =>
                    {
                        BestAlbumsData = album;
                        foreach (var x in album)
                        {
                            BestAlbums.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                        }
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "Recent Albums API error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task GetPolularArtistsAsync(Context cnt)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/artist/popular");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Artist> artists = JsonConvert.DeserializeObject<List<Artist>>(response.Content);
                    Application.SynchronizationContext.Post(_ =>
                    {
                        BestArtistList = artists;
                        foreach (var x in artists)
                        {
                            string DisplayGenre;
                            List<string> genres = new List<string>();

                            if (x.Genres != null)
                                genres = JsonConvert.DeserializeObject<List<string>>(x.Genres);

                            if (genres.Count > 1)
                            {
                                Random ran = new Random();
                                int index = ran.Next(0, genres.Count - 1);
                                DisplayGenre = genres[index];
                            }
                            else if (genres.Count == 1)
                            {
                                DisplayGenre = genres[0];
                            }
                            else
                                DisplayGenre = string.Empty;

                            var img = string.Empty;
                            if (x.Images.FirstOrDefault() != null)
                                img = x.Images.First().Url;

                            BestArtists.Add(new BlockWithImage(x.Id, RvType.Artist, x.Name, DisplayGenre, img));
                        }
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "Recent Albums API error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception e)
            {

            }
        }

        public async Task GetOldAlbumsAsync(Context cnt)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/album/old");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Album> album = JsonConvert.DeserializeObject<List<Album>>(response.Content);
                    Application.SynchronizationContext.Post(_ =>
                    {
                        JumpBackData = album;
                        foreach (var x in album)
                        {
                            JumpBack.Add(new BlockWithImage(x.Id, RvType.Album, x.Name, JsonConvert.DeserializeObject<List<Artist>>(x.Artists).First().Name, x.Images.First().Url));
                        }
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "Recent Albums API error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception)
            {

            }
        }

        public async Task GetPlaylists(Context cnt)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/playlist/playlists");
                var request = new RestRequest(Method.GET);
                IRestResponse response = await Client.ExecuteGetTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Playlist> album = JsonConvert.DeserializeObject<List<Playlist>>(response.Content);
                    Application.SynchronizationContext.Post(_ =>
                    {
                        TopPlaylistData = album;
                        foreach (var x in album)
                        {
                            TopPlaylist.Add(new BlockWithImage(x.Id, RvType.Playlist, x.Name, x.Created.ToString("yyyy-MM-dd"), x.ImageUrl));
                        }
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(this.Context, "Recent Albums API error", ToastLength.Short).Show();
                    }, null);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}