using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public abstract class FragmentBase : SupportFragment
    {
        private static int FrameLayoutId { get; set; } = 1;

        public FragmentBase CurrentFragment;

        public FragmentBase MYParentFragment;

        private Stack<dynamic> FragmentStack { get; set; }

        public abstract int LayoutId { get; set; }

        private ViewGroup ParentView { get; set; }

        protected string JsonModel { get; set; }

        protected View RootView;

        protected ActivityBase ParentActivity;

        public FrameLayout FragmentFrame;

        public View GetView()
        {
            return RootView;
        }

        private int GetLayout()
        {
            return LayoutId;
        }

        public int GetFragmentId()
        {
            return ++FrameLayoutId;
        }

        private void GetViewToInsert()
        {
            if (ParentView == null)
            {
                ParentView = RootView.FindViewById<ViewGroup>(GetParentView());

                if (ParentView == null)
                    throw new Exception("View not found");
            }
        }

        //Do not use this for view getting
        public abstract int GetParentView();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(GetLayout(), container, false);
            ParentActivity = (ActivityBase)Activity;
            InitView();
            return RootView;
        }

        public abstract void ForceUpdate();
        public abstract void ReleaseData();

        public void Hide()
        {
            RootView.Alpha = 0.0f;
            RootView.TranslationX = 10000;
            ParentActivity = null;
            ReleaseData();
        }

        public void Show()
        {
            RootView.Alpha = 1f;
            RootView.TranslationX = 0;
        }

        public Current_state GetState()
        {
            if (ParentActivity == null)
                ParentActivity = (ActivityBase)Activity;

            return ParentActivity?.GetInstance()?.GetState();
        }

        public SupportFragment GetCurrentFragment()
        {
            return ParentActivity.GetInstance().FirstLayerFragment;
        }

        public API GetAPIService()
        {
            return ParentActivity.GetInstance().GetAPIService();
        }

        public void InvokeOnMainThread(Action action)
        {
            Activity.RunOnUiThread(() =>
            {
                action.Invoke();
            });
        }

        protected abstract void InitView();

        public void LoadAlbum(Album album)
        {
            ParentActivity.GetInstance().LoadAlbum(album);
        }

        public void LoadArtist(Artist artist)
        {
            ParentActivity.GetInstance().LoadArtist(artist);
        }

        public override void OnResume()
        {
            ForceUpdate();
            ParentActivity?.FragmentLoaded();
            base.OnResume();
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnStop()
        {
            ReleaseData();
            base.OnStop();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public void RunOnUiThread(Action action)
        {
            Activity?.RunOnUiThread(() =>
            {
                action?.Invoke();
            });
        }

        public void RemoveMe()
        {
            ParentActivity?.OnBackPressed();
        }

        public void SendData(string data)
        {
            this.JsonModel = data;
        }

        public T GetModel<T>()
        {
            if (!string.IsNullOrEmpty(JsonModel))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(JsonModel);
                }
                catch //Ignored
                {
                }
            }
            Snackbar snacbar = Snackbar.Make(RootView, "Failed to load song details", Snackbar.LengthLong);
            snacbar.SetAction("Ok", (view) =>
            {
                snacbar.Dismiss();
                snacbar.Dispose();
            });
            snacbar.Show();
            return default(T);
        }

        private Stack<dynamic> GetFragmentStack()
        {
            if (FragmentStack == null)
                FragmentStack = new Stack<dynamic>();
            return FragmentStack;
        }

        public abstract void LoadFragment(dynamic switcher);

        public void AddParent(FragmentBase parent)
        {
            this.MYParentFragment = parent;
        }

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null)
        {
            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;

            GetFragmentStack().Push(switcher);
            LoadFragment(switcher);
            CurrentFragment.AddParent(this);

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                CurrentFragment.SendData(jsonModel);

            if (!CurrentFragment.IsAdded)
            {
                ChildFragmentManager.BeginTransaction()
                .Replace(GetFragmentViewId(), CurrentFragment)
                .Commit();

                ParentActivity?.SetBackBtn(() => { ParentActivity?.RemoveCurrentFragment(ChildFragmentManager, CurrentFragment); });
            }
            else
            {
                CurrentFragment?.ForceUpdate();
            }
        }

        public void ReloadMyParentFragment()
        {
            if (MYParentFragment != null)
                MYParentFragment.ForceUpdate();
            else
                ParentActivity?.LoadBaseFragment();
        }

        public int GetFragmentViewId()
        {
            if (FragmentFrame == null)
            {
                FragmentFrame = new FrameLayout(this.Context);

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
    }
}