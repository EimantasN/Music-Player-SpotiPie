
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Realms;
using System.Collections.Generic;
using System.Linq;
using SpotyPie.Database.ViewModels;
using System.Threading;

namespace SpotyPie
{
    [Activity(Label = "SettingsActivity", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class SettingsActivity : AppCompatActivity
    {
        private Spinner MusicQualitySpinner;
        private Switch DataSaverSwitch;
        private Switch ExplicitContentSwitch;
        private Switch UnplayableSongsSwitch;
        private Switch AutoplaySwitch;
        private Switch CustomImagesSwitch;
        private Switch AutoHeadsetSwitch;
        private Switch CellularSwitch;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.settings);

            Settings settings = InitRealSettings();

            DataSaverSwitch = FindViewById<Switch>(Resource.Id.data_saver_switch);
            DataSaverSwitch.CheckedChange += DataSaverSwitch_CheckedChange;
            DataSaverSwitch.Checked = settings.DataSaver;

            ExplicitContentSwitch = FindViewById<Switch>(Resource.Id.explicit_content_switch);
            ExplicitContentSwitch.CheckedChange += ExplicitContentSwitch_CheckedChange;
            ExplicitContentSwitch.Checked = settings.ExplicitContentSwitch;

            UnplayableSongsSwitch = FindViewById<Switch>(Resource.Id.unplayable_songs_switch);
            UnplayableSongsSwitch.CheckedChange += UnplayableSongsSwitch_CheckedChange;
            UnplayableSongsSwitch.Checked = settings.UnplayableSongsSwitch;

            AutoplaySwitch = FindViewById<Switch>(Resource.Id.autoplay_switch);
            AutoplaySwitch.CheckedChange += AutoplaySwitch_CheckedChange;
            AutoplaySwitch.Checked = settings.AutoplaySwitch;

            CustomImagesSwitch = FindViewById<Switch>(Resource.Id.custom_images_switch);
            CustomImagesSwitch.CheckedChange += CustomImagesSwitch_CheckedChange;
            CustomImagesSwitch.Checked = settings.CustomImagesSwitch;

            AutoHeadsetSwitch = FindViewById<Switch>(Resource.Id.auto_headset_switch);
            AutoHeadsetSwitch.CheckedChange += AutoHeadsetSwitch_CheckedChange;
            AutoHeadsetSwitch.Checked = settings.AutoHeadsetSwitch;

            CellularSwitch = FindViewById<Switch>(Resource.Id.cellular_switch);
            CellularSwitch.CheckedChange += CellularSwitch_CheckedChange;
            CellularSwitch.Checked = settings.CellularSwitch;
        }

        private Settings InitRealSettings()
        {
            var realm = Realm.GetInstance();

            if (realm.All<Settings>().FirstOrDefault() == null)
                realm.Write(() =>
                {
                    realm.Add(new Settings());
                });
            return realm.All<Settings>().First();
        }

        protected override void OnDestroy()
        {
            DataSaverSwitch.CheckedChange -= DataSaverSwitch_CheckedChange;
            ExplicitContentSwitch.CheckedChange -= ExplicitContentSwitch_CheckedChange;
            UnplayableSongsSwitch.CheckedChange -= UnplayableSongsSwitch_CheckedChange;
            AutoplaySwitch.CheckedChange -= AutoplaySwitch_CheckedChange;
            CustomImagesSwitch.CheckedChange -= CustomImagesSwitch_CheckedChange;
            AutoHeadsetSwitch.CheckedChange -= AutoHeadsetSwitch_CheckedChange;
            CellularSwitch.CheckedChange -= CellularSwitch_CheckedChange;
            MusicQualitySpinner.ItemSelected -= Spinner_ItemSelected;
            base.OnDestroy();
        }

        private void CellularSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.CellularSwitch = CellularSwitch.Checked);
            }).Start();
        }

        private void AutoHeadsetSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"AutoHeadsetSwitch {AutoHeadsetSwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.AutoHeadsetSwitch = AutoHeadsetSwitch.Checked);
            }).Start();
        }

        private void CustomImagesSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"CustomImagesSwitch {CustomImagesSwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.CustomImagesSwitch = CustomImagesSwitch.Checked);
            }).Start();
        }

        private void AutoplaySwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"AutoplaySwitch {AutoplaySwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.AutoplaySwitch = AutoplaySwitch.Checked);
            }).Start();
        }

        private void UnplayableSongsSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"UnplayableSongsSwitch {UnplayableSongsSwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.UnplayableSongsSwitch = UnplayableSongsSwitch.Checked);
            }).Start();
        }

        private void ExplicitContentSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"ExplicitContentSwitch {ExplicitContentSwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.ExplicitContentSwitch = ExplicitContentSwitch.Checked);
            }).Start();
        }

        private void DataSaverSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"DataSaverSwitch {DataSaverSwitch.Checked}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.DataSaver = DataSaverSwitch.Checked);
            }).Start();
        }

        private void InitSpinner()
        {
            MusicQualitySpinner = FindViewById<Spinner>(Resource.Id.music_quality_spinner);
            var items = new List<string>() { "Flac 24-bit 192-khz", "Flac 16-bit 44.1 khz", "Mp3 360-bit" };
            var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSelectableListItem, items);
            MusicQualitySpinner.Adapter = adapter;
            MusicQualitySpinner.ItemSelected += Spinner_ItemSelected;
        }

        private void Spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Toast.MakeText(this.ApplicationContext, $"Selected {MusicQualitySpinner.SelectedItemId}", ToastLength.Short).Show();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.MusicQuality = (int)MusicQualitySpinner.SelectedItemId);
            }).Start();
        }
    }
}