using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using System;
using System.Threading.Tasks;

namespace SpotyPie.SongBinder.Fragments
{
    public class BindIndividualSongFragment : FragmentBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.bind_one_song_layout;

        private SeekBar SongCof;
        private SeekBar AlbumCof;
        private SeekBar ArtistCof;

        private TextView SongText;
        private TextView AlbumText;
        private TextView ArtistText;

        private TextView SongValue;
        private TextView AlbumValue;
        private TextView ArtistValue;

        private Switch SongSwitch;
        private Switch AlbumSwitch;
        private Switch ArtistSwitch;

        private ProgressBar Loading;

        protected override void InitView()
        {
            Loading = RootView.FindViewById<ProgressBar>(Resource.Id.loading);

            SongCof = RootView.FindViewById<SeekBar>(Resource.Id.song_cof_seeker);
            SongCof.ProgressChanged += SongCof_ProgressChanged;
            SongCof.StopTrackingTouch += StopTrackingTouch;

            AlbumCof = RootView.FindViewById<SeekBar>(Resource.Id.album_cof_seeker);
            AlbumCof.ProgressChanged += AlbumCof_ProgressChanged;
            AlbumCof.StopTrackingTouch += StopTrackingTouch;

            ArtistCof = RootView.FindViewById<SeekBar>(Resource.Id.artist_cof_seeker);
            ArtistCof.ProgressChanged += ArtistCof_ProgressChanged;
            ArtistCof.StopTrackingTouch += StopTrackingTouch;

            SongText = RootView.FindViewById<TextView>(Resource.Id.song_text);
            AlbumText = RootView.FindViewById<TextView>(Resource.Id.album_text);
            ArtistText = RootView.FindViewById<TextView>(Resource.Id.artist_text);

            SongValue = RootView.FindViewById<TextView>(Resource.Id.song_value);
            AlbumValue = RootView.FindViewById<TextView>(Resource.Id.album_value);
            ArtistValue = RootView.FindViewById<TextView>(Resource.Id.artist_value);

            SongSwitch = RootView.FindViewById<Switch>(Resource.Id.song_switch);
            SongSwitch.Checked = true;

            AlbumSwitch = RootView.FindViewById<Switch>(Resource.Id.album_switch);
            AlbumSwitch.Checked = true;

            ArtistSwitch = RootView.FindViewById<Switch>(Resource.Id.artist_switch);
            ArtistSwitch.Checked = true;

            SongSwitch.CheckedChange += SwitchCheckedChange;
            AlbumSwitch.CheckedChange += SwitchCheckedChange;
            ArtistSwitch.CheckedChange += SwitchCheckedChange;
        }

        private void SwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SongCof.Enabled = SongSwitch.Checked;
            AlbumCof.Enabled = AlbumSwitch.Checked;
            ArtistCof.Enabled = ArtistSwitch.Checked;
        }

        private void StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            SongCof.Enabled = false;
            AlbumCof.Enabled = false;
            ArtistCof.Enabled = false;
            SongSwitch.Enabled = false;
            AlbumSwitch.Enabled = false;
            ArtistSwitch.Enabled = false;

            Loading.Visibility = Android.Views.ViewStates.Visible;

            Task.Run(() => LoadSongsAsync());
            //throw new NotImplementedException();
        }

        private async Task LoadSongsAsync()
        {
            await Task.Delay(2000);
            RunOnUiThread(() =>
            {
                try
                {
                }
                catch (Exception)
                {

                }
                finally
                {
                    Loading.Visibility = Android.Views.ViewStates.Gone;
                    SongCof.Enabled = true;
                    AlbumCof.Enabled = true;
                    ArtistCof.Enabled = true;
                    SongSwitch.Enabled = true;
                    AlbumSwitch.Enabled = true;
                    ArtistSwitch.Enabled = true;
                }
            });
        }

        private void SongCof_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            SongValue.Text = SongCof.Progress.ToString();
        }

        private void AlbumCof_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            AlbumValue.Text = AlbumCof.Progress.ToString();
        }

        private void ArtistCof_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            ArtistValue.Text = ArtistCof.Progress.ToString();
        }

        public override void ForceUpdate()
        {
            SongTag songDetails = GetModel<SongTag>();
            if (songDetails != null)
            {
                SongText.Text = songDetails.Title;
                AlbumText.Text = songDetails.Album;
                ArtistText.Text = songDetails.Artist;
            }
        }

        public override void ReleaseData()
        {

        }
    }
}