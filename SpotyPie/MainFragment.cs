using Android.Content;
using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.RecycleView;
using System;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class MainFragment : FragmentBase
    {
        //Recent albums
        public RvList<Album> RecentAlbums;

        //Best albums
        public RvList<Album> BestAlbums;

        //Best artists
        public RvList<Album> BestArtists;

        //Jump back albums
        public RvList<Album> JumpBack;

        //Top playlist
        public RvList<Album> TopPlaylist;

        //Holders
        ConstraintLayout RecentHolder;
        ConstraintLayout PlaylistHolder;
        ConstraintLayout BestHolder;
        ConstraintLayout JumpBackHolder;
        ProgressBar Loading;

        public override int LayoutId { get; set; } = Resource.Layout.home_layout;

        protected override void InitView()
        {
            Loading = RootView.FindViewById<ProgressBar>(Resource.Id.Loading);

            PlaylistHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.top_playlist_holder);
            RecentHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.recent_albums_holder);
            BestHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.best_albums_holder);
            JumpBackHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayout);

            Toggle(false, PlaylistHolder);
            Toggle(false, RecentHolder);
            Toggle(false, BestHolder);
            Toggle(false, JumpBackHolder);

            RecentAlbums = new BaseRecycleView<Album>(this, Resource.Id.recent_rv).Setup(LinearLayoutManager.Horizontal);
            BestAlbums = new BaseRecycleView<Album>(this, Resource.Id.best_albums_rv).Setup(LinearLayoutManager.Horizontal);
            //BestArtists = new BaseRecycleView<Album>(this, Resource.Id.best_artists_rv, BestArtistData).Setup();
            JumpBack = new BaseRecycleView<Album>(this, Resource.Id.albums_old_rv).Setup(LinearLayoutManager.Horizontal);
            //TopPlaylist = new BaseRecycleView<Album>(this, Resource.Id.playlist_rv, TopPlaylistData).Setup();
        }

        public void Toggle(bool state, ConstraintLayout layout)
        {
            //Show
            if (state)
            {
                if (Loading.Visibility == Android.Views.ViewStates.Visible)
                    Loading.Visibility = Android.Views.ViewStates.Gone;

                layout.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                layout.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        public void LoadData()
        {
            Task.Run(() => GetRecentAlbumsAsync(this.Context));

            Task.Run(() => GetPolularAlbumsAsync(this.Context));

            //Task.Run(() => GetPolularArtistsAsync(this.Context));

            Task.Run(() => GetOldAlbumsAsync(this.Context));
        }

        public async Task GetRecentAlbumsAsync(Context cnt)
        {
            try
            {
                var api = GetService();
                var albums = await api.GetRecent<Album>();
                InvokeOnMainThread(() =>
                {
                    Toggle(true, RecentHolder);
                    RecentAlbums.AddList(albums);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPolularAlbumsAsync(Context cnt)
        {
            try
            {
                var api = GetService();
                var albums = await api.GetPopular<Album>();
                InvokeOnMainThread(() =>
                {
                    Toggle(true, BestHolder);
                    BestAlbums.AddList(albums);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetOldAlbumsAsync(Context cnt)
        {
            try
            {
                var api = GetService();
                var albums = await api.GetOld<Album>();

                InvokeOnMainThread(() =>
                {
                    Toggle(true, JumpBackHolder);
                    JumpBack.AddList(albums);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task GetPlaylists(Context cnt)
        {
            try
            {
                var playlists = await GetService().GetListAsync<Playlist>(PlaylistType.Playlists);
                InvokeOnMainThread(() =>
                {
                    //TopPlaylistData = playlists;
                    //foreach (var x in playlists)
                    //{
                    //    TopPlaylist.Add(new BlockWithImage(x.Id, RvType.Playlist, x.Name, x.Created.ToString("yyyy-MM-dd"), x.ImageUrl));
                    //}
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override void ForceUpdate()
        {
            LoadData();
        }
    }
}