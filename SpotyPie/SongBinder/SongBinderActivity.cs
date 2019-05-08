using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using SpotyPie.Base;
using SpotyPie.SongBinder.Enumerators;

namespace SpotyPie.SongBinder
{
    [Activity(Label = "SongBinderActivity", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Theme = "@style/Theme.SpotyPie")]
    public class SongBinderActivity : AppCompatActivity
    {
        private FragmentBase CurrentFragment;

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
            LoadFragment(BinderFragments.UnBindedSongList);
        }

        void LoadFragment(BinderFragments fragment)
        {
            //if (HeaderContainer.Visibility == ViewStates.Gone)
            //    HeaderContainer.Visibility = ViewStates.Visible;

            if (CurrentFragment != null)
            {
                CurrentFragment.Hide();
            }

            CurrentFragment = null;
            switch (fragment)
            {
                case BinderFragments.UnBindedSongList:
                    CurrentFragment = new SongBinder.Fragments.SongBindList();
                    break;
                default:
                    break;
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
    }
}