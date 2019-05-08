using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Felipecsl.GifImageViewLib;
using Mobile_Api.Interfaces;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Helpers;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class Search : FragmentBase, ViewTreeObserver.IOnGlobalLayoutListener
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

        private EditText search;
        private ImageView SearchIcon;
        private ProgressBar SearchLoading;

        private GifImageView Searching_Gif;
        private ImageView Search_start_image;

        private BaseRecycleView<Songs> RvBaseSong { get; set; }

        private BaseRecycleView<Album> RvBaseAlbum { get; set; }

        private BaseRecycleView<Artist> RvBaseArtist { get; set; }

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

            RootView.ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }

        private void SetupSearchLoadingGif()
        {
            Searching_Gif = RootView.FindViewById<GifImageView>(Resource.Id.seach_gif);
            Searching_Gif.Visibility = ViewStates.Gone;

            Stream input = this.Activity.Resources.OpenRawResource(Resource.Drawable.seaching_image);//this.Activity.Assets.Open("seaching_image.gif");
            byte[] bytes = ConvertFileToByteArray.Convert(input);
            Searching_Gif.SetBytes(bytes);
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
                GetState().ToggleMiniPlayer(false);
                if (search.Text.Contains("Search"))
                    search.Text = "";
            }
            else
            {
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
            long tempIndex;
            Task SongSearchTask;
            Task AlbumSearchTask;
            Task ArtistSearchTask;
            while (SearchNow)
            {
                try
                {
                    if (query != search.Text && !string.IsNullOrEmpty(search.Text) && search.Text != "Search song, album, playlist and etc.")
                    {
                        query = search.Text;
                        tempIndex = ++index;

                        ToggleSearchLoading(Search_status.Searching);

                        SongSearchTask = Task.Run(() => SearchSong(search.Text));
                        AlbumSearchTask = Task.Run(() => SearchAlbums(search.Text));
                        ArtistSearchTask = Task.Run(() => SearchArtist(search.Text));

                        while (!(SongSearchTask.IsCompletedSuccessfully || SongSearchTask.IsCanceled || SongSearchTask.IsCompleted || SongSearchTask.IsFaulted)) await Task.Delay(50);
                        while (!(AlbumSearchTask.IsCompletedSuccessfully || AlbumSearchTask.IsCanceled || AlbumSearchTask.IsCompleted || AlbumSearchTask.IsFaulted)) await Task.Delay(50);
                        while (!(ArtistSearchTask.IsCompletedSuccessfully || ArtistSearchTask.IsCanceled || ArtistSearchTask.IsCompleted || ArtistSearchTask.IsFaulted)) await Task.Delay(50);

                        if (index == tempIndex)
                        {
                            if (RvBaseSong.GetData().Count > 0 || RvBaseAlbum.GetData().Count > 0 || RvBaseArtist.GetData().Count > 0)
                                ToggleSearchLoading(Search_status.Found);
                            else
                                ToggleSearchLoading(Search_status.NothingFound);
                        }
                    }
                    else if (string.IsNullOrEmpty(query))
                    {
                        ToggleSearchLoading(Search_status.NothingFound);
                    }
                }
                catch (Exception e)
                {
                    Activity.RunOnUiThread(() =>
                        Snackbar.Make(this.Activity.Window.DecorView.RootView, e.Message, Snackbar.LengthShort).Show()
                    );
                }
                await Task.Delay(250);
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
                        if (RvBaseSong.GetData().Count == 0 && RvBaseAlbum.GetData().Count == 0 && RvBaseArtist.GetData().Count == 0)
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
                            Activity.RunOnUiThread(() => { Searching_Gif.StartAnimation(); });
                        }
                    }
                    else
                    {
                        if (Searching_Gif.Visibility != ViewStates.Invisible)
                        {
                            Searching_Gif.Post(() => Searching_Gif.Visibility = ViewStates.Invisible);
                            Search_start_image.Post(() => Search_start_image.Visibility = ViewStates.Visible);
                            Activity.RunOnUiThread(() => { Searching_Gif.StopAnimation(); });
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
            await SearchBaseAsync<Songs>(RvBaseSong.GetData(), SongsContainer, query, SongLimit, RvType.SongWithImage);
        }

        public async Task SearchAlbums(string query)
        {
            await SearchBaseAsync<Album>(RvBaseAlbum.GetData(), AlbumsContainer, query, SongLimit, RvType.AlbumGrid);
        }

        public async Task SearchArtist(string query)
        {
            await SearchBaseAsync<Artist>(RvBaseArtist.GetData(), ArtistContainer, query, SongLimit, RvType.ArtistGrid);
        }

        public async Task SearchBaseAsync<T>(RvList<T> RvList, TextView Container, string query, int limit, RvType type) where T : IBaseInterface
        {
            try
            {
                List<T> DataList = await ParentActivity.GetAPIService().SearchAsync<T>(query);
                if (DataList != null && DataList.Count > 0)
                {
                    if (Container.Visibility == ViewStates.Gone)
                        Container.Post(() => Container.Visibility = ViewStates.Visible);

                    FormatView(ref DataList, type);
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

        private void FormatView<T>(ref List<T> dataList, RvType type) where T : IBaseInterface
        {
            if (dataList == null || dataList.Count == 0)
                return;

            if (dataList.Count == 1)
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvBaseAlbum.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvBaseArtist.SetLayoutManager(RecycleView.Enums.LayoutManagers.Linear_vertical);
                    }
                }, null);

                if (typeof(T) != typeof(Songs))
                    dataList.First().SetModelType(RvType.BigOne);
                else
                    dataList.First().SetModelType(type);
            }
            else
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (typeof(T) == typeof(Album))
                    {
                        RvBaseAlbum.SetLayoutManager(RecycleView.Enums.LayoutManagers.Grind_2_col);
                    }
                    else if (typeof(T) == typeof(Artist))
                    {
                        RvBaseArtist.SetLayoutManager(RecycleView.Enums.LayoutManagers.Grind_2_col);
                    }
                }, null);

                dataList.ForEach(x => x.SetModelType(type));
            }
        }

        public override void ForceUpdate()
        {
            search.Text = "Search song, album, playlist and etc.";
            ToggleSearchLoading(Search_status.Started);
            if (RvBaseSong == null)
            {
                RvBaseSong = new BaseRecycleView<Songs>(this, Resource.Id.song_rv);
                RvBaseSong.Setup(RecycleView.Enums.LayoutManagers.Linear_vertical);
                RvBaseSong.DisableScroolNested();
            }

            if (RvBaseAlbum == null)
            {
                RvBaseAlbum = new BaseRecycleView<Album>(this, Resource.Id.albums_rv);
                RvBaseAlbum.Setup(RecycleView.Enums.LayoutManagers.Grind_2_col);
                RvBaseAlbum.DisableScroolNested();
            }

            if (RvBaseArtist == null)
            {
                RvBaseArtist = new BaseRecycleView<Artist>(this, Resource.Id.artists_rv);
                RvBaseArtist.Setup(RecycleView.Enums.LayoutManagers.Grind_2_col);
                RvBaseArtist.DisableScroolNested();
            }

            SearchNow = true;
            Task.Run(() => Checker());
        }

        public override void ReleaseData()
        {
            if (RvBaseSong != null)
            {
                RvBaseSong.Dispose();
                RvBaseSong = null;
            }

            if (RvBaseAlbum != null)
            {
                RvBaseAlbum.Dispose();
                RvBaseAlbum = null;
            }

            if (RvBaseArtist != null)
            {
                RvBaseArtist.Dispose();
                RvBaseArtist = null;
            }
        }

        public void OnGlobalLayout()
        {
            int heightDiff = RootView.RootView.Height - RootView.Height;
            if (heightDiff > 500)
            {
                GetState().ToggleBotNav(false);
                GetState().ToggleMiniPlayer(false);
            }
            else
            {
                GetState().ToggleBotNav(true);
                GetState().ToggleMiniPlayer(true);
            }
        }
    }
}