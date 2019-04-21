using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Felipecsl.GifImageViewLib;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Search : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.search_layout;

        public const int SongLimit = 6;
        public const int AlbumLimit = 6;
        public const int ArtistLimit = 6;


        private bool IsSearchResultEmpty = true;
        private bool SongFinded = false;
        private bool AlbumFinded = false;
        private bool ArtistFinded = false;
        private bool PlaylistFinded = false;

        public bool SearchNow = true;

        private TextView SongsContainer;
        private TextView AlbumsContainer;
        private TextView ArtistContainer;

        FrameLayout SearchEmpty;

        EditText search;
        ImageView SearchIcon;
        ProgressBar SearchLoading;

        private GifImageView Searching_Gif;
        private ImageView Search_start_image;

        private RvList<Songs> RvDataSongs { get; set; }
        private RvList<Album> RvDataAlbums { get; set; }
        private RvList<Artist> RvDataArtist { get; set; }

        protected override void InitView()
        {
            SetupSearchLoadingGif();

            Search_start_image = RootView.FindViewById<ImageView>(Resource.Id.se_image);

            SongsContainer = RootView.FindViewById<TextView>(Resource.Id.SongsTitle);
            SongsContainer.Visibility = ViewStates.Gone;

            AlbumsContainer = RootView.FindViewById<TextView>(Resource.Id.AlbumsTitle);
            AlbumsContainer.Visibility = ViewStates.Gone;

            ArtistContainer = RootView.FindViewById<TextView>(Resource.Id.artist_title);
            ArtistContainer.Visibility = ViewStates.Gone;

            SearchEmpty = RootView.FindViewById<FrameLayout>(Resource.Id.searchStartx);

            SearchIcon = RootView.FindViewById<ImageView>(Resource.Id.seacr_icon);
            SearchLoading = RootView.FindViewById<ProgressBar>(Resource.Id.search_loading);
            search = RootView.FindViewById<EditText>(Resource.Id.search_text);
            search.FocusChange += Search_FocusChange;

            if (RvDataSongs == null)
            {
                var rvBase = new BaseRecycleView<Songs>(this, Resource.Id.song_rv);
                RvDataSongs = rvBase.Setup(LinearLayoutManager.Vertical);
                rvBase.DisableScroolNested();
            }

            if (RvDataAlbums == null)
            {
                var rvBaseAlbum = new BaseRecycleView<Album>(this, Resource.Id.albums_rv);
                RvDataAlbums = rvBaseAlbum.Setup(LinearLayoutManager.Vertical);
                rvBaseAlbum.DisableScroolNested();
            }

            if (RvDataArtist == null)
            {
                var rvBaseArtist = new BaseRecycleView<Artist>(this, Resource.Id.artists_rv);
                RvDataArtist = rvBaseArtist.Setup(LinearLayoutManager.Vertical);
                rvBaseArtist.DisableScroolNested();
            }
        }

        private void SetupSearchLoadingGif()
        {
            Searching_Gif = RootView.FindViewById<GifImageView>(Resource.Id.seach_gif);
            Searching_Gif.Visibility = ViewStates.Gone;

            Stream input = this.Activity.Resources.OpenRawResource(Resource.Drawable.seaching_image);//this.Activity.Assets.Open("seaching_image.gif");
            byte[] bytes = ConvertFileToByteArray.Convert(input);
            Searching_Gif.SetBytes(bytes);
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
                GetState().ToggleBotNav(false);
                GetState().ToggleMiniPlayer(false);
                if (search.Text.Contains("Search"))
                    search.Text = "";
            }
            else
            {
                GetState().ToggleBotNav(true);
                GetState().ToggleMiniPlayer(true);
                if (string.IsNullOrEmpty(search.Text))
                    search.Text = "Search song, album, playlist and etc.";
            }

            if (!SongFinded && !AlbumFinded && !ArtistFinded && !PlaylistFinded)
                IsSearchResultEmpty = true;
            else
                IsSearchResultEmpty = false;
        }

        public async Task Checker()
        {
            var query = string.Empty;
            long index = 0;
            while (SearchNow)
            {
                try
                {
                    if (query != search.Text && !string.IsNullOrEmpty(search.Text) && search.Text != "Search song, album, playlist and etc.")
                    {
                        query = search.Text;
                        long tempIndex = ++index;

                        ToggleSearchLoading(Search_status.Searching);

                        var a = Task.Run(() => SearchSong(search.Text));
                        var b = Task.Run(() => SearchAlbums(search.Text));
                        var c = Task.Run(() => SearchArtist(search.Text));

                        while (!(a.IsCompletedSuccessfully || a.IsCanceled || a.IsCompleted || a.IsFaulted)) await Task.Delay(50);
                        while (!(b.IsCompletedSuccessfully || b.IsCanceled || b.IsCompleted || b.IsFaulted)) await Task.Delay(50);
                        while (!(c.IsCompletedSuccessfully || c.IsCanceled || c.IsCompleted || c.IsFaulted)) await Task.Delay(50);

                        if (index == tempIndex)
                        {
                            if (RvDataSongs.Count > 0 || RvDataAlbums.Count > 0 || RvDataArtist.Count > 0)
                                ToggleSearchLoading(Search_status.Found);
                            else
                                ToggleSearchLoading(Search_status.NothingFound);
                        }
                    }
                }
                catch (Exception e)
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Snackbar.Make(this.Activity.Window.DecorView.RootView, e.Message, Snackbar.LengthShort).Show();
                    }, null);
                }
                await Task.Delay(25);
            }
        }

        private void ToggleSearchLoading(Search_status status)
        {
            switch (status)
            {
                case Search_status.Started:
                    {
                        ToggleSeachInfoFrame(true);
                        ToggleSearchBarLoading(false);
                        break;
                    }
                case Search_status.Searching:
                    {
                        if (RvDataSongs.Count == 0 && RvDataAlbums.Count == 0 && RvDataArtist.Count == 0)
                        {
                            ToggleSeachInfoFrame(true, true);
                        }
                        ToggleSearchBarLoading(true);
                        break;
                    }
                case Search_status.NothingFound:
                    {
                        ToggleSeachInfoFrame(true);
                        ToggleSearchBarLoading(false);
                        break;
                    }
                case Search_status.Found:
                    {
                        ToggleSeachInfoFrame(false);
                        ToggleSearchBarLoading(false);
                        break;
                    }
                default:
                    break;
            }

            void ToggleSeachInfoFrame(bool show, bool gif_show = false)
            {
                if (show)
                {
                    if (SearchEmpty.Visibility != ViewStates.Visible)
                        SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Visible);

                    if (gif_show)
                    {
                        if (Searching_Gif.Visibility != ViewStates.Visible)
                        {
                            Searching_Gif.Post(() => Searching_Gif.Visibility = ViewStates.Visible);
                            Search_start_image.Post(() => Search_start_image.Visibility = ViewStates.Invisible);
                            Application.SynchronizationContext.Post(_ => { Searching_Gif.StartAnimation(); }, null);
                        }
                    }
                    else
                    {
                        if (Searching_Gif.Visibility != ViewStates.Invisible)
                        {
                            Searching_Gif.Post(() => Searching_Gif.Visibility = ViewStates.Invisible);
                            Search_start_image.Post(() => Search_start_image.Visibility = ViewStates.Visible);
                            Application.SynchronizationContext.Post(_ => { Searching_Gif.StopAnimation(); }, null);
                        }
                    }
                }
                else
                {
                    if (SearchEmpty.Visibility != ViewStates.Gone)
                        SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Gone);
                }
            }

            void ToggleSearchBarLoading(bool show)
            {
                if (show)
                {
                    if (SearchLoading.Visibility != ViewStates.Visible)
                    {
                        SearchIcon.Post(() => SearchIcon.Visibility = ViewStates.Invisible);
                        SearchLoading.Post(() => SearchLoading.Visibility = ViewStates.Visible);
                    }
                }
                else
                {
                    if (SearchLoading.Visibility != ViewStates.Gone)
                    {
                        SearchLoading.Post(() => SearchLoading.Visibility = ViewStates.Invisible);
                        SearchIcon.Post(() => SearchIcon.Visibility = ViewStates.Visible);
                    }
                }
            }
        }

        private void ToggleSearchEmptyContainer(bool show)
        {
            if (show)
            {
                if (SearchEmpty.Visibility != ViewStates.Visible)
                {
                    SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Visible);
                }
            }
            else
            {
                if (SearchEmpty.Visibility != ViewStates.Gone)
                {
                    SearchEmpty.Post(() => SearchEmpty.Visibility = ViewStates.Gone);
                }
            }
        }

        public async Task SearchSong(string query)
        {
            await SearchBaseAsync<Songs>(RvDataSongs, SongsContainer, query, SongLimit);
        }

        public async Task SearchAlbums(string query)
        {
            await SearchBaseAsync<Album>(RvDataAlbums, AlbumsContainer, query, SongLimit);
        }

        public async Task SearchArtist(string query)
        {
            await SearchBaseAsync<Artist>(RvDataArtist, ArtistContainer, query, SongLimit);
        }

        public async Task SearchBaseAsync<T>(RvList<T> RvList, TextView Container, string query, int limit)
        {
            try
            {
                List<T> DataList = await ParentActivity.GetService().Search<T>(query);
                if (DataList != null && DataList.Count > 0)
                {
                    if (Container.Visibility == ViewStates.Gone)
                        Container.Post(() => Container.Visibility = ViewStates.Visible);

                    RvList.AddList(DataList.Take(limit).ToList());
                }
                else
                {
                    if (Container.Visibility == ViewStates.Visible)
                    {
                        Container.Post(() => Container.Visibility = ViewStates.Gone);
                        RvList.Clear();
                    }
                }
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Toast.MakeText(this.Context, e.Message, ToastLength.Short).Show();
                }, null);
            }
        }

        public override void ForceUpdate()
        {
            search.Text = "Search song, album, playlist and etc.";
            ToggleSearchLoading(Search_status.Started);
        }
    }
}