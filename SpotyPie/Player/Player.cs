using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Media;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Database.Helpers;
using SpotyPie.Enums;
using SpotyPie.Helpers;
using SpotyPie.Music;
using SpotyPie.Music.Helpers;
using SpotyPie.Player.Interfaces;
using SpotyPie.Services;
using SpotyPie.Services.Binders;
using SpotyPie.Services.Interfaces;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie.Player
{
    [Activity(Label = "SpotyPie", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@drawable/logo_spotify", Theme = "@style/Theme.SpotyPie")]
    public class Player : ActivityBase, View.IOnTouchListener, Playback.ICallback, IServiceConnection
    {
        private bool IsBinded = false;
        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Player;

        private int ViewLoadState = 0;

        private bool Bound { get; set; } = false;

        //private MusicService MusicService;

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

        private int LastPosition = 0;

        protected override void InitView()
        {
            ServiceConnection = this;

            Pager = FindViewById<SpotyPieViewPager>(Resource.Id.img_holder);
            Adapter = new ImageAdapter(this.ApplicationContext, this);
            Pager.Adapter = Adapter;
            Pager.PageSelected += Pager_PageSelected;
            Pager_PageSelected(Pager, new ViewPager.PageSelectedEventArgs(-1));

            SongListButton = FindViewById<ImageButton>(Resource.Id.song_list);
            SongListButton.Click += SongListButton_Click;

            NextSong = FindViewById<ImageButton>(Resource.Id.next_song);
            NextSong.Click += NextSong_Click;
            PreviewSong = FindViewById<ImageButton>(Resource.Id.preview_song);
            PreviewSong.Click += PreviewSong_Click;

            Repeat = FindViewById<ImageButton>(Resource.Id.repeat);
            Repeat.Click += Repeat_Click;
            Shuffle = FindViewById<ImageButton>(Resource.Id.shuffle);
            Shuffle.Click += Shuffle_Click;
            Save_to_songs = FindViewById<ImageView>(Resource.Id.save_to_songs);
            Save_to_songs.Click += Save_to_songs_Click;

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
            PlayToggle = FindViewById<ImageButton>(Resource.Id.play_stop);

            HidePlayerButton.Click += HidePlayerButton_Click;
            PlayToggle.Click += PlayToggle_Click;
            Repeat_Click(null, null);
            Shuffle_Click(null, null);

            FragmentWidth = Resources.DisplayMetrics.WidthPixels;

            SongTimeSeekBar = FindViewById<SeekBar>(Resource.Id.seekBar);

            SongTimeSeekBar.StartTrackingTouch += SongTimeSeekBar_StartTrackingTouch;
            SongTimeSeekBar.StopTrackingTouch += SongTimeSeekBar_StopTrackingTouch;
            SongTimeSeekBar.ProgressChanged += SongTimeSeekBar_ProgressChanged;
        }

        private void Pager_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            Intent actionToService = null;
            if (e.Position == -1)
            {

            }
            if (LastPosition == 0 && e.Position == 0)
            {
                Pager.Enable(false);
                actionToService = new Intent("com.spotypie.adnroid.musicservice.play");

            }
            else if (LastPosition < e.Position) //Moved foward
            {
                Pager.Enable(false);
                LastPosition = e.Position;
                actionToService = new Intent("com.spotypie.adnroid.musicservice.next");
                Task.Run(() => RunOnUiThread(() => SongLoadStarted()));
            }
            else if (LastPosition > e.Position)
            {
                Pager.Enable(false);
                LastPosition = e.Position;
                actionToService = new Intent("com.spotypie.adnroid.musicservice.prev");
            }

            Task.Run(async () =>
            {
                Songs song = null;
                while (song == null)
                {
                    if (e.Position == -1)
                    {
                        RunOnUiThread(() => { song = Adapter.GetRecentItem(); });
                    }
                    else
                    {
                        RunOnUiThread(() => { song = Adapter.GetCurrentSong(e.Position); });
                    }
                    await Task.Delay(250);
                }

                RunOnUiThread(() =>
                {
                    if (LastPosition == -1)
                        LastPosition = Adapter.GetCurrentItem();

                    TitleHelper.Format(PlayerSongName, song.Name == null ? "Error" : song.Name, 14);
                    TitleHelper.Format(PlayerArtistName, song.ArtistName == null ? "Error" : song.ArtistName, 12);
                    TitleHelper.Format(CurrentSongListValue, song.AlbumName == null ? "Error" : song.AlbumName, 12);
                });
                RunOnUiThread(() => { if (actionToService != null) { SendBroadcast(actionToService); } });
            });
        }

        public void SongLoadStarted()
        {
            CurretSongTimeText.Text = "00:00";
            if (!SeekActive && SongTimeSeekBar != null)
                SongTimeSeekBar.Progress = 0;
            SongTimeSeekBar.Enabled = false;
            PlayToggle.SetImageResource(Resource.Drawable.play_loading);
            PlayToggle.Tag = Resource.Drawable.play_loading;
        }

        public void SkipToNext()
        {
            throw new NotImplementedException();
        }

        public void SkipToPrevious()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if(!IsBinded)
                BindService(new Intent(this, typeof(Music.MusicService)), ServiceConnection, Bind.AboveClient);

            //Intent intent = new Intent(this.Activity, typeof(MusicService));
            //Activity.BindService(intent, this.ServiceConnection, Bind.AutoCreate);

            //ImgHolder.SetOnTouchListener(this);
        }

        protected override void OnDestroy()
        {
            if (IsBinded)
            {
                //UnbindService(ServiceConnection);
            }
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            //Pager.SetCurrentItem(Adapter.GetCurrentItem(), false);
            base.OnBackPressed();
        }

        public override void LoadFragment(dynamic switcher, string jsonModel = null)
        {
            switch (switcher)
            {
                case Enums.Activitys.Player.CurrentSongList:
                    GetFManager().SetCurrentFragment(new PlayerSongList());
                    return;
            }
        }

        public override int GetParentView(bool Player = false)
        {
            return Resource.Id.parent_view;
        }

        #region Player events
        private void SongListButton_Click(object sender, EventArgs ee)
        {
            LoadFragmentInner(Enums.Activitys.Player.CurrentSongList, screen: Enums.LayoutScreenState.FullScreen);
        }

        public void NextSongPlayer()
        {
        }

        public void PrevSongPlayer()
        {
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
        }

        private void PreviewSong_Click(object sender, EventArgs e)
        {
            Pager.SetCurrentItem(LastPosition == 0 ? 0 : LastPosition - 1, true);
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            Pager.SetCurrentItem(LastPosition + 1, true);
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            int tag = (int)PlayToggle.Tag;
            if (tag != Resource.Drawable.play_loading)
                SongLoadStarted();
            else
                return;

            if (tag == Resource.Drawable.play_button)
            {
                SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
            }
            else if (tag == Resource.Drawable.pause)
            {
                SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.pause"));
            }
        }

        public void PlayerPrepared(int duration)
        {
        }

        public void Music_play()
        {
        }

        public void Music_pause()
        {
            PlayToggle.SetImageResource(Resource.Drawable.play_button);
            PlayToggle.Tag = Resource.Drawable.play_button;
        }

        public void SetSeekBarProgress(int progress, string text)
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    CurretSongTimeText.Text = text;
                    if (!SeekActive && SongTimeSeekBar != null)
                        SongTimeSeekBar.Progress = progress;
                });
            });
        }

        public void SongEnded()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    //CurretSongTimeText.Text = "00:00";
                    //SongTimeSeekBar.Progress = 0;
                });
            });
        }

        public void SongStopped()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    PlayToggle.Tag = Resource.Drawable.play_button;
                });
            });
        }

        //This method must be call then song is setted to refresh main UI view
        public void SongLoadStarted(List<Songs> newSongList, int position)
        {
        }



        public void SongLoadEnded()
        {
            Task.Run(() =>
            {
                RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

        private void SongTimeSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
        }

        private void SongTimeSeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            Intent intent = new Intent("com.spotypie.adnroid.musicservice.seek");
            intent.PutExtra("PLAYER_SEEK", e.SeekBar.Progress.ToString());
            SendBroadcast(intent);
            SeekActive = false;
        }

        private void SongTimeSeekBar_StartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            SeekActive = true;
        }

        private void Repeat_Click(object sender, EventArgs e)
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

        private void Shuffle_Click(object sender, EventArgs e)
        {
            if (Shuffle_state)
                Shuffle.SetImageResource(Resource.Drawable.shuffle_disabled);
            else
                Shuffle.SetImageResource(Resource.Drawable.shuffle_variant);


            Shuffle_state = !Shuffle_state;
        }

        private void Save_to_songs_Click(object sender, EventArgs e)
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
                            PreviewSong_Click(null, null);
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
                            NextSong_Click(null, null);
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

        #region MediaSession Callback

        public void OnDurationChanged(int miliseconds)
        {
            RunOnUiThread(() =>
            {
                TotalSongTimeText.Text = new TimeSpan(0, 0, 0, 0, miliseconds).ToString(@"mm\:ss");
                TotalSongTimeText.Visibility = ViewStates.Visible;
            });
        }

        public void OnPositionChanged(int miliseconds, TimeSpan currentTime)
        {
            RunOnUiThread(() =>
            {
                if (SongTimeSeekBar.Enabled)
                {
                    SongTimeSeekBar.Progress = miliseconds;
                    CurretSongTimeText.Text = currentTime.ToString(@"mm\:ss");
                }
            });
        }

        public void OnCompletion()
        {
            throw new NotImplementedException();
        }

        public void OnPlaybackStatusChanged(int state)
        {
            switch (state)
            {
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying:
                    OnPlay();
                    break;
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering:
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StateNone:
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    PlayToggle.Tag = Resource.Drawable.play_loading;
                    break;
                case Android.Support.V4.Media.Session.PlaybackStateCompat.StatePaused:
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                    PlayToggle.Tag = Resource.Drawable.play_button;
                    break;
            }
        }

        public void OnPlaybackMetaDataChanged(MediaMetadataCompat meta)
        {
            throw new NotImplementedException();
        }

        public void OnError(string error)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Music service connection

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            if (!IsBinded)
            {
                var binder = service as Music.MusicServiceBinder;
                binder.SetMusicServiceUpdateCallback(this);
                IsBinded = true;
                //SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            ShowMessage("Service unbinded");
        }

        public void OnPlay()
        {
            Pager.Enable(true);
            Task.Run(() =>
            {
                Songs song = QueueHelper.GetPlayingSong();
                RunOnUiThread(() =>
                {
                    SongTimeSeekBar.Enabled = true;

                    TitleHelper.Format(PlayerSongName, song.Name, 14);
                    TitleHelper.Format(PlayerArtistName, song.ArtistName, 12);
                    TitleHelper.Format(CurrentSongListValue, song.AlbumName, 12);

                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

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

        #endregion
    }
}