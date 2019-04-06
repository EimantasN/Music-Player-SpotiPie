﻿using Android.App;
using Android.OS;
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
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : AppCompatActivity
    {
        private BluetoothHelper _bluetoothHelper;

        private Current_state APPSTATE;

        FragmentBase Home;
        FragmentBase Browse;
        FragmentBase Search;
        FragmentBase Library;
        public AlbumFragment AlbumFragment;
        public SupportFragment ArtistFragment;

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

        public FrameLayout Content;
        public FrameLayout FirstLayer;
        public FrameLayout SecondLayer;
        public FrameLayout PlayerContainer;

        public FragmentBase FirstLayerFragment;
        public FragmentBase SecondLayerFragment;
        public SupportFragment Player;

        public static int Add_to_playlist_id = 0;

        ConstraintLayout HeaderContainer;

        public static SupportFragmentManager mSupportFragmentManager;

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
            Player = new Player.Player();
            APPSTATE = new Current_state((Player.Player)Player);

            //_bluetoothHelper = new BluetoothHelper(Application.ApplicationContext);
            //MyCommandExecute();

            HeaderContainer = FindViewById<ConstraintLayout>(Resource.Id.HeaderContainer);

            mSupportFragmentManager = SupportFragmentManager;

            PlayerContainer = FindViewById<FrameLayout>(Resource.Id.player_frame);
            FirstLayer = FindViewById<FrameLayout>(Resource.Id.first_layer);
            SecondLayer = FindViewById<FrameLayout>(Resource.Id.second_layer);
            Content = FindViewById<FrameLayout>(Resource.Id.content_frame);

            widthInDp = Resources.DisplayMetrics.WidthPixels;
            HeightInDp = Resources.DisplayMetrics.HeightPixels;
            PlayerContainer.Visibility = ViewStates.Gone;

            ToogleSecondLayer(false);
            FirstLayer.Visibility = ViewStates.Gone;

            Home = new Browse();
            Browse = new MainFragment();
            AlbumFragment = new AlbumFragment();

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.player_frame, Player)
                .Commit();

            //Content.BringToFront();

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

            if (GetState().IsPlaying)
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
        }

        protected override void OnDestroy()
        {
            APPSTATE.Dispose();
            base.OnDestroy();
        }

        public Current_state GetState()
        {
            return APPSTATE;
        }

        public override void OnBackPressed()
        {
            if (GetState().PlayerIsVisible)
            {
                PlayerContainer.TranslationX = widthInDp;
                return;
            }
            if (FirstLayer.TranslationX == 0)
            {
                FirstLayer.TranslationX = widthInDp;
                return;
            }

            if (FirstLayerFragment != null)
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
            //mSupportFragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.song_options, new PlaylistFragment())
            //    .Commit();
            //MainActivity.firstLayer.TranslationX = 0;
        }

        private void BackHeaderButton_Click(object sender, EventArgs e)
        {
            try
            {
                GetState().HideHeaderNavigationButtons();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, GetState().BackFragment)
                    .Commit();
            }
            catch (System.Exception)
            {
                Home = new MainFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, Home)
                    .Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!GetState().IsPlayerLoaded)
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
            GetState().Music_play_toggle();
        }

        private void MiniPlayer_Click(object sender, EventArgs e)
        {
            GetState().Player_visiblibity_toggle();
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

            if (GetState().BackFragment != null)
            {
                GetState().BackFragment.Hide();
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
                    ActionName.Text = "MUSE";
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

            GetState().BackFragment = fragment;
            if (!fragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.content_frame, fragment)
                    .Commit();
            }
            else
            {
                GetState().BackFragment.Show();
            }
        }

        public void RemoveCurrentFragment()
        {
            if (FirstLayerFragment != null)
            {
                var transaction = mSupportFragmentManager.BeginTransaction();
                transaction.Remove(FirstLayerFragment);
                transaction.Commit();
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
                FirstLayerFragment = null;
                FirstLayer.TranslationX = widthInDp;
            }
        }

        public void LoadAlbum(Album album)
        {
            if (AlbumFragment == null)
            {
                AlbumFragment = new AlbumFragment();
                AlbumFragment.SetAlbum(album);
            }
            //Current_state.SetAlbum(Dataset[position]);
            MainActivity.mSupportFragmentManager.BeginTransaction().Replace(Resource.Id.second_layer, AlbumFragment).Commit();
            ToogleSecondLayer(true);
        }

        public void ToogleSecondLayer(bool show)
        {
            //SHOW
            if (show)
            {
                SecondLayer.Visibility = ViewStates.Visible;
                SecondLayer.BringToFront();
            }
            else
            {
                SecondLayer.Visibility = ViewStates.Gone;
                SecondLayer.TranslationX = 0;
            }
        }
    }
}

