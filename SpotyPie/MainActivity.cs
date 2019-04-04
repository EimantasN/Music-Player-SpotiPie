using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Mobile_Api;
using Mobile_Api.Models;
using Mobile_Api.Models.Enums;
using SpotyPie.Base;
using SpotyPie.Helpers;
using SpotyPie.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : AppCompatActivity, IBluetoothProfileServiceListener
    {
        private BluetoothHelper _bluetoothHelper;

        FragmentBase Home;
        FragmentBase Browse;
        FragmentBase Search;
        FragmentBase Library;
        SupportFragment Player;
        public static SupportFragment Album;
        public static SupportFragment Artist;

        BottomNavigationView bottomNavigation;
        public static ImageButton PlayToggle;

        public static TextView ArtistName;
        public static TextView SongTitle;
        public static ImageButton BackHeaderButton;
        public static ImageButton OptionsHeaderButton;

        public static int widthInDp = 0;
        public static int HeightInDp = 0;
        public static bool PlayerVisible = false;
        public ImageButton ShowPlayler;

        public static TextView ActionName;
        public static ConstraintLayout MiniPlayer;
        public static FrameLayout PlayerContainer;

        public static FrameLayout Fragment;

        public static int Add_to_playlist_id = 0;

        ConstraintLayout HeaderContainer;

        public static SupportFragmentManager mSupportFragmentManager;

        public static Android.Support.V4.App.Fragment CurrentFragment;

        public ICommand MyCommand { get; }

        private void MyCommandExecute()
        {
            var deviceName = "MDR";
            if (!_bluetoothHelper.IsConnected)
                _bluetoothHelper.Connect(deviceName);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _bluetoothHelper = new BluetoothHelper(Application.ApplicationContext);
            MyCommandExecute();

            HeaderContainer = FindViewById<ConstraintLayout>(Resource.Id.HeaderContainer);

            mSupportFragmentManager = SupportFragmentManager;

            PlayerContainer = FindViewById<FrameLayout>(Resource.Id.player_frame);
            Fragment = FindViewById<FrameLayout>(Resource.Id.song_options);

            widthInDp = Resources.DisplayMetrics.WidthPixels;
            HeightInDp = Resources.DisplayMetrics.HeightPixels;
            PlayerContainer.TranslationX = 0;
            Fragment.TranslationX = widthInDp;

            Home = new Home();
            Browse = new Browse();
            //Search = new Search();
            //Library = new LibraryFragment();
            Player = new Player.Player();
            Album = new AlbumFragment();
            Artist = new ArtistFragment();

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.player_frame, Player)
                .Commit();

            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);
            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.NavBot);
            ActionName = FindViewById<TextView>(Resource.Id.textView);
            MiniPlayer = FindViewById<ConstraintLayout>(Resource.Id.PlayerContainer);
            ArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            ShowPlayler = FindViewById<ImageButton>(Resource.Id.show_player);
            //Animation marquee = AnimationUtils.LoadAnimation(this, Resource.Drawable.marquee);
            //ArtistName.StartAnimation(marquee);

            SongTitle = FindViewById<TextView>(Resource.Id.song_name);
            BackHeaderButton = FindViewById<ImageButton>(Resource.Id.back);
            OptionsHeaderButton = FindViewById<ImageButton>(Resource.Id.options);

            if (Current_state.IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            BackHeaderButton.Click += BackHeaderButton_Click;

            MiniPlayer.Click += MiniPlayer_Click;
            ShowPlayler.Click += MiniPlayer_Click;
            PlayToggle.Click += PlayToggle_Click;
            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            LoadFragment(Resource.Id.home);
            MiniPlayer.Visibility = ViewStates.Gone;
            BlueTooh();
        }

        public override void OnBackPressed()
        {
            if (Current_state.PlayerIsVisible)
            {
                PlayerContainer.TranslationX = widthInDp;
                return;
            }
            if (Fragment.TranslationX == 0)
            {
                Fragment.TranslationX = widthInDp;
                return;
            }

            if (CurrentFragment != null)
            {
                RemoveCurrentFragment();
                SupportFragmentManager.BeginTransaction()
                   .Replace(Resource.Id.content_frame, Home)
                   .Commit();
                return;
            }
            base.OnBackPressed();
        }

        public static void LoadOptionsMeniu()
        {
            mSupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.song_options, new PlaylistFragment())
                .Commit();
            MainActivity.Fragment.TranslationX = 0;
        }

        private void BackHeaderButton_Click(object sender, EventArgs e)
        {
            try
            {
                Current_state.HideHeaderNavigationButtons();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, Current_state.BackFragment)
                    .Commit();
            }
            catch (System.Exception)
            {
                Home = new Home();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, Home)
                    .Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!Current_state.IsPlayerLoaded)
            {
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.player_frame, Player)
                    .Commit();
                PlayerContainer.TranslationX = widthInDp;
            }
            Task.Run(() => API_data.GetSong());
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            Current_state.Music_play_toggle();
        }

        private void MiniPlayer_Click(object sender, EventArgs e)
        {
            Current_state.Player_visiblibity_toggle();
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            TabAdapter adapter = new TabAdapter(SupportFragmentManager);
            //adapter.AddFragment(new Home(), "Home");
            //adapter.AddFragment(new Browse(), "Browse");
            //adapter.AddFragment(new Search(), "Search");
            //adapter.AddFragment(new LibraryFragment(), "Library");

            viewPager.Adapter = adapter;
        }

        public SongService SongAPI { get; set; }
        public AlbumService AlbumAPI { get; set; }

        private Object ServiceLock { get; set; } = new Object();

        public dynamic GetService(ApiServices service)
        {
            lock (ServiceLock)
            {
                switch (service)
                {
                    case ApiServices.Songs:
                        {
                            if (SongAPI == null)
                                return SongAPI = new SongService();
                            return SongAPI;
                        }
                    case ApiServices.Albums:
                        {
                            if (AlbumAPI == null)
                                return AlbumAPI = new AlbumService();
                            return AlbumAPI;
                        }
                    default:
                        return null;
                }
            }
        }

        void LoadFragment(int id)
        {
            if (HeaderContainer.Visibility == ViewStates.Gone)
                HeaderContainer.Visibility = ViewStates.Visible;

            if (Current_state.BackFragment != null)
            {
                Current_state.BackFragment.Hide();
            }

            FragmentBase fragment = null;
            switch (id)
            {
                case Resource.Id.home:
                    fragment = Browse;
                    ActionName.Text = "Home";
                    break;
                case Resource.Id.browse:
                    fragment = Home;
                    ActionName.Text = "Browse";
                    break;
                case Resource.Id.search:
                    fragment = Search;
                    HeaderContainer.Visibility = ViewStates.Gone;
                    break;
                case Resource.Id.library:
                    fragment = Library;
                    ActionName.Text = "Library";
                    break;
            }

            if (fragment == null)
                return;

            Current_state.BackFragment = fragment;
            if (!fragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.content_frame, fragment)
                    .Commit();
            }
            else
            {
                Current_state.BackFragment.Show();
            }
        }

        public void RemoveCurrentFragment()
        {
            if (CurrentFragment != null)
            {
                var transaction = mSupportFragmentManager.BeginTransaction();
                transaction.Remove(CurrentFragment);
                transaction.Commit();
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
                CurrentFragment = null;
                MainActivity.Fragment.TranslationX = widthInDp;
            }
        }

        BluetoothHeadset Headset;
        BluetoothDevice Device;

        public void BlueTooh()
        {
            //try
            //{
            //    BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            //    var pairedDevices = mBluetoothAdapter.BondedDevices;
            //    List<String> s = new List<string>();
            //    foreach (BluetoothDevice bt in pairedDevices)
            //    {
            //        if (bt.Name.Contains("MDR"))
            //        {
            //            var a = Java.Util.UUID.FromString("00001108-0000-1000-8000-00805F9B34FB");
            //            bt.CreateRfcommSocketToServiceRecord(a);
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //}
        }

        public void OnServiceConnected(ProfileType profile, IBluetoothProfile proxy)
        {
            if (profile == ProfileType.Headset)
            {
                Headset = (BluetoothHeadset)proxy;
                if (Headset == null)
                    return;

                var method = Headset.Class.GetDeclaredMethod("connect", Java.Lang.Class.FromType(typeof(BluetoothDevice))); // here BluetoothDevice is the object of the device which you want to connect to...

                if (method != null && Device != null)
                {
                    method.Invoke(Headset, Device);
                }
            }
        }

        public void OnServiceDisconnected([GeneratedEnum] ProfileType profile)
        {
            //throw new NotImplementedException();
        }
    }
}

