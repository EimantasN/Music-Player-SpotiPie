using Android.App;
using Android.Widget;
using SpotyPie.Base;
using SpotyPie.Enums;

namespace SpotyPie.SongBinder
{
    [Activity(Label = "SongBinderActivity", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.SpotyPie")]
    public class SongBinderActivity : ActivityBase
    {
        public override int LayoutId { get; set; } = Resource.Layout.song_binder_activity;
        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Main;
        public override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.FullScreen;

        //Action buttons
        private Button BindSongs;
        private Button Sync;
        private Button AddArtist;
        private Button DeleteSong;
        private Button LoadTorrent;
        private Button SetQuality;

        protected override void InitView()
        {
            IsFragmentLoadedAdded = true;
            base.InitView();
            BindSongs = FindViewById<Button>(Resource.Id.bind_song_btn);
            BindSongs.Click += BindSongs_Click;

            Sync = FindViewById<Button>(Resource.Id.sync_btn);
            Sync.Enabled = false;

            AddArtist = FindViewById<Button>(Resource.Id.add_artist_btn);
            AddArtist.Enabled = false;

            DeleteSong = FindViewById<Button>(Resource.Id.delete_song_btn);
            DeleteSong.Enabled = false;

            LoadTorrent = FindViewById<Button>(Resource.Id.load_torrent_btn);
            LoadTorrent.Enabled = false;

            SetQuality = FindViewById<Button>(Resource.Id.quality_btn);
            SetQuality.Enabled = false;
        }

        private void BindSongs_Click(object sender, System.EventArgs e)
        {
            LoadFragmentInner(FragmentEnum.UnBindedSongList);
        }

        public override dynamic GetInstance()
        {
            return this;
        }

        public override int GetParentView(bool Player = false)
        {
            return Resource.Id.parent_view;
        }

        public override void SetScreen(LayoutScreenState screen)
        {

        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            switch (switcher)
            {
                case FragmentEnum.UnBindedSongList:
                    return new Fragments.SongBindList();
                case FragmentEnum.SongDetailsFragment:
                    return new Fragments.SongDetailsFragment();
                case FragmentEnum.BindIndividualSongFragment:
                    return new Fragments.BindIndividualSongFragment();
                default:
                    return null;
            }
        }
    }
}