using Android.App;
using Android.OS;
using Android.Widget;
using Realms;
using System.Collections.Generic;
using System.Linq;
using SpotyPie.Database.ViewModels;
using System.Threading;
using Android.Bluetooth;
using System;
using System.Threading.Tasks;
using Android.Views;
using SpotyPie.Base;

namespace SpotyPie
{
    [Activity(Label = "SettingsActivity", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.SpotyPie")]
    public class SettingsActivity : ActivityBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.settings;

        private API Api_service { get; set; }

        private Spinner MusicQualitySpinner;
        private Spinner BluetoothDeviceSpinner;
        private Switch DataSaverSwitch;
        private Switch ExplicitContentSwitch;
        private Switch UnplayableSongsSwitch;
        private Switch AutoplaySwitch;
        private Switch CustomImagesSwitch;
        private Switch AutoHeadsetSwitch;
        private Switch CellularSwitch;

        private TextView DeviceName;
        private TextView DeviceStatus;
        private TextView CurrentText;

        private TextView SongBindedText;
        private TextView SongBindedCount;
        private TextView SongUnbindedText;
        private TextView SongUnbindedCount;
        private Button LaunchSongBinder;

        protected override void InitView()
        {
            base.InitView();
            Settings settings = InitRealSettings();

            InitSpinner();

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

            //Song binder
            SongBindedText = FindViewById<TextView>(Resource.Id.song_binded_text);
            SongBindedCount = FindViewById<TextView>(Resource.Id.song_binded_count);

            SongUnbindedText = FindViewById<TextView>(Resource.Id.song_unbinded_text);
            SongUnbindedCount = FindViewById<TextView>(Resource.Id.song_unbinded_count);

            LaunchSongBinder = FindViewById<Button>(Resource.Id.song_binder_btn);
            LaunchSongBinder.Click += LaunchSongBinder_Click;
        }

        protected override void OnResume()
        {
            base.OnResume();
            GetBindedStatistics();
            InitBluetoothSpinner();
        }

        private void GetBindedStatistics()
        {
            Task.Run(async () =>
            {
                var bindStatistics = await GetAPIService().GetBindedStatisticsAsync();
                RunOnUiThread(() =>
                {
                    SongUnbindedCount.Text = $"{bindStatistics.unbindedCount}";
                    SongBindedCount.Text = $"{bindStatistics.bindedCount}";
                });
            });
        }

        private void LaunchSongBinder_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SongBinder.SongBinderActivity));
        }

        private void InitBluetoothSpinner()
        {
            if (AutoHeadsetSwitch.Checked)
            {
                Task.Run(() =>
                {
                    Settings settings = InitRealSettings();
                    List<String> Devices = new List<String>();
                    if (string.IsNullOrEmpty(settings.CurrentBthDeviceName))
                        Devices.Add("Not selected");
                    else
                        Devices.Add(settings.CurrentBthDeviceName);

                    Devices.AddRange(PairedDevices(Devices[0]));
                    RunOnUiThread(() =>
                    {
                        var adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSelectableListItem, Devices);

                        DeviceName = FindViewById<TextView>(Resource.Id.textView33);
                        DeviceName.Text = Devices[0];
                        DeviceName.Visibility = ViewStates.Visible;

                        //TODO Add connectivity
                        DeviceStatus = FindViewById<TextView>(Resource.Id.device_connected_status);
                        DeviceStatus.Visibility = ViewStates.Gone;

                        CurrentText = FindViewById<TextView>(Resource.Id.textView34);
                        CurrentText.Visibility = ViewStates.Visible;

                        BluetoothDeviceSpinner = FindViewById<Spinner>(Resource.Id.bluetooth_device_spinner);
                        BluetoothDeviceSpinner.Visibility = ViewStates.Visible;

                        BluetoothDeviceSpinner.Adapter = adapter;
                        BluetoothDeviceSpinner.ItemSelected += BluetoothDeviceSpinner_ItemSelected;
                    });
                });
            }
            else
            {
                if (DeviceName != null)
                {
                    RunOnUiThread(() =>
                    {
                        DeviceName.Visibility = ViewStates.Gone;
                        DeviceStatus.Visibility = ViewStates.Gone;
                        CurrentText.Visibility = ViewStates.Gone;
                        BluetoothDeviceSpinner.Visibility = ViewStates.Gone;
                    });
                }
            }
        }

        private void BluetoothDeviceSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            TextView a = (TextView)e.View;
            DeviceName.Text = a.Text;

            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.CurrentBthDeviceName = a.Text);
            }).Start();
        }

        private Settings InitRealSettings()
        {
            var realm = Realm.GetInstance();

            if (realm.All<Settings>().FirstOrDefault() == null)
                realm.Write(() =>
                {
                    realm.Add(new Settings());
                });
            var settings = realm.All<Settings>().First();
            return settings;
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
            InitBluetoothSpinner();
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.AutoHeadsetSwitch = AutoHeadsetSwitch.Checked);
            }).Start();
        }

        private void CustomImagesSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.CustomImagesSwitch = CustomImagesSwitch.Checked);
            }).Start();
        }

        private void AutoplaySwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.AutoplaySwitch = AutoplaySwitch.Checked);
            }).Start();
        }

        private void UnplayableSongsSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.UnplayableSongsSwitch = UnplayableSongsSwitch.Checked);
            }).Start();
        }

        private void ExplicitContentSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.ExplicitContentSwitch = ExplicitContentSwitch.Checked);
            }).Start();
        }

        private void DataSaverSwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
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
            new Thread(() =>
            {
                var realm = Realm.GetInstance();
                var settings = realm.All<Settings>().First();
                realm.Write(() => settings.MusicQuality = (int)MusicQualitySpinner.SelectedItemId);
            }).Start();
        }

        public List<string> PairedDevices(string current)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            List<string> devices = new List<string>();
            foreach (var bd in adapter.BondedDevices)
            {
                if (bd.Name != current)
                    devices.Add(bd.Name);
            }
            return devices;
        }

        public override void LoadFragment(dynamic switcher, string jsonModel = null)
        {

        }

        public override dynamic GetInstance()
        {
            return this;
        }

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }
    }
}