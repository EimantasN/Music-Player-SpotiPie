using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Enums;
using SpotyPie.Models;
using System;
using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public abstract class FragmentBase : SupportFragment
    {
        private static int FrameLayoutId { get; set; } = 1;

        protected abstract Enums.LayoutScreenState ScreenState { get; set; }

        public virtual NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Default;

        public abstract int LayoutId { get; set; }

        private ViewGroup ParentView { get; set; }

        private FrameLayout PlayerFrame;

        protected string JsonModel { get; set; }

        protected View RootView;

        protected ActivityBase ParentActivity;

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
            return GetActivity().GetInstance().FirstLayerFragment;
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

        public void LoadFragmentInner(dynamic switcher, string jsonModel = null, bool AddToBackButtonStack = true, LayoutScreenState screen = LayoutScreenState.Holder)
        {
            GetActivity().GetFManager().FragmentHistory.Push(new FragmentState());
            GetActivity().GetFManager().FragmentHistory.Peek().AddToBackStack = AddToBackButtonStack;
            GetActivity().GetFManager().FragmentHistory.Peek().FatherState = ParentActivity.FManager.CurrentFragmentState;
            GetActivity().GetFManager().FragmentHistory.Peek().ScreenState = screen;

            if (GetActivity().GetFManager().FragmentHistory.Peek()?.FatherState?.Fragment != null)
            {
                GetActivity().GetFManager().CurrentFragmentState?.FatherState?.Fragment.Hide();
            }

            GetActivity().GetFManager().GetFragmentStack().Push(switcher);

            LoadFragment(switcher);

            if (GetActivity().GetFManager().FragmentHistory?.Peek()?.Fragment == null)
            {
                throw new Exception("Fragment not founded");
            }

            //Can send data to fragment
            if (!string.IsNullOrEmpty(jsonModel))
                GetActivity().GetFManager().FragmentHistory?.Peek()?.SendData(jsonModel);

            if (GetActivity().GetFManager().FragmentHistory.Peek().Fragment is Player.Player)
                GetActivity().GetFManager().FragmentHistory.Peek().LayoutId = GetFragmentViewId(true);
            else
                GetActivity().GetFManager().FragmentHistory.Peek().LayoutId = GetFragmentViewId();

            InsertFragment(GetActivity().GetFManager().FragmentHistory.Peek().LayoutId, ParentActivity.GetFManager().FragmentHistory.Peek().Fragment);
            GetActivity().FManager.FragmentHistory.Peek().BackButton =
                () =>
                {
                    ParentView.RemoveView(FragmentFrame);
                    FragmentFrame = null;
                    GetActivity().RemoveCurrentFragment(ChildFragmentManager,
                        GetActivity().GetFManager().FragmentHistory.Peek().Fragment);
                };

            SetScreen(screen);
        }

        public void InsertFragment(int layoutId, FragmentBase fragment)
        {
            if (ChildFragmentManager != null)
            {
                var transaction = ChildFragmentManager.BeginTransaction();
                if (transaction != null)
                {
                    FragmentTransitions.SetCustomTransitions(ref transaction, fragment);
                    transaction.Replace(layoutId, fragment);
                    //transaction.AddToBackStack(null);
                    transaction.Commit();
                }
            }
        }

        public abstract void LoadFragment(dynamic switcher);

        public int GetFragmentViewId(bool isPlayer = false)
        {
            if (!isPlayer)
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
            else
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
        }

        public virtual void SetScreen(LayoutScreenState screen)
        {
            ParentActivity.SetScreen(screen);
        }
    }
}