using Android.App;
using Android.OS;
using Android.Views;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public abstract class FragmentBase : SupportFragment
    {
        public abstract int LayoutId { get; set; }

        protected View RootView;

        private MainActivity ParentActivity;

        public View GetView()
        {
            return RootView;
        }

        private int GetLayout()
        {
            return LayoutId;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(GetLayout(), container, false);

            ParentActivity = (MainActivity)this.Activity;

            InitView();
            return RootView;
        }

        public void Hide()
        {
            RootView.Alpha = 0.0f;
            RootView.TranslationX = 10000;
        }

        public void Show()
        {
            RootView.Alpha = 1f;
            RootView.TranslationX = 0;
        }

        public Current_state GetState()
        {
            return ParentActivity.GetState();
        }

        public dynamic GetService(ApiServices service = ApiServices.Shared)
        {
            return ParentActivity.GetService(service);
        }

        public void InvokeOnMainThread(Action action)
        {
            Application.SynchronizationContext.Post(_ =>
            {
                action.Invoke();
            }, null);
        }

        protected abstract void InitView();

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }
    }
}