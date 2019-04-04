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
    public class FragmentBase : SupportFragment
    {
        protected View RootView;

        private MainActivity ParentActivity;

        public View GetView()
        {
            return RootView;
        }

        public virtual int GetLayout()
        {
            return Resource.Layout.home_layout;
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

        protected virtual void InitView()
        {
        }

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