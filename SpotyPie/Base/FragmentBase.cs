using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Mobile_Api.Models;
using Newtonsoft.Json;
using System;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Base
{
    public abstract class FragmentBase : SupportFragment
    {
        public abstract int LayoutId { get; set; }

        protected string JsonModel { get; set; }

        protected View RootView;

        protected ActivityBase ParentActivity;

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
            base.OnResume();
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
    }
}