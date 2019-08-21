using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Mobile_Api.Models.Realm;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie.MainFragments
{
    public class NowPlayingFragment : FragmentBase
    {
        private TextView ArtistName;
        private TextView SongTitle;
        private ImageButton PlayToggle;
        private ImageButton ShowPlayler;

        public override int LayoutId { get; set; } = Resource.Layout.now_playing_layout;
        protected override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Holder;

        protected override void InitView()
        {
            RootView.Visibility = ViewStates.Gone;

            PlayToggle = RootView.FindViewById<ImageButton>(Resource.Id.play_stop);
            ShowPlayler = RootView.FindViewById<ImageButton>(Resource.Id.show_player);

            SongTitle = RootView.FindViewById<TextView>(Resource.Id.song_title);
            SongTitle.Selected = true;
            ArtistName = RootView.FindViewById<TextView>(Resource.Id.artist_name);
            ArtistName.Selected = true;

            if (GetState().IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            PlayToggle.Click += PlayToggle_Click;
            ShowPlayler.Click += ShowMusicPlayer;
            RootView.Click += ShowMusicPlayer;
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            //if (GetState().IsPlaying)
            //    GetState().GetPlayer().Music_pause();
            //else
            //{
            //    GetState().GetPlayer().Music_play();
            //}
        }

        private void ShowMusicPlayer(object sender, EventArgs e)
        {
            GetState().SetSong(GetAPIService().GetCurrentListLive(), 0);
        }

        public override void ForceUpdate()
        {
            LoadCurrentState();
        }

        public override void ReleaseData()
        {

        }

        public override int GetParentView()
        {
            return Resource.Id.PlayerContainer;
        }

        public override void LoadFragment(dynamic switcher)
        {

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
                        Test(song);
                        if (song != null)
                        {
                            SongTitle.Text = song.Name;
                            ArtistName.Text = song.ArtistName;
                            RootView.Visibility = ViewStates.Visible;
                        }
                    });
                }
                catch (Exception e)
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this.Context, "Failed load current state", ToastLength.Long).Show();
                    });
                }
            });
        }

        private void Test(Songs song)
        {
            //using (var realm = Realm.GetInstance())
            //{
            //    var data = realm.All<ApplicationSongList>().FirstOrDefault(x => x.Id == 1);

            //    data.Add(realm, new Realm_Songs(song));

            //    var xdata = realm.All<ApplicationSongList>().FirstOrDefault(x => x.Id == 1);
            //}

            //using (var realm = Realm.GetInstance())
            //{
            //    var data = realm.All<ApplicationSongList>().FirstOrDefault(x => x.Id == 1);
            //    var songList = realm.All<Mobile_Api.Models.Realm.Music>().ToList();
            //}
        }
    }
}