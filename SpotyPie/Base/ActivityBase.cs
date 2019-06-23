using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Mobile_Api.Models.Realm;
using Realms;
using SpotyPie.Enums;
using SpotyPie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using MusicList = Mobile_Api.Models.Realm.Music;
using Android.Content;
using SpotyPie.Music;

namespace SpotyPie.Base
{
    public abstract class ActivityBase : AppCompatActivity
    {
        private Current_state State { get; set; }

        private static int FrameLayoutId { get; set; } = 100000;

        public abstract NavigationColorState NavigationBtnColorState { get; set; }

        public abstract LayoutScreenState ScreenState { get; set; }

        public abstract int LayoutId { get; set; }

        public CustomFragmetManager FManager { get; set; }

        public FragmentBase MYParentFragment;

        public Stack<dynamic> FragmentStack { get; set; }

        private ViewGroup ParentView { get; set; }

        private ViewGroup PlayerView { get; set; }

        private Player.Player Player { get; set; }

        public Player.Player GetPlayer()
        {
            if (Player == null)
                Player = new Player.Player();
            return Player;
        }

        private FrameLayout FragmentFrame;

        private FrameLayout PlayerFrame;

        public bool IsFragmentLoadedAdded = false;

        private API Api_service { get; set; }

        public SupportFragmentManager mSupportFragmentManager;

        public EventArgs e = null;

        private Realm Realm;

        public IRealmCollection<MusicList> SongList;
        public IRealmCollection<Realm_Songs> List;

        public delegate void SongListChangeHandler(IRealmCollection<MusicList> list, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);

        public SongListChangeHandler SongListHandler;

        public delegate void CurrentSongStateChangeHandler(Realm_Songs currentSong, EventArgs e);

        public CurrentSongStateChangeHandler CurrentSongHandler;

        public abstract dynamic GetInstance();

        private int GetLayout() { return LayoutId; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayout());

            SupportFragmentManager.BackStackChanged += SupportFragmentManager_BackStackChanged;
            InitView();
        }

        public virtual void LoadBaseFragment()
        {

        }

        private void InitRealChangeLisiner()
        {
            if (Realm == null || Realm.IsClosed)
            {
                try
                {
                    Realm = Realm.GetInstance();
                }
                catch (Realms.Exceptions.RealmMigrationNeededException )
                {
                    Realm.DeleteRealm(RealmConfiguration.DefaultConfiguration);
                    Realm = Realm.GetInstance();
                }
                Realm.RealmChanged += Realm_RealmChanged1;
                SongList = Realm.All<MusicList>().AsRealmCollection();

                //var c = SongList[0].Song.Id;
                //List = Realm.All<Realm_Songs>().Where(x => x.Id == c).AsRealmCollection();

                //for (int i = 1; i < SongList.Count; i++)
                //{
                //    c = SongList[i].Song.Id;
                //    List.Append(Realm.All<Realm_Songs>().First(x => x.Id == c));
                //}

                //var a = List.Count;
                //for (int i = 0; i < List.Count(); i++)
                //    List[i].PropertyChanged += SongList_PropertyChanged1;

                SongList.CollectionChanged += SongList_CollectionChanged;
            }
        }

        private void Realm_RealmChanged1(object sender, EventArgs e)
        {
            SongListHandler?.Invoke(SongList, null);
        }

        private void SongList_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SongListHandler?.Invoke(SongList, null);
        }

        private void SongList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SongListHandler?.Invoke(SongList, e);
            //if (e.NewItems != null && e.NewItems.Count != 0)
            //{
            //    for (int i = 0; i < e.NewItems.Count; i++)
            //    {
            //        var song = (MusicList)e.NewItems[i];
            //        var id = song.Song.Id;
            //        if (!List.Any(x => x.Id == song.Song.Id))
            //        {
            //            List.Append(Realm.All<Realm_Songs>().First(x => x.Id == id));
            //            List.Last().PropertyChanged += SongList_PropertyChanged1;
            //        }
            //    }
            //}

            //for (int i = 0; i < List.Count; i++)
            //{
            //    if (!SongList.Any(x => x.Song.Id == List[i].Id))
            //    {
            //        List[i].PropertyChanged -= SongList_PropertyChanged1;
            //    }
            //}
        }

        private void SongList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentCount":
                    {
                        break;
                    }
                case "LastCount":
                    {
                        break;
                    }
                case "PlayingSong":
                    {
                        //CurrentSongHandler?.Invoke(new Songs(SongList.PlayingSong), e);
                        break;
                    }
                case "IsPlaying":
                    {
                        break;
                    }
                case "UpdatedAt":
                    {
                        break;
                    }
                case "Songs":
                    {
                        break;
                    }
            }
        }

        private void Song_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //CurrentSongHandler?.Invoke(sender, e);
        }

        private void Realm_RealmChanged(object sender, EventArgs e)
        {
            //Handler?.Invoke(sender, e);
        }

        protected override void OnResume()
        {
            SetNavigationBarColor();
            InitRealChangeLisiner();
            base.OnResume();
        }

        protected override void OnRestart()
        {
            InitRealChangeLisiner();
            base.OnRestart();
        }

        protected override void OnStop()
        {
            if (Realm != null)
            {
                if (!Realm.IsClosed)
                {
                    Realm.Dispose();
                    Realm = null;
                }
            }
            base.OnStop();
        }

        private void SupportFragmentManager_BackStackChanged(object sender, EventArgs e)
        {
            var snack = Snackbar.Make(Window.DecorView.RootView, $"BackStack - {SupportFragmentManager.BackStackEntryCount}", Snackbar.LengthIndefinite);
            snack.SetAction("Ok", (view) =>
            {
                snack.Dismiss();
                snack.Dispose();
            });
            snack.Show();
        }

        public void ShowMessage(string text)
        {
            var snack = Snackbar.Make(Window.DecorView.RootView, text, Snackbar.LengthShort);
            snack.SetAction("Ok", (view) =>
            {
                snack.Dismiss();
                snack.Dispose();
            });
            snack.Show();
        }

        protected virtual void InitView()
        {
            mSupportFragmentManager = SupportFragmentManager;
            if (IsFragmentLoadedAdded)
            {
                //FragmentFrame = FindViewById<FrameLayout>(FirstLayerFragmentHolder);
                //FragmentFrame.Visibility = ViewStates.Gone;
                //FragmentLoading = FindViewById<ProgressBar>(Resource.Id.fragmentloading);
                //FragmentLoading.Visibility = ViewStates.Gone;
            }
        }

        public API GetAPIService()
        {
            if (Api_service == null)
                return Api_service = new API(new Mobile_Api.Service(), this);
            return Api_service;
        }

        public Current_state GetState()
        {
            if (State == null)
                State = new Current_state(this);
            return State;
        }

        public override void OnBackPressed()
        {
            if (GetFManager().CheckBackButton())
                base.OnBackPressed();
            else
            {
                //LoadFragmentInner(GetFragmentStack().Pop());
                //if (GetFragmentStack().Count == 0)
                //{
                //    if (IsFragmentLoadedAdded)
                //    {
                //        if (FragmentFrame.Visibility == ViewStates.Visible)
                //            FragmentFrame.Visibility = ViewStates.Gone;

                //        if (FragmentLoading.Visibility == ViewStates.Visible)
                //            FragmentLoading.Visibility = ViewStates.Gone;
                //    }
                //}
            }
        }

        public void RemovePlayerView()
        {
            if (PlayerFrame != null)
            {
                ParentView.RemoveView(PlayerFrame);
                PlayerFrame = null;
            }
        }

        public virtual void FragmentLoaded()
        {
            //if (IsFragmentLoadedAdded && FragmentLoading.Visibility == ViewStates.Visible)
            //{
            //    FragmentLoading.Visibility = ViewStates.Gone;
            //}
        }

        public virtual void LoadFragment(dynamic switcher, string jsonModel = null)
        {
        }

        public void AddParent(FragmentBase parent)
        {
            this.MYParentFragment = parent;
        }

        public CustomFragmetManager GetFManager()
        {
            if (FManager == null)
                FManager = new CustomFragmetManager(this);

            return FManager;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null, bool AddToBackButtonStack = true, LayoutScreenState screen = LayoutScreenState.Holder)
        {
            GetFManager().LoadFragmentInner(switcher, jsonModel, AddToBackButtonStack, screen);

            SetScreen(screen);
        }

        protected override void OnDestroy()
        {
            FragmentStack = null;
            base.OnDestroy();
        }

        public int GetFragmentId()
        {
            return ++FrameLayoutId;
        }

        //Do not use this for view getting
        public abstract int GetParentView(bool Player = false);

        private void GetViewToInsert(bool Player)
        {
            if (Player)
            {
                if (PlayerView == null)
                {
                    PlayerView = FindViewById<ViewGroup>(GetParentView(true));
                    if (PlayerView == null)
                        throw new Exception("Palyer viewgroup not found");
                }
            }
            else
            {
                if (ParentView == null)
                {
                    ParentView = FindViewById<ViewGroup>(GetParentView());
                    if (ParentView == null)
                        throw new Exception("Viewgroup not found");
                }
            }
        }

        public virtual int GetFragmentViewId(bool isPlayer = false)
        {
            if (!isPlayer)
            {
                if (FragmentFrame == null)
                {
                    FragmentFrame = new FrameLayout(this.ApplicationContext);

                    FragmentFrame.LayoutParameters = new ConstraintLayout.LayoutParams(
                        ConstraintLayout.LayoutParams.MatchParent,
                        ConstraintLayout.LayoutParams.MatchParent);

                    FragmentFrame.Id = GetFragmentId();

                    GetViewToInsert(false);

                    ParentView.AddView(FragmentFrame);

                    FragmentFrame.BringToFront();
                    return FragmentFrame.Id;
                }
                else
                    return FragmentFrame.Id;
            }
            else
            {
                if (PlayerFrame == null)
                {
                    PlayerFrame = new FrameLayout(this.ApplicationContext);

                    PlayerFrame.LayoutParameters = new ConstraintLayout.LayoutParams(
                        ConstraintLayout.LayoutParams.MatchParent,
                        ConstraintLayout.LayoutParams.MatchParent);

                    PlayerFrame.Id = int.MaxValue - 1;

                    GetViewToInsert(true);

                    PlayerView.AddView(PlayerFrame);

                    PlayerFrame.BringToFront();
                    return PlayerFrame.Id;
                }
                else
                {
                    PlayerFrame.BringToFront();
                    return PlayerFrame.Id;
                }
            }
        }

        public void RemoveCurrentFragment(SupportFragmentManager fragmentManager, FragmentBase fragment)
        {
            if (fragment != null)
            {
                fragment.ReleaseData();
                var transaction = fragmentManager.BeginTransaction();
                transaction.Remove(fragment);
                transaction.Commit();
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitExitMask);
                transaction = null;
                fragment = null;
            }
        }

        public Android.Media.Session.MediaController GetMediaController()
        {
            return MediaController;
        }

        public void SetNavigationBarColor(NavigationColorState state = NavigationColorState.Default)
        {
            if (state != NavigationColorState.Default)
                NavigationBtnColorState = state;
            switch (NavigationBtnColorState)
            {
                case NavigationColorState.Main:
                    Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#222327"));
                    break;
                case NavigationColorState.Player:
                    Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#000000"));
                    break;
                case NavigationColorState.Settings:
                    Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#222222"));
                    break;
            }
        }

        public abstract void SetScreen(LayoutScreenState screen);

        public void StartPlayer()
        {
            var intent = new Intent(this, typeof(Player.Player));
            intent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
        }

        protected void StartMusicService()
        {
            Task.Run(() =>
            {
                Type serviceMusic = typeof(MusicService);
                ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);
                foreach (var service in manager.GetRunningServices(int.MaxValue))
                {
                    if (serviceMusic.Name.Equals(service.Service.ClassName))
                    {
                        return;
                    }
                }
                RunOnUiThread(() =>
                {
                    this.StartService(new Intent(this, typeof(MusicService)));
                });
            });
        }
    }
}