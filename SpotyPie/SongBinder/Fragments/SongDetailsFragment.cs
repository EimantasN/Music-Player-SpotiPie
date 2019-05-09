using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
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

        private TextView SongTitle;
        private TextView Album;
        private TextView Artist;
        private TextView Gendres;
        private TextView Year;
        private TextView DiscNumber;
        private TextView Bitrate;

        protected override void InitView()
        {
            InfoHolder = RootView.FindViewById<ConstraintLayout>(Resource.Id.info_holder);
            Loading = RootView.FindViewById<ProgressBar>(Resource.Id.loading);

            SongTitle = RootView.FindViewById<TextView>(Resource.Id.song_title_value);
            Album = RootView.FindViewById<TextView>(Resource.Id.album_value);
            Artist = RootView.FindViewById<TextView>(Resource.Id.artist_value);
            Gendres = RootView.FindViewById<TextView>(Resource.Id.gendres_value);
            Year = RootView.FindViewById<TextView>(Resource.Id.year_value);
            DiscNumber = RootView.FindViewById<TextView>(Resource.Id.disc_number_value);
            Bitrate = RootView.FindViewById<TextView>(Resource.Id.bitrate_value);

            Task.Run(() => LoadInfoAsync());
        }

        private async Task LoadInfoAsync()
        {
            RunOnUiThread(() =>
            {
                Loading.Visibility = ViewStates.Gone;
                InfoHolder.Visibility = ViewStates.Visible;
            });
        }

        public override void ForceUpdate()
        {
            //if (!string.IsNullOrEmpty(JsonModel))
            //{
            //    try
            //    {
            //        SongTag songDetails = JsonConvert.DeserializeObject<SongTag>(JsonModel);
            //        SongTitle.Text = songDetails.Title;
            //        Album.Text = songDetails.Album;
            //        Artist.Text = songDetails.Artist;
            //        Gendres.Text = songDetails.Gendre;
            //        Year.Text = songDetails.Year.ToString();
            //        DiscNumber.Text = songDetails.TrackNumber.ToString();
            //        Bitrate.Text = songDetails.Bitrate.ToString();
            //    }
            //    catch (Exception e)
            //    {
            //        Snackbar snacbar = Snackbar.Make(RootView, "Failed to load song details", Snackbar.LengthLong);
            //        snacbar.SetAction("Ok", (view) =>
            //        {
            //            snacbar.Dismiss();
            //            snacbar.Dispose();
            //        });
            //        snacbar.Show();
            //    }
            //}
        }

        public override void ReleaseData()
        {
        }
    }
}