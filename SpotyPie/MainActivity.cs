using Android.App;
using Android.Content;
using Android.Support.Constraints;
using Android.Support.Design.Widget;
using Android.Views;
using Mobile_Api.Models;
using Newtonsoft.Json;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Helpers;
using SpotyPie.MainFragments;
using SpotyPie.Music;

namespace SpotyPie
{
    [Activity(Label = "SpotyPie", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class MainActivity : ActivityBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.activity_main;

        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Main;
        public override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Holder;

        private MainFragment MainFragment;
        private Search Search;
        //private HostStats Performance;
        private LibraryFragment Library;
        private AlbumFragment AlbumFragment;
        private ArtistFragment ArtistFragment;


        public BottomNavigationView BottomNavigation;

        public int WidthInDp = 0;
        public int HeightInDp = 0;
        public bool PlayerVisible = false;

        public FragmentBase FirstLayerFragment;
        public FragmentBase SecondLayerFragment;

        public int Add_to_playlist_id = 0;

        protected override void InitView()
        {
            base.InitView();

            ConstraintLayout layout = FindViewById<ConstraintLayout>(Resource.Id.MainContainer);
            GradientBG.SetBacground(layout);

            WidthInDp = Resources.DisplayMetrics.WidthPixels;
            HeightInDp = Resources.DisplayMetrics.HeightPixels;

            BottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.NavBot);

            BottomNavigation.NavigationItemSelected += BottomNavigation_NavigationItemSelected;
            LoadFragmentInner(FragmentEnum.Home, AddToBackButtonStack: false);
        }

        protected override void OnResume()
        {
            LoadMiniPlayer();
            StartMusicService();
            base.OnResume();
        }

        private void LoadMiniPlayer()
        {
            GetFManager().InsertFragment(Resource.Id.mini_player_holder, new NowPlayingFragment());
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void BottomNavigation_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            GetFManager().Reset();
            FManager = null;

            switch (e.Item.ItemId)
            {
                case Resource.Id.home:
                    LoadFragmentInner(FragmentEnum.Home);
                    break;
                case Resource.Id.search:
                    LoadFragmentInner(FragmentEnum.Search);
                    break;
                case Resource.Id.library:
                    LoadFragmentInner(FragmentEnum.Library);
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.StopService(new Intent(this, typeof(MusicService)));
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
        }

        public void LoadAlbum(Album album)
        {
            LoadFragmentInner(FragmentEnum.Album, JsonConvert.SerializeObject(album));
        }

        public void LoadArtist(Artist artist)
        {
            LoadFragmentInner(FragmentEnum.Artist, JsonConvert.SerializeObject(artist));
        }

        public override dynamic GetInstance()
        {
            return this;
        }

        public override int GetParentView(bool Player = false)
        {
            if (Player)
                return Resource.Id.MainContainer;
            else
                return Resource.Id.content_holder;
        }

        public override void SetScreen(LayoutScreenState screen)
        {
            switch (screen)
            {
                case LayoutScreenState.FullScreen:
                    if (BottomNavigation.Visibility == ViewStates.Visible)
                    {
                        BottomNavigation.Visibility = ViewStates.Gone;
                    }
                    break;
                case LayoutScreenState.Holder:
                    if (BottomNavigation.Visibility == ViewStates.Gone)
                    {
                        BottomNavigation.Visibility = ViewStates.Visible;
                    }
                    break;
            }
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            switch (switcher)
            {
                case FragmentEnum.Home:
                    if (MainFragment == null)
                        MainFragment = new MainFragment();
                    return MainFragment;
                case FragmentEnum.Search:
                    if (Search == null)
                        Search = new Search();
                    return Search;
                case FragmentEnum.Library:
                    if (Library == null) Library = new LibraryFragment();
                    return Library;
                case FragmentEnum.Album:
                    if (AlbumFragment == null) AlbumFragment = new AlbumFragment();
                    return AlbumFragment;
                case FragmentEnum.Artist:
                    if (ArtistFragment == null) ArtistFragment = new ArtistFragment();
                    return ArtistFragment;
                case FragmentEnum.Player:
                    StartPlayer();
                    return null;
                default:
                    return null;
            }
        }
    }
}

