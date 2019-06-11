using Android.Support.Constraints;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.RecycleView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie
{
    public class MainFragment : FragmentBase
    {
        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;
        public override int LayoutId { get; set; } = Resource.Layout.home_layout;

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
            Task.Run(() => GetAPIService().GetPolularAlbumsAsync(BestAlbums.GetData(), () => { Toggle(true, BestHolder); }, this.Activity));
            Task.Run(() => GetAPIService().GetOldAlbumsAsync(JumpBack.GetData(), () => { Toggle(true, JumpBackHolder); }, this.Activity));
        }

        private void LoadRecentAlbums()
        {
            try
            {
                Realm realm = Realm.GetInstance();
                var albums = realm.All<Realm_Album>().OrderBy(x => x.LastActiveTime).Take(8).ToList();
                if (albums != null && albums.Count != 0)
                {
                    List<Album> OrginalAlbums = new List<Album>();
                    foreach (var x in albums)
                    {
                        OrginalAlbums.Add(new Album(x));
                    }
                }
                else
                {
                    Toggle(false, RecentHolder);
                }

                Task.Run(() => GetAPIService().GetRecentAlbumsAsync(RecentAlbums.GetData(), () => { Toggle(true, RecentHolder); }, this.Activity));
            }
            catch (Exception e)
            {
            }
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

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override void LoadFragment(dynamic switcher)
        {
            throw new NotImplementedException();
        }
    }
}