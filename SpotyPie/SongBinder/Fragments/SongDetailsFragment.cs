using Android.Support.Constraints;
using Android.Views;
using Android.Widget;
using SpotyPie.Base;
using System;
using System.Threading.Tasks;

namespace SpotyPie.SongBinder.Fragments
{
    public class SongDetailsFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_details;

        private ConstraintLayout InfoHolder;
        private ProgressBar Loading;

        protected override void InitView()
        {
            InfoHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.info_holder);
            Loading = RootView.FindViewById<ProgressBar>(Resource.Id.loading);


            Task.Run(() => LoadInfoAsync());
        }

        private async Task LoadInfoAsync()
        {
            await Task.Delay(2500);
            RunOnUiThread(() =>
            {
                Loading.Visibility = ViewStates.Gone;
                InfoHolder.Visibility = ViewStates.Visible;
            });
        }

        public override void ForceUpdate()
        {
        }

        public override void ReleaseData()
        {
        }
    }
}