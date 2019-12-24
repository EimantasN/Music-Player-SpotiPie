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
using SpotyPie.Music.Manager;

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

        public bool IsFragmentLoadedAdded = false;

        private API Api_service { get; set; }

        public SupportFragmentManager mSupportFragmentManager;

        public abstract dynamic GetInstance();

        private int GetLayout() { return LayoutId; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayout());
            Fabric.Fabric.With(this, new Crashlytics.Crashlytics());

            SupportFragmentManager.BackStackChanged += SupportFragmentManager_BackStackChanged;

            SongManager.SetAcitivityRef(this);
            InitView();
        }

        public virtual void LoadBaseFragment() { }

        protected override void OnResume()
        {
            SetNavigationBarColor();
            base.OnResume();
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
        }

        public virtual void FragmentLoaded() { }

        public virtual void LoadFragment(dynamic switcher, string jsonModel = null) { }

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

        public virtual int GetFragmentViewId()
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
            Type serviceMusic = typeof(MusicService);
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (serviceMusic.Name.Equals(service.Service.ClassName))
                {
                    return;
                }
            }
            this.StartService(new Intent(this, typeof(MusicService)));
        }
    }
}