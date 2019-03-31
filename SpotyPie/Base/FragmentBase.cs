using Android.App;
using Android.OS;
using Android.Views;
using Mobile_Api;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public class FragmentBase : SupportFragment
    {
        protected View RootView;

        private MainActivity ParentActivity;

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

        public SharedService GetService()
        {
            return ParentActivity.Service;
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