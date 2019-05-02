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
        //Best artists
        //public RvList<Album> BestArtists;

        //Top playlist
        //public RvList<Album> TopPlaylist;

        //Recent albums
        private BaseRecycleView<Album> RecentAlbums { get; set; }

        //Best albums
        private BaseRecycleView<Album> BestAlbums { get; set; }

        //Jump back albums
        private BaseRecycleView<Album> JumpBack { get; set; }

        //Holders
        private ConstraintLayout RecentHolder;
        private ConstraintLayout PlaylistHolder;
        private ConstraintLayout BestHolder;
        private ConstraintLayout JumpBackHolder;
        private ProgressBar Loading;

        private ImageButton Settings;

        public override int LayoutId { get; set; } = Resource.Layout.home_layout;

        protected override void InitView()
        {
            Loading = RootView.FindViewById<ProgressBar>(Resource.Id.Loading);

            PlaylistHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.top_playlist_holder);
            RecentHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.recent_albums_holder);
            BestHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.best_albums_holder);
            JumpBackHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayout);

            Settings = RootView.FindViewById<ImageButton>(Resource.Id.settings);
            Settings.Click += Settings_Click;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Activity.StartActivity(typeof(SettingsActivity));
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

        public override void ForceUpdate()
        {
            Toggle(false, PlaylistHolder);
            Toggle(false, RecentHolder);
            Toggle(false, BestHolder);
            Toggle(false, JumpBackHolder);

            if (RecentAlbums == null)
            {
                RecentAlbums = new BaseRecycleView<Album>(this, Resource.Id.recent_rv);
                RecentAlbums.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
            }

            if (BestAlbums == null)
            {
                BestAlbums = new BaseRecycleView<Album>(this, Resource.Id.best_albums_rv);
                BestAlbums.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
            }

            if (JumpBack == null)
            {
                JumpBack = new BaseRecycleView<Album>(this, Resource.Id.albums_old_rv);
                JumpBack.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
            }

            Task.Run(() => GetAPIService().GetRecentAlbumsAsync(RecentAlbums.GetData(), () => { Toggle(true, RecentHolder); }));
            Task.Run(() => GetAPIService().GetPolularAlbumsAsync(BestAlbums.GetData(), () => { Toggle(true, BestHolder); }));
            Task.Run(() => GetAPIService().GetOldAlbumsAsync(JumpBack.GetData(), () => { Toggle(true, JumpBackHolder); }));
        }

        public override void ReleaseData()
        {
            if (RecentAlbums != null)
            {
                RecentAlbums.Dispose();
                RecentAlbums = null;
            }

            if (BestAlbums != null)
            {
                BestAlbums.Dispose();
                BestAlbums = null;
            }

            if (JumpBack != null)
            {
                JumpBack.Dispose();
                JumpBack = null;
            }
        }
    }
}