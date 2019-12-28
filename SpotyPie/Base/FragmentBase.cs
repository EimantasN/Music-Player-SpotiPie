using Android.OS;
using Android.Support.Constraints;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Enums;
using SpotyPie.Models;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using System;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public abstract class FragmentBase : SupportFragment, IAutoFragmentManagement
    {
        private static int FrameLayoutId { get; set; } = 1;

        public SupportFragmentManager SupportFragmentManager => ChildFragmentManager;

        protected abstract LayoutScreenState ScreenState { get; set; }

        public virtual NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Default;

        public abstract int LayoutId { get; set; }

        public SpotyPieFragmetManager FManager { get; set; }

        private ViewGroup ParentView { get; set; }

        private FrameLayout PlayerFrame;

        protected string JsonModel { get; set; }

        protected View RootView;

        protected ActivityBase ParentActivity { get; private set; }

        public FrameLayout FragmentFrame;

        public View GetView()
        {
            return RootView;
        }

        public ActivityBase GetActivity()
        {
            if (ParentActivity == null)
                ParentActivity = (ActivityBase)Activity;
            return ParentActivity;
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
            return GetActivity()?.GetState();
        }

        public API GetAPIService()
        {
            return GetActivity().GetInstance().GetAPIService();
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
            GetActivity().GetInstance().LoadAlbum(album);
        }

        public void LoadArtist(Artist artist)
        {
            GetActivity().GetInstance().LoadArtist(artist);
        }

        public override void OnResume()
        {
            ForceUpdate();
            GetActivity()?.FragmentLoaded();
            GetActivity()?.SetNavigationBarColor(NavigationBtnColorState);
            base.OnResume();
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
        }

        public override void OnStop()
        {
            base.OnStop();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public override void OnDetach()
        {
            ReleaseData();
            base.OnDetach();
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
            GetActivity()?.OnBackPressed();
        }

        public void SendData(string data)
        {
            this.JsonModel = data;
        }

        public T GetModel<T>()
        {
            if (!string.IsNullOrEmpty(JsonModel))
            {
                return JsonConvert.DeserializeObject<T>(JsonModel);
            }
            return default(T);
        }

        public SpotyPieFragmetManager GetFManager()
        {
            if (FManager == null)
                FManager = new SpotyPieFragmetManager(this);

            return FManager;
        }

        public void LoadFragmentInner(FragmentEnum switcher, FragmentScope scope = FragmentScope.Activity, string jsonModel = null, bool AddToBackButtonStack = true, LayoutScreenState screen = LayoutScreenState.Holder)
        {
            if (scope == FragmentScope.Activity)
            {
                GetActivity()?.LoadFragmentInner(switcher, jsonModel, AddToBackButtonStack, screen);
            }
            else
            {
                GetFManager().LoadFragmentInner(switcher, jsonModel, AddToBackButtonStack, screen);
            }
        }

        public abstract FragmentBase LoadFragment(FragmentEnum switcher);

        public int GetFragmentViewId()
        {
            if (PlayerFrame == null)
            {
                PlayerFrame = new FrameLayout(this.Context);

                PlayerFrame.LayoutParameters = new ConstraintLayout.LayoutParams(
                    ConstraintLayout.LayoutParams.MatchParent,
                    ConstraintLayout.LayoutParams.MatchParent);

                PlayerFrame.Id = int.MaxValue - 2;

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

        public virtual void SetScreen(LayoutScreenState screen)
        {
            GetActivity()?.SetScreen(screen);
        }

        public void RemoveCurrentFragment(SupportFragmentManager supportFragmentManager, FragmentBase fragmentBase)
        {
            if (ParentView != null && FragmentFrame != null)
            {
                if (fragmentBase != null)
                {
                    ParentView.RemoveView(FragmentFrame);
                    FragmentFrame = null;
                }
            }
            GetActivity().RemoveCurrentFragment(supportFragmentManager, fragmentBase);
        }
    }
}