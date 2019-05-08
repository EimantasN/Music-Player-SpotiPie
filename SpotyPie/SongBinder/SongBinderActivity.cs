using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using SpotyPie.Base;
using SpotyPie.SongBinder.Enumerators;

namespace SpotyPie.SongBinder
{
    [Activity(Label = "SongBinderActivity", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.SpotyPie")]
    public class SongBinderActivity : ActivityBase
    {
        //Action buttons
        private Button BindSongs;
        private Button Sync;
        private Button AddArtist;
        private Button DeleteSong;
        private Button LoadTorrent;
        private Button SetQuality;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.song_binder_activity);

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
            LoadFragmentInner(BinderFragments.UnBindedSongList);
        }


        //DO not use this to load fragment
        protected override void LoadFragment(dynamic switcher)
        {
            switch (switcher)
            {
                case BinderFragments.UnBindedSongList:
                    CurrentFragment = new Fragments.SongBindList();
                    break;
                case BinderFragments.SongDetailsFragment:
                    CurrentFragment = new Fragments.SongDetailsFragment();
                    break;
                default:
                    break;
            }
        }

        public override dynamic GetInstance()
        {
            return this;
        }

        protected override void InitFather()
        {
            throw new System.NotImplementedException();
        }
    }
}