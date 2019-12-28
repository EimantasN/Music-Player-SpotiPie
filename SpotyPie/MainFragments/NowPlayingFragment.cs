using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Music;
using SpotyPie.Music.Enums;
using SpotyPie.Music.Manager;
using System;

namespace SpotyPie.MainFragments
{
    public class NowPlayingFragment : FragmentBase
    {
        private TextView ArtistName;
        private TextView SongTitle;
        private ImageButton PlayToggle;
        private ImageButton ShowPlayler;
        private ProgressBar SongProgress;

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

            SongProgress = RootView.FindViewById<ProgressBar>(Resource.Id.progressBar);
            SongProgress.Enabled = false;

            PlayToggle.Click += PlayToggle_Click;
            ShowPlayler.Click += ShowMusicPlayer;
            RootView.Click += ShowMusicPlayer;

            OnPlayingStateChange(SongManager._playState);
            OnSongChange(SongManager.Song);
            if (Playback.CurrentDuration != 0)
            {
                OnDurationChange(Playback.CurrentDuration);
                OnPositionChange(Playback.CurrentPosition);
            }
            else
            {
                SongProgress.Visibility = ViewStates.Gone;
            }
        }

        private void OnDurationChange(int duration)
        {
            if (duration > 0)
            {
                SongProgress.Visibility = ViewStates.Visible;
                SongProgress.Max = duration;
            }
        }

        private void OnPositionChange(int position)
        {
            SongProgress.Progress = position;
        }

        public void OnPlayingStateChange(PlayState state)
        {
            switch (state)
            {
                case PlayState.Loading:
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    break;
                case PlayState.Playing:
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    break;
                case PlayState.Stopeed:
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    break;
            }
        }

        public void OnSongChange(Songs song)
        {
            if (song != null)
            {
                SongTitle.Text = song.Name;
                ArtistName.Text = song.ArtistName;
                RootView.Visibility = ViewStates.Visible;
            }
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            SongManager.ToggleState();
        }

        private void ShowMusicPlayer(object sender, EventArgs e)
        {
            GetActivity().StartPlayer();
        }

        public override void ForceUpdate()
        {
            SongManager.SongHandler += OnSongChange;
            SongManager.PlayingHandler += OnPlayingStateChange;
            Playback.DurationHandler += OnDurationChange;
            Playback.PositionHandler += OnPositionChange;
        }

        public override void ReleaseData()
        {
            SongManager.SongHandler -= OnSongChange;
            SongManager.PlayingHandler -= OnPlayingStateChange;
            Playback.DurationHandler += OnDurationChange;
            Playback.PositionHandler += OnPositionChange;
        }

        public override int GetParentView()
        {
            return Resource.Id.PlayerContainer;
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            return null;
        }
    }
}