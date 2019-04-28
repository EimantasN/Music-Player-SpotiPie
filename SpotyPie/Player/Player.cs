using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using SpotyPie.Models;
using SpotyPie.Services;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Player
{
    public class Player : SupportFragment, View.IOnTouchListener, ServiceCallbacks, IServiceConnection
    {
        View RootView;

        private int ViewLoadState = 0;
        private bool Bound { get; set; } = false;

        private MusicService MusicService;

        private IServiceConnection ServiceConnection;

        private string Current_Player_Image { get; set; }

        protected float mLastPosY;
        protected static int newsId = 0;
        protected const int OffsetContainer = 250;
        protected int FragmentWidth = 0;

        private int CurrentState { get; set; } = 1;

        PlaylistSongList PlayerSongList;

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);
        bool saved_to_songs = false;
        bool Updating = false;

        public ImageButton HidePlayerButton;
        public ImageButton PlayToggle;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        public ImageView Player_Image;
        public TextView Player_song_name;
        public TextView Player_artist_name;
        public TextView Player_playlist_name;

        ImageButton SongListButton;

        SeekBar SongTimeSeekBar;

        ImageButton Repeat;

        ImageButton Shuffle;

        bool Shuffle_state = false;

        MainActivity ParentActivity;

        ImageView Save_to_songs;

        private int CurrentSongPosition = 0;
        private bool SeekActive = false;

        public Current_state GetState()
        {
            return ParentActivity.GetState();
        }

        public FrameLayout PlayerSongListContainer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.player, container, false);

            ServiceConnection = this;

            ParentActivity = (MainActivity)Activity;

            SongListButton = RootView.FindViewById<ImageButton>(Resource.Id.song_list);
            SongListButton.Click += SongListButton_Click;

            NextSong = RootView.FindViewById<ImageButton>(Resource.Id.next_song);
            NextSong.Click += NextSong_Click;
            PreviewSong = RootView.FindViewById<ImageButton>(Resource.Id.preview_song);
            PreviewSong.Click += PreviewSong_Click;

            Repeat = RootView.FindViewById<ImageButton>(Resource.Id.repeat);
            Repeat.Click += Repeat_Click;
            Shuffle = RootView.FindViewById<ImageButton>(Resource.Id.shuffle);
            Shuffle.Click += Shuffle_Click;
            Save_to_songs = RootView.FindViewById<ImageView>(Resource.Id.save_to_songs);
            Save_to_songs.Click += Save_to_songs_Click;

            Player_Image = RootView.FindViewById<ImageView>(Resource.Id.album_image);
            Player_song_name = RootView.FindViewById<TextView>(Resource.Id.song_name);
            Player_artist_name = RootView.FindViewById<TextView>(Resource.Id.artist_name);
            Player_playlist_name = RootView.FindViewById<TextView>(Resource.Id.playlist_name);

            CurretSongTimeText = RootView.FindViewById<TextView>(Resource.Id.current_song_time);
            TotalSongTimeText = RootView.FindViewById<TextView>(Resource.Id.total_song_time);
            TotalSongTimeText.Visibility = ViewStates.Invisible;

            HidePlayerButton = RootView.FindViewById<ImageButton>(Resource.Id.back_button);
            PlayToggle = RootView.FindViewById<ImageButton>(Resource.Id.play_stop);

            if (GetState().IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            HidePlayerButton.Click += HidePlayerButton_Click;
            PlayToggle.Click += PlayToggle_Click;
            Repeat_Click(null, null);
            Shuffle_Click(null, null);

            FragmentWidth = Resources.DisplayMetrics.WidthPixels;

            SongTimeSeekBar = RootView.FindViewById<SeekBar>(Resource.Id.seekBar);

            SongTimeSeekBar.StartTrackingTouch += SongTimeSeekBar_StartTrackingTouch;
            SongTimeSeekBar.StopTrackingTouch += SongTimeSeekBar_StopTrackingTouch;
            SongTimeSeekBar.ProgressChanged += SongTimeSeekBar_ProgressChanged;

            return RootView;
        }


        private void SongTimeSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (MusicService?.MusicPlayer?.Duration != null)
                CurrentSongPosition = (int)(MusicService.MusicPlayer.Duration * e.Progress / 100);
        }

        private void SongTimeSeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            MusicService?.SeekToPlayer(CurrentSongPosition);
            SeekActive = false;
        }

        private void SongTimeSeekBar_StartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            SeekActive = true;
        }

        public override void OnResume()
        {
            if (!IsMyServiceRunning(typeof(MusicService)))
            {
                this.Activity.StartService(new Intent(this.Activity, typeof(MusicService)));
            }

            Intent intent = new Intent(this.Activity, typeof(MusicService));
            this.Activity.BindService(intent, this.ServiceConnection, Bind.AutoCreate);
            Player_Image.SetOnTouchListener(this);
            base.OnResume();
        }

        public override void OnDestroy()
        {
            Player_Image.SetOnTouchListener(null);
            base.OnDestroy();
        }

        public override void OnStop()
        {
            base.OnStop();
            if (Bound)
            {
                MusicService.SetCallbacks(null); // unregister
                this.Activity.UnbindService(ServiceConnection);
                Bound = false;
            }
        }

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
                            NextSong_Click(null, null);
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
                            PreviewSong_Click(null, null);
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

        private PlaylistSongList GetSongListFragment()
        {
            if (PlayerSongList == null)
            {
                PlayerSongList = new PlaylistSongList();

                PlayerSongListContainer = RootView.FindViewById<FrameLayout>(Resource.Id.player_frame);
                PlayerSongListContainer.Visibility = ViewStates.Gone;
            }

            return PlayerSongList;
        }

        private void SongListButton_Click(object sender, EventArgs ee)
        {
            try
            {
                if (!GetSongListFragment().IsAdded)
                {
                    ChildFragmentManager.BeginTransaction()
                        .Add(Resource.Id.player_frame, GetSongListFragment())
                        .Commit();
                }
                else
                {
                    GetSongListFragment().Update();
                }
                CurrentState = 2;
                PlayerSongListContainer.TranslationX = 0;
                PlayerSongListContainer.Visibility = ViewStates.Visible;
                PlayerSongListContainer.BringToFront();
            }
            catch (Exception e)
            {
            }
        }

        public void NextSongPlayer()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Player_Image.TranslationX = FragmentWidth * -1;
                    Player_Image?.Animate().TranslationX(0);
                }, null);
            });
        }

        public void PrevSongPlayer()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    Player_Image.TranslationX = FragmentWidth;
                    Player_Image?.Animate().TranslationX(0);
                }, null);
            });
        }

        private void PreviewSong_Click(object sender, EventArgs e)
        {
            MusicService?.ChangeSong(false);
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            MusicService?.ChangeSong(true);
        }


        private void PlayToggle_Click(object sender, EventArgs e)
        {
            Play();
        }

        public void Play()
        {
            Music_play();
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
            GetState().Player_visiblibity_toggle();
        }

        #region Player events

        public void PlayerPrepared(int duration)
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    TotalSongTime = new TimeSpan(0, 0, (int)MusicService.MusicPlayer.Duration / 1000);
                    string totalTime = TotalSongTime.Minutes + ":" + (TotalSongTime.Seconds > 9 ? TotalSongTime.Seconds.ToString() : "0" + TotalSongTime.Seconds);
                    TotalSongTimeText.Text = totalTime;
                    TotalSongTimeText.Visibility = ViewStates.Visible;
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                }, null);
            });
        }

        public void Music_play()
        {
            PlayToggle.SetImageResource(Resource.Drawable.pause);
        }

        public void Music_pause()
        {
            PlayToggle.SetImageResource(Resource.Drawable.play_button);
        }

        public void SetSeekBarProgress(int progress, string text)
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    CurretSongTimeText.Text = text;
                    if (!SeekActive && SongTimeSeekBar != null)
                        SongTimeSeekBar.Progress = progress;
                }, null);
            });
        }

        public void SongEnded()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    CurretSongTimeText.Text = "0:00";
                    SongTimeSeekBar.Progress = 0;
                }, null);
            });
        }

        public void SongStopped()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                }, null);
            });
        }


        //This method must be call then song is setted to refresh main UI view
        public void SongLoadStarted(List<Songs> newSongList, int position)
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    TotalSongTimeText.Visibility = ViewStates.Invisible;

                    GetState().Current_Song_List = newSongList;
                    GetState().Current_Song = newSongList[position];

                    if (Current_Player_Image != newSongList[position].LargeImage)
                    {
                        Current_Player_Image = newSongList[position].LargeImage;
                        Picasso.With(Activity.ApplicationContext).Load(newSongList[position].LargeImage).Into(Player_Image);
                    }

                    Player_song_name.Text = GetState().Current_Song.Name;
                    //Player_artist_name = GetState().
                    //Player_playlist_name = RootView.FindViewById<TextView>(Resource.Id.playlist_name);

                    CurretSongTimeText.Text = "0.00";
                    SongTimeSeekBar.Progress = 0;
                    Player_song_name.Text = newSongList[position].Name;
                    ParentActivity.SongTitle.Text = newSongList[position].Name;
                    ParentActivity.ArtistName.Text = newSongList[position].ArtistName;
                    Player_artist_name.Text = newSongList[position].ArtistName;
                    if (ParentActivity.MiniPlayer.Visibility == ViewStates.Gone)
                        ParentActivity.MiniPlayer.Visibility = ViewStates.Visible;

                    ViewLoadState = 2;

                }, null);
            });
        }

        public void SongLoadEnded()
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                }, null);

            });
        }

        public void SongChangeStarted(List<Songs> song, int position)
        {
            Task.Run(() =>
            {
                Application.SynchronizationContext.Post(_ => { ParentActivity.TogglePlayer(true); }, null);
                MusicService?.SongChangeStarted(song, position);
            });
        }

        #endregion

        private void Repeat_Click(object sender, EventArgs e)
        {
            if (MusicService == null) return;
            switch (MusicService.Repeat_state)
            {
                case 0:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat);
                        MusicService.Repeat_state = 1;
                        if (MusicService != null) MusicService.MusicPlayer.Looping = true;
                        break;
                    }
                case 1:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat_once);
                        MusicService.Repeat_state = 2;
                        break;
                    }
                case 2:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat_off);
                        MusicService.Repeat_state = 0;
                        break;
                    }
            }
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

        public bool CheckChildFragments()
        {
            if (CurrentState == 1)
            {
            }
            else if (CurrentState == 2)
            {
                PlayerSongListContainer.Visibility = ViewStates.Gone;
                CurrentState = 1;
                return false;
            }
            return true;
        }

        public int? GetSongId()
        {
            return GetState()?.Current_Song?.Id;
        }

        public int? GetArtistId()
        {
            return GetState()?.Current_Artist?.Id;
        }

        public int? GetAlbumId()
        {
            return GetState()?.Current_Album?.Id;
        }

        public int? GetPlaylistId()
        {
            return GetState()?.Current_Playlist?.Id;
        }

        public List<Songs> GetSongList()
        {
            return GetState()?.Current_Song_List;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            LocalBinder binder = (LocalBinder)service;
            MusicService = binder.Service;
            Bound = true;
            MusicService.SetCallbacks(this);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Bound = false;
        }

        private bool IsMyServiceRunning(Type serviceClass)
        {
            ActivityManager manager = (ActivityManager)this.Activity.GetSystemService(Context.ActivityService);
            foreach (ActivityManager.RunningServiceInfo service in manager.GetRunningServices(int.MaxValue))
            {
                if (serviceClass.Name == service.Service.ClassName)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetViewLoadState()
        {
            return ViewLoadState;
        }

        public void SetViewLoadState()
        {
            ViewLoadState = 1;
        }
    }
}