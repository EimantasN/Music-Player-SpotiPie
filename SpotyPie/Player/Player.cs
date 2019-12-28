using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Enums;
using SpotyPie.Helpers;
using SpotyPie.Music;
using SpotyPie.Music.Enums;
using SpotyPie.Music.Manager;
using System;

namespace SpotyPie.Player
{
    [Activity(Label = "SpotyPie", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class Player : ActivityBase, View.IOnTouchListener, IServiceConnection
    {
        private bool IsBinded = false;
        private bool DisableSmoothScrool = true;
        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Player;

        private IServiceConnection ServiceConnection;

        private string CurrentPlayerImage { get; set; }

        protected float mLastPosY;

        protected static int newsId = 0;

        protected const int OffsetContainer = 250;

        protected int FragmentWidth = 0;

        public int CurrentState { get; set; } = 1;

        public override int LayoutId { get; set; } = Resource.Layout.player;

        public override LayoutScreenState ScreenState { get; set; } = LayoutScreenState.FullScreen;

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);

        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);

        bool saved_to_songs = false;

        public ImageButton HidePlayerButton;
        public ImageButton PlayToggle;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        private TextView PlayerSongName;
        private TextView PlayerArtistName;
        private TextView CurrentSongListType;
        private TextView CurrentSongListValue;


        ImageButton SongListButton;

        SeekBar SongTimeSeekBar;

        ImageButton Repeat;

        ImageButton Shuffle;

        bool Shuffle_state = false;

        ImageView Save_to_songs;

        private bool SeekActive = false;

        SpotyPieViewPager Pager;
        ImageAdapter Adapter;

        protected override void InitView()
        {
            ServiceConnection = this;
            DisableSmoothScrool = true;

            Pager = FindViewById<SpotyPieViewPager>(Resource.Id.img_holder);
            Adapter = new ImageAdapter(this.ApplicationContext, this);
            Pager.Adapter = Adapter;
            Pager.PageSelected += OnPagerSelected;
            OnPagerSelected(Pager, new ViewPager.PageSelectedEventArgs(-1));

            SongListButton = FindViewById<ImageButton>(Resource.Id.song_list);
            SongListButton.Click += OnSongListButtonClick;

            NextSong = FindViewById<ImageButton>(Resource.Id.next_song);
            NextSong.Click += OnNextSongClick;
            PreviewSong = FindViewById<ImageButton>(Resource.Id.preview_song);
            PreviewSong.Click += OnPrevSongClick;

            Repeat = FindViewById<ImageButton>(Resource.Id.repeat);
            Repeat.Click += OnRepeatClick;
            Shuffle = FindViewById<ImageButton>(Resource.Id.shuffle);
            Shuffle.Click += OnShuffleClick;
            Save_to_songs = FindViewById<ImageView>(Resource.Id.save_to_songs);
            Save_to_songs.Click += OnSongLikedClick;

            PlayerSongName = FindViewById<TextView>(Resource.Id.song_name);
            PlayerSongName.Selected = true;
            PlayerArtistName = FindViewById<TextView>(Resource.Id.artist_name);
            PlayerArtistName.Selected = true;
            CurrentSongListType = FindViewById<TextView>(Resource.Id.playing_from_title);
            CurrentSongListValue = FindViewById<TextView>(Resource.Id.playing_from_value);

            CurretSongTimeText = FindViewById<TextView>(Resource.Id.current_song_time);
            CurretSongTimeText.Text = "00:00";
            TotalSongTimeText = FindViewById<TextView>(Resource.Id.total_song_time);
            TotalSongTimeText.Visibility = ViewStates.Invisible;

            HidePlayerButton = FindViewById<ImageButton>(Resource.Id.back_button);
            HidePlayerButton.Click += OnPlayerHide;
            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);

            PlayToggle.Click += OnPlayToggleClick;

            OnRepeatClick(null, null);
            OnShuffleClick(null, null);

            FragmentWidth = Resources.DisplayMetrics.WidthPixels;

            SongTimeSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar);

            SongTimeSeekBar.StartTrackingTouch += OnStartTrackingTouch;
            SongTimeSeekBar.StopTrackingTouch += OnStopTrackingTouch;

            OnSonChange(SongManager.Song);
            OnPlayStateChange(SongManager._playState);
            OnDurationChange(Playback.CurrentDuration);
            if (Playback.CurrentPosition != 0)
            {
                SongTimeSeekBar.Enabled = true;
            }
            OnPositionChange(Playback.CurrentPosition);
        }

        private void OnPlayerHide(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        protected override void OnStart()
        {
            base.OnStart();
            SongManager.SongHandler += OnSonChange;
            SongManager.PlayingHandler += OnPlayStateChange;
            Playback.DurationHandler += OnDurationChange;
            Playback.PositionHandler += OnPositionChange;
            BindSong(SongManager.Song);
        }

        protected override void OnStop()
        {
            base.OnStop();
            SongManager.SongHandler -= OnSonChange;
            SongManager.PlayingHandler -= OnPlayStateChange;
            Playback.DurationHandler -= OnDurationChange;
            Playback.PositionHandler -= OnPositionChange;
        }

        private void OnPositionChange(int position)
        {
            if (!SeekActive)
            {
                SongTimeSeekBar.Enabled = true;
                SongTimeSeekBar.Progress = position;
                CurretSongTimeText.Text = new TimeSpan(0, 0, 0, 0, position).ToString(@"mm\:ss");
            }
            if (position == 0)
            {
                CurretSongTimeText.Text = "00:00";
                SongTimeSeekBar.Enabled = false;
            }
        }

        private void OnDurationChange(int duration)
        {
            if (duration != 0)
            {
                SongTimeSeekBar.Max = duration;
                TotalSongTimeText.Text = new TimeSpan(0, 0, 0, 0, duration).ToString(@"mm\:ss");
                TotalSongTimeText.Visibility = ViewStates.Visible;
            }
            else
            {
                TotalSongTimeText.Visibility = ViewStates.Invisible;
            }
        }

        public void OnPlayStateChange(PlayState state)
        {
            switch (state)
            {
                case PlayState.Playing:
                    Pager.Enable(true);
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    break;
                case PlayState.Stopeed:
                    Pager.Enable(true);
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    break;
                case PlayState.Loading:
                    Pager.Enable(false);
                    SongLoadStarted();
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    break;
                default:
                    break;
            }
        }

        public void OnSonChange(Songs song) => BindSong(song);

        private void BindSong(Songs song)
        {
            if (song != null)
            {
                new Handler().PostDelayed(() =>
                {
                    TitleHelper.Format(PlayerSongName, song.Name, 14);
                    TitleHelper.Format(PlayerArtistName, song.ArtistName, 12);
                    TitleHelper.Format(CurrentSongListValue, song.AlbumName, 12);

                    if (DisableSmoothScrool)
                    {
                        DisableSmoothScrool = false;
                        Pager.SetCurrentItem(SongManager.Index, false);
                    }
                    else
                    {
                        Pager.SetCurrentItem(SongManager.Index, true);
                    }
                }, 25);
            }
        }

        private void OnPagerSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (e.Position == -1)
            {
                //Ignore
            }
            else if (SongManager.Index == 0 && e.Position == 0)
            {
                SongManager.Play();
            }
            else if (SongManager.Index < e.Position) //Moved foward
            {
                SongManager.Next();
            }
            else if (SongManager.Index > e.Position) //Moved backward
            {
                SongManager.Prev();
            }
        }

        public void SongLoadStarted()
        {
            SongTimeSeekBar.Enabled = false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!IsBinded)
            {
                BindService(new Intent(this, typeof(MusicService)), ServiceConnection, Bind.AboveClient);
            }
        }

        public override int GetParentView(bool Player = false)
        {
            return Resource.Id.parent_view;
        }

        private void OnSongListButtonClick(object sender, EventArgs ee)
        {
            LoadFragmentInner(FragmentEnum.CurrentSongList, screen: LayoutScreenState.FullScreen);
        }

        #region Player events

        private void OnPrevSongClick(object sender, EventArgs e)
        {
            if (SongManager.Prev())
            {
                OnDurationChange(0);
                OnPositionChange(0);
            }
        }

        private void OnNextSongClick(object sender, EventArgs e)
        {
            if (SongManager.Next())
            {
                OnDurationChange(0);
                OnPositionChange(0);
            }
        }

        private void OnPlayToggleClick(object sender, EventArgs e)
        {
            SongManager.ToggleState();
        }

        public void SetSeekBarProgress(int progress, string text)
        {
            RunOnUiThread(() =>
            {
                CurretSongTimeText.Text = text;
                if (!SeekActive && SongTimeSeekBar != null)
                    SongTimeSeekBar.Progress = progress;
            });
        }

        private void OnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            Intent intent = new Intent("com.spotypie.adnroid.musicservice.seek");
            intent.PutExtra("PLAYER_SEEK", e.SeekBar.Progress.ToString());
            SendBroadcast(intent);
            SeekActive = false;
        }

        private void OnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            SeekActive = true;
        }

        private void OnRepeatClick(object sender, EventArgs e)
        {
            //if (MusicService == null) return;
            //switch (MusicService.Repeat_state)
            //{
            //    case 0:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat);
            //            MusicService.Repeat_state = 1;
            //            if (MusicService != null) MusicService.MusicPlayer.Looping = true;
            //            break;
            //        }
            //    case 1:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat_once);
            //            MusicService.Repeat_state = 2;
            //            break;
            //        }
            //    case 2:
            //        {
            //            Repeat.SetImageResource(Resource.Drawable.repeat_off);
            //            MusicService.Repeat_state = 0;
            //            break;
            //        }
            //}
        }

        private void OnShuffleClick(object sender, EventArgs e)
        {
            if (Shuffle_state)
                Shuffle.SetImageResource(Resource.Drawable.shuffle_disabled);
            else
                Shuffle.SetImageResource(Resource.Drawable.shuffle_variant);


            Shuffle_state = !Shuffle_state;
        }

        private void OnSongLikedClick(object sender, EventArgs e)
        {
            if (saved_to_songs)
                Save_to_songs.SetImageResource(Resource.Drawable.check);
            else
                Save_to_songs.SetImageResource(Resource.Drawable.@checked);


            saved_to_songs = !saved_to_songs;
        }

        #endregion

        #region Lisiners

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mLastPosY = e.GetX();
                    return true;

                case MotionEventActions.Up:
                    var metrics = Resources.DisplayMetrics;
                    var widthInDp = metrics.HeightPixels;

                    var transXExit = v.TranslationX;
                    if (v.TranslationX > 0)
                    {
                        if (transXExit < (FragmentWidth / 3))
                        {
                            v.Animate().TranslationX(0);
                            return true;
                        }
                        else
                        {
                            v.Animate().TranslationX(FragmentWidth);
                            OnPrevSongClick(null, null);
                            return true;
                        }
                    }
                    else
                    {
                        if (Math.Abs(transXExit) < (FragmentWidth / 3))
                        {
                            v.Animate().TranslationX(0);
                            return true;
                        }
                        else
                        {
                            v.Animate().TranslationX(FragmentWidth);
                            OnNextSongClick(null, null);
                            return true;
                        }
                    }

                case MotionEventActions.Move:
                    var proc = 90 * v.TranslationX / (FragmentWidth - OffsetContainer);
                    //Debug.Print(proc.ToString());
                    if (proc > 90) proc = 90;
                    var currentPosition = e.GetX();
                    var deltX = mLastPosY - currentPosition;

                    var transX = v.TranslationX;
                    transX -= deltX;

                    v.TranslationX = transX;
                    //v.Animate().TranslationY(transY);
                    return true;

                default:
                    v.Animate().TranslationX(0);
                    return v.OnTouchEvent(e);
            }
        }

        #endregion

        #region Music service connection

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            if (!IsBinded)
            {
                var binder = service as MusicServiceBinder;
                binder.SetConnectionStatus(true);
                IsBinded = true;
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            ShowMessage("Service unbinded");
        }

        #endregion

        public override dynamic GetInstance()
        {
            return this;
        }

        public override void SetScreen(LayoutScreenState screen)
        {
            switch (screen)
            {
                case LayoutScreenState.FullScreen:
                    break;
                case LayoutScreenState.Holder:
                    break;
            }
        }

        public override FragmentBase LoadFragment(FragmentEnum switcher)
        {
            switch (switcher)
            {
                case FragmentEnum.CurrentSongList:
                    return new PlayerSongList();
                case FragmentEnum.SongOptionsFragment:
                    return new SongOptionsFragment();
                default:
                    return null;
            }
        }
    }
}