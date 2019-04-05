using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using RestSharp;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Library.Fragments;
using SpotyPie.Models;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;


namespace SpotyPie
{
    public class Search : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.search_layout;

        //List Songs
        public List<Song> SearchSongs;
        public RvList<Song> Songs;
        private RecyclerView.LayoutManager SongsLayoutManager;
        private RecyclerView.Adapter SongsAdapter;
        private RecyclerView SongsRecyclerView;

        //public List<Album> AlbumsData;
        //public RvList<TwoBlockWithImage> Albums = new RvList<TwoBlockWithImage>();
        //private RecyclerView.LayoutManager AlbumsLayoutManager;
        //private RecyclerView.Adapter AlbumsAdapter;
        //private RecyclerView AlbumsRecyclerView;

        public List<Artist> ArtistsData;
        public RvList<Artist> Artists;
        private RecyclerView.LayoutManager ArtistsLayoutManager;
        private RecyclerView.Adapter ArtistsAdapter;
        private RecyclerView ArtistsRecyclerView;

        private bool IsSearchResultEmpty = true;
        private bool SongFinded = false;
        private bool AlbumFinded = false;
        private bool ArtistFinded = false;
        private bool PlaylistFinded = false;

        public bool SearchNow = true;

        ConstraintLayout SongsContainer;
        ConstraintLayout AlbumsContainer;
        ConstraintLayout PlaylistContainer;
        ConstraintLayout ArtistContainer;

        FrameLayout SearchEmpty;

        EditText search;
        ImageView SearchIcon;

        public static float Action;

        protected override void InitView()
        {
            SongsContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayoutSongs);
            AlbumsContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayoutAlbums);
            ArtistContainer = RootView.FindViewById<ConstraintLayout>(Resource.Id.ConstrainLayoutArtist);
            SearchEmpty = RootView.FindViewById<FrameLayout>(Resource.Id.searchStartx);
            var layoutParrams = SearchEmpty.LayoutParameters;
            layoutParrams.Height = MainActivity.HeightInDp;
            SearchEmpty.LayoutParameters = layoutParrams;

            SearchIcon = RootView.FindViewById<ImageView>(Resource.Id.imageView);

            search = RootView.FindViewById<EditText>(Resource.Id.editText);
            search.Text = "";
            search.BeforeTextChanged += Search_BeforeTextChanged;
            search.FocusChange += Search_FocusChange;

            // song list
            SearchSongs = new List<Song>();
            Songs = new RvList<Song>();
            SongsLayoutManager = new LinearLayoutManager(this.Activity);
            SongsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.song_rv);
            SongsRecyclerView.SetLayoutManager(SongsLayoutManager);
            SongsAdapter = new VerticalRV(Songs, this.Context);
            Songs.Adapter = SongsAdapter;
            SongsRecyclerView.SetAdapter(SongsAdapter);
            SongsRecyclerView.NestedScrollingEnabled = false;

            SongsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (SongsRecyclerView != null && SongsRecyclerView.ChildCount != 0)
                {
                    GetState().Current_Song_List = SearchSongs;
                    GetState().SetSong(GetState().Current_Song_List[position]);
                }
            });


            //AlbumsLayoutManager = new LinearLayoutManager(this.Activity);
            //AlbumsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.albums_rv);
            //AlbumsRecyclerView.SetLayoutManager(AlbumsLayoutManager);
            //AlbumsAdapter = new BaseRv<TwoBlockWithImage>(Albums, AlbumsRecyclerView, this.Context);
            //Albums.Adapter = AlbumsAdapter;
            //AlbumsRecyclerView.SetAdapter(AlbumsAdapter);
            //AlbumsRecyclerView.NestedScrollingEnabled = false;

            //AlbumsRecyclerView.SetItemClickListener((rv, position, view) =>
            //{
            //    if (SongsRecyclerView != null && SongsRecyclerView.ChildCount != 0)
            //    {
            //        var c = SongsRecyclerView.Width;
            //        float Procent = (Action * 100) / AlbumsRecyclerView.Width;
            //        if (Procent <= 50)
            //        {
            //            Current_state.SetAlbum(AlbumsData[position]);
            //            FragmentManager.BeginTransaction()
            //            .Replace(Resource.Id.content_frame, MainActivity.Album)
            //            .Commit();
            //        }
            //        else
            //        {
            //            Current_state.SetAlbum(AlbumsData[position + 1]);
            //            FragmentManager.BeginTransaction()
            //            .Replace(Resource.Id.content_frame, MainActivity.Album)
            //            .Commit();
            //        }
            //    }
            //});

            //ARTIST RV
            ArtistsData = new List<Artist>();
            Artists = new RvList<Artist>();
            ArtistsLayoutManager = new LinearLayoutManager(this.Activity);
            ArtistsRecyclerView = RootView.FindViewById<RecyclerView>(Resource.Id.artists_rv);
            ArtistsRecyclerView.SetLayoutManager(ArtistsLayoutManager);
            ArtistsAdapter = new ArtistRV(Artists, this.Context);
            Artists.Adapter = ArtistsAdapter;
            ArtistsRecyclerView.SetAdapter(ArtistsAdapter);
            ArtistsRecyclerView.NestedScrollingEnabled = false;

            ArtistsRecyclerView.SetItemClickListener((rv, position, view) =>
            {
                if (ArtistsRecyclerView != null && ArtistsRecyclerView.ChildCount != 0)
                {
                    //Current_state.SetSong(Current_state.Current_Song_List[position]);
                }
            });
        }

        public override void OnResume()
        {
            base.OnResume();
            SearchNow = true;
            Task.Run(() => Checker());
        }

        public override void OnStop()
        {
            base.OnStop();
            SearchNow = false;
        }

        private void Search_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (search.IsFocused)
            {
                if (search.Text.Contains("Search"))
                    search.Text = "";
            }

            if (!SongFinded && !AlbumFinded && !ArtistFinded && !PlaylistFinded)
                IsSearchResultEmpty = true;
            else
                IsSearchResultEmpty = false;
        }

        private void Search_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
        }

        public async Task Checker()
        {
            var query = "";
            while (SearchNow)
            {
                try
                {
                    if (query != search.Text)
                    {
                        var a = Task.Run(() => SearchSong(search.Text));
                        var b = Task.Run(() => SearchAlbums(search.Text));
                        var c = Task.Run(() => SearchArtist(search.Text));

                        while (!(a.IsCompletedSuccessfully || a.IsCanceled || a.IsCompleted || a.IsFaulted)) await Task.Delay(500);
                        while (!(b.IsCompletedSuccessfully || b.IsCanceled || b.IsCompleted || b.IsFaulted)) await Task.Delay(500);
                        while (!(c.IsCompletedSuccessfully || c.IsCanceled || c.IsCompleted || c.IsFaulted)) await Task.Delay(500);

                        if (!SongFinded && !AlbumFinded && !ArtistFinded && !PlaylistFinded)
                            SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Visible);
                        else
                            SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Gone);

                        query = search.Text;
                    }
                }
                catch (Exception e)
                {
                    Snackbar.Make(this.Activity.Window.DecorView.RootView, e.Message, Snackbar.LengthShort).Show();
                }
                await Task.Delay(500);
            }
        }

        public async Task SearchSong(string query)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/api/songs/search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(query), ParameterType.RequestBody);
                IRestResponse response = await Client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    Songs.Clear();
                    var Songsx = JsonConvert.DeserializeObject<List<Song>>(response.Content);
                    if (Songsx != null && Songsx.Count != 0)
                    {
                        Application.SynchronizationContext.Post(_ =>
                        {
                            SearchSongs = Songsx;
                            SongFinded = true;
                            if (SongsContainer.Visibility == ViewStates.Gone)
                                SongsContainer.Visibility = ViewStates.Visible;
                        }, null);

                        foreach (var x in Songsx.Take(16))
                        {
                            Songs.Add(x);
                        }
                    }
                    else
                    {
                        Application.SynchronizationContext.Post(_ =>
                        {
                            SongFinded = false;
                            if (SongsContainer.Visibility == ViewStates.Visible)
                                SongsContainer.Visibility = ViewStates.Gone;
                        }, null);
                    }
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    SongFinded = false;
                    if (SongsContainer.Visibility == ViewStates.Visible)
                        SongsContainer.Visibility = ViewStates.Gone;
                }, null);
            }
        }

        public async Task SearchAlbums(string query)
        {

            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/Api/Album/Search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(query), ParameterType.RequestBody);
                IRestResponse response = await Client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    //var Albumsx = JsonConvert.DeserializeObject<List<Album>>(response.Content);
                    //if (Albumsx != null && Albumsx.Count != 0)
                    //{
                    //    Albums.Clear();
                    //    Application.SynchronizationContext.Post(_ =>
                    //    {
                    //        AlbumsData = Albumsx;
                    //        AlbumFinded = true;
                    //        if (AlbumsContainer.Visibility == ViewStates.Gone)
                    //            AlbumsContainer.Visibility = ViewStates.Visible;
                    //    }, null);
                    //    for (int i = 0; i < Albumsx.Count && i <= 8; i = i + 2)
                    //    {
                    //        if (Albums.Count - i == 1)
                    //        {
                    //            var x = Albumsx[i];
                    //            //Albums.Add(new TwoBlockWithImage(
                    //            //new BlockWithImage(
                    //            //    x.Id,
                    //            //    RvType.Album,
                    //            //    x.Name,
                    //            //    x.Label,
                    //            //    x.Images.First().Url)));
                    //        }
                    //        else
                    //        {
                    //            var x = Albumsx[i];
                    //            var y = Albumsx[i + 1];
                    //            //Albums.Add(new TwoBlockWithImage(
                    //            //    new BlockWithImage(
                    //            //        x.Id,
                    //            //        RvType.Album,
                    //            //        x.Name,
                    //            //        x.Label,
                    //            //        x.Images.First().Url),
                    //            //        new BlockWithImage(
                    //            //        y.Id,
                    //            //        RvType.Album,
                    //            //        y.Name,
                    //            //        x.Label,
                    //            //        y.Images.First().Url)));
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Application.SynchronizationContext.Post(_ =>
                    //    {
                    //        AlbumFinded = false;
                    //        if (AlbumsContainer.Visibility == ViewStates.Visible)
                    //            AlbumsContainer.Visibility = ViewStates.Gone;
                    //    }, null);
                    //}
                }
                else
                {

                }
            }
            catch (Exception)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    AlbumFinded = false;
                    if (AlbumsContainer.Visibility == ViewStates.Visible)
                        AlbumsContainer.Visibility = ViewStates.Gone;
                }, null);
            }
        }

        public async Task SearchArtist(string query)
        {
            try
            {
                RestClient Client = new RestClient("http://pie.pertrauktiestaskas.lt/Api/artist/Search");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(query), ParameterType.RequestBody);
                IRestResponse response = await Client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    var mArtists = JsonConvert.DeserializeObject<List<Artist>>(response.Content);
                    if (mArtists != null && mArtists.Count != 0)
                    {
                        Artists.Clear();
                        foreach (var x in mArtists)
                        {
                            Artists.Add(x);
                        }
                        Application.SynchronizationContext.Post(_ =>
                        {
                            ArtistsData = mArtists;
                            ArtistFinded = true;
                            if (ArtistContainer.Visibility == ViewStates.Gone)
                                ArtistContainer.Visibility = ViewStates.Visible;
                        }, null);

                    }
                    else
                    {
                        Application.SynchronizationContext.Post(_ =>
                        {
                            ArtistFinded = false;
                            if (ArtistContainer.Visibility == ViewStates.Visible)
                                ArtistContainer.Visibility = ViewStates.Gone;
                        }, null);
                    }
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    ArtistFinded = false;
                    if (ArtistContainer.Visibility == ViewStates.Visible)
                        ArtistContainer.Visibility = ViewStates.Gone;
                }, null);
            }
        }
    }
}