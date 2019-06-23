using Android.Content;
using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
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
        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;
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
            Loading.Visibility = Android.Views.ViewStates.Gone;

            PlaylistHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.top_playlist_holder);
            RecentHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.recent_albums_holder);
            BestHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.best_albums_holder);
            JumpBackHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.constraintLayout);

            Settings = RootView.FindViewById<ImageButton>(Resource.Id.settings);
            Settings.Click += Settings_Click;

            Toggle(false, PlaylistHolder);
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
            LoadRecentData();
            LoadPopularAlbums();
            LoadOldAlbums();
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

        #region Data loading

        private void LoadOldAlbums()
        {
            if (JumpBack == null)
            {
                JumpBack = new BaseRecycleView<Album>(this, Resource.Id.albums_old_rv);
                JumpBack.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                JumpBack.SetFocusable(false);
            }

            using (Realm realm = Realm.GetInstance())
            {
                var albums = realm.All<Realm_Album>().OrderBy(x => x.LastActiveTime).ToList();
                if (albums == null || albums.Count == 0)
                {
                    Toggle(false, JumpBackHolder);
                }
                else
                {
                    List<Album> list = new List<Album>();
                    foreach (var x in albums.Take(8))
                    {
                        list.Add(new Album(x));
                    }
                    JumpBack.GetData().AddList(list);
                }
            }
            Task.Run(async () =>
            {
                var albums = await GetAPIService().GetOldAlbumsAsync();
                if (albums != null && albums.Count != 0)
                    LoadData(JumpBack.GetData(), albums, () => { Toggle(true, JumpBackHolder); });
            });
        }

        private void LoadPopularAlbums()
        {
            if (BestAlbums == null)
            {
                BestAlbums = new BaseRecycleView<Album>(this, Resource.Id.best_albums_rv);
                BestAlbums.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                BestAlbums.SetFocusable(false);
            }

            using (Realm realm = Realm.GetInstance())
            {
                var albums = realm.All<Realm_Album>().OrderByDescending(x => x.Popularity).ToList();
                if (albums == null || albums.Count == 0)
                {
                    Toggle(false, BestHolder);
                }
                else
                {
                    List<Album> list = new List<Album>();
                    foreach (var x in albums.Take(8))
                    {
                        list.Add(new Album(x));
                    }
                    BestAlbums.GetData().AddList(list);
                }
            }
            Task.Run(async () =>
            {
                List<Album> albums = await GetAPIService().GetPolularAlbumsAsync();
                if (albums != null && albums.Count != 0)
                    LoadData(BestAlbums.GetData(), albums, () => { Toggle(true, BestHolder); });
            });
        }

        private void LoadRecentData()
        {
            if (RecentAlbums == null)
            {
                RecentAlbums = new BaseRecycleView<Album>(this, Resource.Id.recent_rv);
                RecentAlbums.Setup(RecycleView.Enums.LayoutManagers.Linear_horizontal);
                RecentAlbums.SetFocusable(false);
            }

            using (Realm realm = Realm.GetInstance())
            {
                List<Realm_Album> albums = realm.All<Realm_Album>().OrderByDescending(x => x.LastActiveTime).ToList();
                if (albums == null || albums.Count == 0)
                {
                    Toggle(false, RecentHolder);
                }
                else
                {
                    List<Album> list = new List<Album>();
                    foreach (var x in albums.Take(8))
                    {
                        list.Add(new Album(x));
                    }
                    RecentAlbums.GetData().AddList(list);
                }
            }
            Task.Run(async () =>
            {
                List<Album> albums = await GetAPIService().GetRecentAlbumsAsync();
                if(albums != null && albums.Count != 0)
                    LoadData(RecentAlbums.GetData(), albums, () => { Toggle(true, RecentHolder); });
            });
        }

        private void LoadData(RvList<Album> list, List<Album> albumsData, Action action)
        {
            RunOnUiThread(() => { action?.Invoke(); });
            list.AddList(albumsData);
        }

        #endregion

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }

        public override void LoadFragment(dynamic switcher)
        {
            return;
        }
    }
}