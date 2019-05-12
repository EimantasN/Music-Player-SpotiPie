using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SpotyPie.Models;
using System;
using System.Collections.Generic;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie.Base
{
    public abstract class ActivityBase : AppCompatActivity
    {
        private static int FrameLayoutId { get; set; } = 100000;

        public abstract int LayoutId { get; set; }

        protected virtual int FirstLayerFragmentHolder { get; set; } = Resource.Id.content_frame;

        public CustomFragmetManager FManager { get; set; }

        public FragmentBase MYParentFragment;

        public Stack<dynamic> FragmentStack { get; set; }

        private ViewGroup ParentView { get; set; }

        private FrameLayout FragmentFrame;

        private FrameLayout PlayerFrame;

        private ProgressBar FragmentLoading;

        public bool IsFragmentLoadedAdded = false;

        private API Api_service { get; set; }

        public SupportFragmentManager mSupportFragmentManager;

        public abstract dynamic GetInstance();

        private int GetLayout()
        {
            return LayoutId;
        }

        public virtual void LoadBaseFragment()
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(GetLayout());

            SupportFragmentManager.BackStackChanged += SupportFragmentManager_BackStackChanged;
            InitView();
        }

        private void SupportFragmentManager_BackStackChanged(object sender, System.EventArgs e)
        {
            var snack = Snackbar.Make(Window.DecorView.RootView, $"BackStack - {SupportFragmentManager.BackStackEntryCount}", Snackbar.LengthIndefinite);
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
                FragmentFrame = FindViewById<FrameLayout>(FirstLayerFragmentHolder);
                FragmentFrame.Visibility = ViewStates.Gone;
                FragmentLoading = FindViewById<ProgressBar>(Resource.Id.fragmentloading);
                FragmentLoading.Visibility = ViewStates.Gone;
            }
        }


        public API GetAPIService()
        {
            if (Api_service == null)
                return Api_service = new API(new Mobile_Api.Service(), this);
            return Api_service;
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
            ParentView.RemoveView(PlayerFrame);
            PlayerFrame = null;
        }

        public virtual void FragmentLoaded()
        {
            if (IsFragmentLoadedAdded && FragmentLoading.Visibility == ViewStates.Visible)
            {
                FragmentLoading.Visibility = ViewStates.Gone;
            }
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

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null, bool AddToBackButtonStack = true)
        {
            GetFManager().LoadFragmentInner(switcher, jsonModel, AddToBackButtonStack);
        }

        protected override void OnDestroy()
        {
            FragmentStack = null;
            FManager?.OnStop();
            base.OnDestroy();
        }

        public int GetFragmentId()
        {
            return ++FrameLayoutId;
        }

        //Do not use this for view getting
        public abstract int GetParentView();

        private void GetViewToInsert()
        {
            if (ParentView == null)
            {
                ParentView = FindViewById<ViewGroup>(GetParentView());

                if (ParentView == null)
                    throw new Exception("View not found");
            }
        }

        public int GetFragmentViewId(bool isPlayer = false)
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

                    GetViewToInsert();

                    ParentView.AddView(FragmentFrame);

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

                    GetViewToInsert();

                    ParentView.AddView(PlayerFrame);

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
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
                transaction = null;
                fragment = null;
            }
        }
    }
}