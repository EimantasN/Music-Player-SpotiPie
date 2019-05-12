using Android.App;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Enums.Activitys;
using System;
using System.Threading.Tasks;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : ActivityBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.activity_main;

        private int LastViewLayer = 0;

        private int CurrentViewLayer = 1;

        private Current_state APPSTATE;

        private Main LastMainFragment;

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

        protected override void InitView()
        {
            base.InitView();
            APPSTATE = new Current_state(this);

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
            MiniPlayer.Visibility = ViewStates.Gone;

            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);
            ShowPlayler = FindViewById<ImageButton>(Resource.Id.show_player);

            SongTitle = FindViewById<TextView>(Resource.Id.song_name);
            SongTitle.Selected = true;
            ArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            ArtistName.Selected = true;

            LoadCurrentState();

            if (GetState().IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            PlayToggle.Click += PlayToggle_Click;
            ShowPlayler.Click += MiniPlayer_Click;
            MiniPlayer.Click += MiniPlayer_Click;

            #endregion

            BackHeaderButton = FindViewById<ImageButton>(Resource.Id.back);
            OptionsHeaderButton = FindViewById<ImageButton>(Resource.Id.options);

            BackHeaderButton.Click += BackHeaderButton_Click;

            bottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            LoadFragmentInner(Main.Home, AddToBackButtonStack: false);
        }

        private void LoadCurrentState()
        {
            //TODO make more maintanable
            Task.Run(async () =>
            {
                try
                {
                    var song = await GetAPIService().GetCurrentSong();
                    RunOnUiThread(() =>
                    {
                        if (song != null)
                        {
                            SongTitle.Text = song.Name;
                            ArtistName.Text = song.ArtistName;
                            MiniPlayer.Visibility = ViewStates.Visible;
                        }
                    });
                }
                catch (Exception e)
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this.ApplicationContext, "Failed load current state", ToastLength.Long).Show();
                    });
                }
            });
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            //switch (CurrentViewLayer)
            //{
            //    case 1:
            //        base.OnBackPressed();
            //        break;
            //    case 2:
            //        {
            //            ToogleSecondLayer(false);
            //            break;
            //        }
            //    case 3:
            //        {
            //            ToogleThirdLayer(false);
            //            if (LastViewLayer != 1)
            //            {
            //                ToogleSecondLayer(true);
            //            }
            //            break;
            //        }
            //    case 4:
            //        {
            //            TogglePlayer(false);
            //            switch (LastViewLayer)
            //            {
            //                case 1:
            //                    break;
            //                case 2:
            //                    {
            //                        ToogleSecondLayer(true);
            //                        break;
            //                    }
            //                case 3:
            //                    {
            //                        ToogleThirdLayer(true);
            //                        break;
            //                    }
            //            }
            //            break;
            //        }
            //}
        }

        private void BackHeaderButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnBackPressed();
            }
            catch (Exception)
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
            GetState().SetSong(GetAPIService().GetCurrentListLive(), 0);
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.Item.ItemId)
            {
                case Resource.Id.home:
                    LoadFragmentInner(Main.Home);
                    break;
                case Resource.Id.browse:
                    LoadFragmentInner(Main.Browse);
                    break;
                case Resource.Id.search:
                    LoadFragmentInner(Main.Search);
                    break;
                case Resource.Id.library:
                    LoadFragmentInner(Main.Library);
                    break;
            }
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

        public override void LoadBaseFragment()
        {
            base.LoadBaseFragment();
            //LoadFragmentInner(LastMainFragment);
        }

        public override void LoadFragment(dynamic switcher, string jsonModel = null)
        {
            switch (switcher)
            {
                case Main main:
                    switch (main)
                    {
                        case Main.Home:

                            if (MainFragment == null)
                                MainFragment = new MainFragment();
                            GetFManager().SetCurrentFragment(MainFragment);
                            LastMainFragment = main;
                            ActionName.Text = "Home";

                            return;
                        case Main.Browse:

                            if (Browse == null)
                                Browse = new MainArtist();
                            GetFManager().SetCurrentFragment(Browse);
                            LastMainFragment = main;
                            ActionName.Text = "Muse";

                            return;
                        case Main.Search:

                            if (Search == null)
                                Search = new Search();
                            GetFManager().SetCurrentFragment(Search);
                            LastMainFragment = main;
                            HeaderContainer.Visibility = ViewStates.Gone;

                            return;
                        case Main.Library:

                            if (Library == null) Library = new LibraryFragment();
                            GetFManager().SetCurrentFragment(Library);
                            LastMainFragment = main;
                            ActionName.Text = "Library";
                            return;
                    }
                    throw new Exception("Fragment not found in Main enum");
                case HomePage home:
                    switch (home)
                    {
                        case HomePage.Album:
                            if (AlbumFragment == null) AlbumFragment = new AlbumFragment();
                            GetFManager().SetCurrentFragment(AlbumFragment);
                            GetFManager().GetCurrentFragment().SendData(jsonModel);
                            return;
                        case HomePage.Artist:
                            if (ArtistFragment == null) ArtistFragment = new ArtistFragment();
                            GetFManager().SetCurrentFragment(ArtistFragment);
                            return;
                        case HomePage.Player:
                            GetFManager().SetCurrentFragment(new Player.Player());
                            return;
                    }
                    throw new Exception("Fragment not found in HomePage enum");
                default:
                    throw new Exception("Fragment not found");
            }
        }

        public void RemoveSplash(FragmentBase fr)
        {
            SupportFragmentManager.BeginTransaction().Remove(fr).CommitAllowingStateLoss();
        }

        public void LoadAlbum(Album album)
        {
            LoadFragmentInner(HomePage.Album, JsonConvert.SerializeObject(album));
            //GetState().SetAlbum(album);

            //if (!AlbumFragment.IsAdded)
            //{
            //    LoadFragmentInner(SupportFragmentManager, AlbumFragment);
            //    SupportFragmentManager.BeginTransaction().Add(Resource.Id.first_layer, AlbumFragment).Commit();
            //}
            //else
            //{
            //    AlbumFragment.Show();
            //}
            //if (ArtistFragment != null)
            //    ArtistFragment.Hide();

            //AlbumFragment.SetAlbum(album);
            //ToogleSecondLayer(true);
        }

        public Current_state GetState()
        {
            if (APPSTATE == null)
                APPSTATE = new Current_state(this);
            return APPSTATE;
        }

        public void LoadArtist(Artist artist)
        {
            if (ArtistFragment == null) ArtistFragment = new ArtistFragment();
            GetState().SetArtist(artist);

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
                FirstLayer.Visibility = ViewStates.Visible;
                FirstLayer.BringToFront();
            }
            else
            {
                GetFManager().GetCurrentFragment()?.ForceUpdate();
                FirstLayer.Visibility = ViewStates.Gone;
            }
        }

        public void TogglePlayer(bool show)
        {
            LoadFragmentInner(HomePage.Player);
            ////SHOW
            //if (show)
            //{
            //    LastViewLayer = CurrentViewLayer;
            //    CurrentViewLayer = 4;
            //    HideOthers();
            //    PlayerContainer.Visibility = ViewStates.Visible;
            //    PlayerContainer.BringToFront();
            //}
            //else
            //{
            //    bottomNavigation.Visibility = ViewStates.Visible;
            //    PlayerContainer.Visibility = ViewStates.Gone;
            //}
        }

        public void ToogleThirdLayer(bool show)
        {
            //SHOW
            if (show)
            {
                LastViewLayer = CurrentViewLayer;
                CurrentViewLayer = 3;
                HideOthers();
                SecondLayer.Visibility = ViewStates.Visible;
                SecondLayer.BringToFront();
            }
            else
            {
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

        public override dynamic GetInstance()
        {
            return this;
        }

        public override int GetParentView()
        {
            return Resource.Id.MainContainer;
        }
    }
}

