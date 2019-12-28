using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Enums;
using System;
using System.Threading.Tasks;

namespace SpotyPie.SongBinder.Fragments
{
    public class SongDetailsFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_details;

        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;

        private ConstraintLayout InfoHolder;
        private ProgressBar Loading;

        private TextView SongTitle;
        private TextView Album;
        private TextView Artist;
        private TextView Gendres;
        private TextView Year;
        private TextView DiscNumber;
        private TextView Bitrate;

        private Button Delete;
        private Button AddImage;
        private Button SetQuality;
        private Button Connect;


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

            Delete = RootView.FindViewById<Button>(Resource.Id.delete);
            Delete.Click += Delete_Song_Click;

            AddImage = RootView.FindViewById<Button>(Resource.Id.add_image);
            AddImage.Enabled = false;

            SetQuality = RootView.FindViewById<Button>(Resource.Id.quality);
            SetQuality.Enabled = false;

            Connect = RootView.FindViewById<Button>(Resource.Id.connect);
            Connect.Click += Connect_Song_Click;

            Task.Run(() => LoadInfo());
        }

        private void Delete_Song_Click(object sender, EventArgs e)
        {
            Delete.Visibility = ViewStates.Gone;
            Delete.Enabled = false;
            Task.Run(async () =>
            {
                string status = await GetAPIService().DeleteSongAsync(GetModel<SongTag>().FilePath);

                RunOnUiThread(() =>
                {
                    Snackbar snackBar;
                    snackBar = Snackbar.Make(RootView, "File Deleted", Snackbar.LengthIndefinite);
                    if (status == "Success")
                    {
                        snackBar.SetAction("Ok", (view) =>
                        {
                            RemoveMe();
                            snackBar.Dismiss();
                            snackBar.Dispose();
                            snackBar = null;
                        });
                    }
                    else
                    {
                        snackBar.SetText("Failed To delete file");
                        snackBar.SetAction("Retry", (view) =>
                        {
                            Delete_Song_Click(null, null);
                            snackBar.Dismiss();
                            snackBar.Dispose();
                            snackBar = null;
                        });
                    }
                    snackBar.Show();

                    Delete.Visibility = ViewStates.Visible;
                    Delete.Enabled = true;
                });
            });
        }

        private void Connect_Song_Click(object sender, EventArgs e)
        {
            ParentActivity?.LoadFragmentInner(FragmentEnum.BindIndividualSongFragment, JsonModel);
        }

        private void LoadInfo()
        {
            RunOnUiThread(() =>
            {
                Loading.Visibility = ViewStates.Gone;
                InfoHolder.Visibility = ViewStates.Visible;
            });
        }

        public override void ForceUpdate()
        {
            SongTag songDetails = GetModel<SongTag>();
            if (songDetails != null)
            {
                SongTitle.Text = songDetails.Title;
                Album.Text = songDetails.Album;
                Artist.Text = songDetails.Artist;
                Gendres.Text = songDetails.Gendre;
                Year.Text = songDetails.Year.ToString();
                DiscNumber.Text = songDetails.TrackNumber.ToString();
                Bitrate.Text = songDetails.Bitrate.ToString();
            }
        }

        public override void ReleaseData()
        {
        }

        public override int GetParentView()
        {
            throw new NotImplementedException();
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}