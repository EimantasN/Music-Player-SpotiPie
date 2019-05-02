using Android.App;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : AppCompatActivity
    {
        private int LastViewLayer = 0;
        private int CurrentViewLayer = 1;
        //private BluetoothHelper _bluetoothHelper;

        private Current_state APPSTATE;

        private FragmentBase CurrentFragment;

        private MainFragment MainFragment;
        private Search Search;
        private MainArtist Browse;
        private LibraryFragment Library;
        private AlbumFragment AlbumFragment;
        private ArtistFragment ArtistFragment;


        public BottomNavigationView bottomNavigation;
        public ImageButton PlayToggle;

        public TextView ArtistName;
        public TextView SongTitle;
        public ImageButton BackHeaderButton;
        public ImageButton OptionsHeaderButton;

        public int widthInDp = 0;
        public int HeightInDp = 0;
        public bool PlayerVisible = false;
        public ImageButton ShowPlayler;

        public TextView ActionName;
        public ConstraintLayout MiniPlayer;

        public FrameLayout Content;
        public FrameLayout FirstLayer;
        public FrameLayout SecondLayer;
        public FrameLayout PlayerContainer;

        public FragmentBase FirstLayerFragment;
        public FragmentBase SecondLayerFragment;

        public int Add_to_playlist_id = 0;

        ConstraintLayout HeaderContainer;

        public SupportFragmentManager mSupportFragmentManager;

        public ICommand MyCommand { get; }

        private void MyCommandExecute()
        {
            //var deviceName = "MDR";
            //if (!_bluetoothHelper.IsConnected)
            //    _bluetoothHelper.Connect(deviceName);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            mSupportFragmentManager = SupportFragmentManager;

            APPSTATE = new Current_state(this);

            var x = GetAPIService().GetCurrentList();

            HeaderContainer = FindViewById<ConstraintLayout>(Resource.Id.HeaderContainer);

            PlayerContainer = FindViewById<FrameLayout>(Resource.Id.player_frame);
            FirstLayer = FindViewById<FrameLayout>(Resource.Id.first_layer);
            SecondLayer = FindViewById<FrameLayout>(Resource.Id.second_layer);
            Content = FindViewById<FrameLayout>(Resource.Id.content_frame);

            widthInDp = Resources.DisplayMetrics.WidthPixels;
            HeightInDp = Resources.DisplayMetrics.HeightPixels;
            PlayerContainer.Visibility = ViewStates.Gone;
            SecondLayer.Visibility = ViewStates.Gone;
            FirstLayer.Visibility = ViewStates.Gone;

            bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.NavBot);
            ActionName = FindViewById<TextView>(Resource.Id.textView);

            #region MINI PLAYER

            MiniPlayer = FindViewById<ConstraintLayout>(Resource.Id.PlayerContainer);

            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);
            ShowPlayler = FindViewById<ImageButton>(Resource.Id.show_player);

            SongTitle = FindViewById<TextView>(Resource.Id.song_name);
            SongTitle.Selected = true;
            ArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            ArtistName.Selected = true;

            //TODO make more maintanable
            Task.Run(async () =>
            {
                var song = await GetAPIService().GetCurrentSong();
                SongTitle.Text = song.Name;
                ArtistName.Text = song.ArtistName;
            });

            if (GetState().IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            //MiniPlayer.Visibility = ViewStates.Gone;

            PlayToggle.Click += PlayToggle_Click;
            ShowPlayler.Click += MiniPlayer_Click;
            MiniPlayer.Click += MiniPlayer_Click;

            #endregion

            BackHeaderButton = FindViewById<ImageButton>(Resource.Id.back);
            OptionsHeaderButton = FindViewById<ImageButton>(Resource.Id.options);

            BackHeaderButton.Click += BackHeaderButton_Click;

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            LoadFragment(Resource.Id.home);
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
            if (APPSTATE.GetPlayer().CheckChildFragments())
            {

                switch (CurrentViewLayer)
                {
                    case 1:
                        base.OnBackPressed();
                        break;
                    case 2:
                        {
                            ToogleSecondLayer(false);
                            break;
                        }
                    case 3:
                        {
                            ToogleThirdLayer(false);
                            if (LastViewLayer != 1)
                            {
                                ToogleSecondLayer(true);
                            }
                            break;
                        }
                    case 4:
                        {
                            TogglePlayer(false);
                            switch (LastViewLayer)
                            {
                                case 1:
                                    break;
                                case 2:
                                    {
                                        ToogleSecondLayer(true);
                                        break;
                                    }
                                case 3:
                                    {
                                        ToogleThirdLayer(true);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }

        public static void LoadOptionsMeniu()
        {
        }

        private void BackHeaderButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnBackPressed();
            }
            catch (System.Exception)
            {
                if (MainFragment == null)
                    MainFragment = new MainFragment();

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, MainFragment)
                    .Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            if (GetState().IsPlaying)
                GetState().GetPlayer().Music_pause();
            else
            {
                GetState().GetPlayer().Music_play();
            }
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

        private API Api_service { get; set; }

        private Object ServiceLock { get; set; } = new Object();

        public API GetAPIService()
        {
            if (Api_service == null)
                return Api_service = new API(new Mobile_Api.Service(), this);
            return Api_service;
        }

        void LoadFragment(int id)
        {
            //if (HeaderContainer.Visibility == ViewStates.Gone)
            //    HeaderContainer.Visibility = ViewStates.Visible;

            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;
            switch (id)
            {
                case Resource.Id.home:
                    {
                        if (MainFragment == null)
                            MainFragment = new MainFragment();
                        CurrentFragment = MainFragment;
                        ActionName.Text = "Home";
                        break;
                    }
                case Resource.Id.browse:
                    {
                        if (Browse == null)
                            Browse = new MainArtist();
                        CurrentFragment = Browse;
                        ActionName.Text = "Muse";
                        break;
                    }
                case Resource.Id.search:
                    {
                        if (Search == null)
                            Search = new Search();
                        CurrentFragment = Search;
                        HeaderContainer.Visibility = ViewStates.Gone;
                        break;
                    }
                case Resource.Id.library:
                    {
                        if (Library == null) Library = new LibraryFragment();
                        CurrentFragment = Library;
                        ActionName.Text = "Library";
                        break;
                    }
            }

            if (CurrentFragment == null)
                return;

            if (!CurrentFragment.IsAdded)
            {
                SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, CurrentFragment)
                .Commit();
            }
            else
            {
                CurrentFragment.Show();
            }
        }

        public void RemoveCurrentFragment()
        {
            if (FirstLayerFragment != null)
            {
                FirstLayerFragment.ReleaseData();
                var transaction = mSupportFragmentManager.BeginTransaction();
                transaction.Remove(FirstLayerFragment);
                transaction.Commit();
                transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentClose);
                transaction = null;
                FirstLayerFragment = null;
                FirstLayer.TranslationX = widthInDp;
            }
        }

        public void RemoveSplash(FragmentBase fr)
        {
            SupportFragmentManager.BeginTransaction().Remove(fr).CommitAllowingStateLoss();
        }

        public void LoadAlbum(Album album)
        {
            if (AlbumFragment == null) AlbumFragment = new AlbumFragment();
            APPSTATE.SetAlbum(album);

            if (!AlbumFragment.IsAdded)
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.first_layer, AlbumFragment).Commit();
            else
            {
                AlbumFragment.Show();
            }
            if (ArtistFragment != null)
                ArtistFragment.Hide();

            AlbumFragment.SetAlbum(album);
            ToogleSecondLayer(true);
        }

        public void LoadArtist(Artist artist)
        {
            if (ArtistFragment == null) ArtistFragment = new ArtistFragment();
            APPSTATE.SetArtist(artist);

            if (!ArtistFragment.IsAdded)
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.first_layer, ArtistFragment).Commit();
            else
            {
                ArtistFragment.Show();
            }
            if (AlbumFragment != null)
                AlbumFragment.Hide();

            ArtistFragment.LoadArtist(artist);
            ToogleSecondLayer(true);
        }

        public void ToogleSecondLayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 2;
                HideOthers();
                //AlbumFragment?.ForceUpdate();
                //ArtistFragment?.ForceUpdate();
                FirstLayer.Visibility = ViewStates.Visible;
                FirstLayer.BringToFront();
            }
            else
            {
                CurrentFragment?.ForceUpdate();
                FirstLayer.Visibility = ViewStates.Gone;
            }
        }

        public void TogglePlayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 4;
                HideOthers();
                PlayerContainer.Visibility = ViewStates.Visible;
                PlayerContainer.BringToFront();
            }
            else
            {
                bottomNavigation.Visibility = ViewStates.Visible;
                PlayerContainer.Visibility = ViewStates.Gone;
            }
        }

        public void ToogleThirdLayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 3;
                HideOthers();
                //AlbumFragment.ForceUpdate();
                SecondLayer.Visibility = ViewStates.Visible;
                SecondLayer.BringToFront();
            }
            else
            {
                //if (AlbumFragment != null && AlbumFragment.IsVisible)
                //    AlbumFragment.ReleaseData();

                //if (ArtistFragment != null && ArtistFragment.IsVisible)
                //    ArtistFragment.ReleaseData();

                SecondLayer.Visibility = ViewStates.Gone;
            }
        }

        public void HideOthers()
        {
            if (CurrentViewLayer != 2)
                ToogleSecondLayer(false);

            if (CurrentViewLayer != 3)
                ToogleThirdLayer(false);

            if (CurrentViewLayer != 4)
                TogglePlayer(false);

            if (CurrentViewLayer == 2)
            {
                bottomNavigation.Visibility = ViewStates.Visible;
            }
            else
            {
                bottomNavigation.Visibility = ViewStates.Gone;
            }
        }
    }
}

