using Android.App;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Threading;
using System.Threading.Tasks;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace SpotyPie.Player
{
    public class Player : SupportFragment, View.IOnTouchListener
    {
        View RootView;

        private const string BaseUrl = "https://pie.pertrauktiestaskas.lt/api/stream/play/";

        private Object ProgressLock { get; set; } = new Object();
        private Object _checkSongLock { get; set; } = new Object();

        protected float mLastPosY;
        protected static int newsId = 0;
        protected const int OffsetContainer = 250;
        protected int FragmentWidth = 0;

        private int CurrentState { get; set; } = 1;

        PlaylistSongList PlayerSongList;

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);
        bool saved_to_songs = false;
        int RefreshRate = 100;
        bool Updating = false;

        public ImageButton HidePlayerButton;
        public ImageButton PlayToggle;
        public MediaPlayer MusicPlayer;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        ProgressBar SongProgress;

        public ImageView Player_Image;
        public TextView Player_song_name;
        public TextView Player_artist_name;
        public TextView Player_playlist_name;

        ImageButton SongListButton;

        ImageButton Repeat;
        int Repeat_state = 1;
        ImageButton Shuffle;
        bool Shuffle_state = false;

        MainActivity ParentActivity;

        ImageView Save_to_songs;

        public Current_state GetState()
        {
            return ParentActivity.GetState();
        }

        public FrameLayout PlayerSongListContainer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.player, container, false);

            ParentActivity = (MainActivity)Activity;

            PlayerSongList = new PlaylistSongList();

            PlayerSongListContainer = RootView.FindViewById<FrameLayout>(Resource.Id.player_frame);
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

            SongProgress = RootView.FindViewById<ProgressBar>(Resource.Id.song_progress);
            SongProgress.Touch += SongProgress_Touch;
            CurretSongTimeText = RootView.FindViewById<TextView>(Resource.Id.current_song_time);
            TotalSongTimeText = RootView.FindViewById<TextView>(Resource.Id.total_song_time);
            TotalSongTimeText.Visibility = ViewStates.Invisible;

            MusicPlayer = new MediaPlayer();
            MusicPlayer.Prepared += Player_Prepared;
            StartPlayMusic();

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

            return RootView;
        }

        public override void OnResume()
        {
            Player_Image.SetOnTouchListener(this);
            base.OnResume();
        }

        public override void OnDestroy()
        {
            Player_Image.SetOnTouchListener(null);
            base.OnDestroy();
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
                        if (transXExit < (FragmentWidth / 2))
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
                        if (Math.Abs(transXExit) < (FragmentWidth / 2))
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

        private void SongListButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!PlayerSongList.IsAdded)
                {
                    ChildFragmentManager.BeginTransaction()
                        .Add(Resource.Id.player_frame, PlayerSongList)
                        .Commit();
                }
                else
                {
                    PlayerSongList.Update();
                }
                CurrentState = 2;
                PlayerSongListContainer.Visibility = ViewStates.Visible;
                PlayerSongListContainer.BringToFront();
            }
            catch (Exception)
            {
            }
        }

        private void SongProgress_Touch(object sender, View.TouchEventArgs e)
        {
            var c = SongProgress.Width;

            float Procent = (e.Event.GetX() * 100) / SongProgress.Width;
            int Second = (int)(GetState().Current_Song.DurationMs * Procent / 100);
            MusicPlayer.SeekTo(Second);
        }

        private void PreviewSong_Click(object sender, EventArgs e)
        {
            GetState().ChangeSong(false);
            Player_Image.TranslationX = FragmentWidth;
            Player_Image.Animate().TranslationX(0);
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            GetState().ChangeSong(true);
            Player_Image.TranslationX = FragmentWidth * -1;
            Player_Image.Animate().TranslationX(0);
        }

        public void StartPlayMusic()
        {
            Task.Run(() =>
            {
                if (GetState().Start_music)
                {
                    try
                    {
                        MusicPlayer.Reset();
                        MusicPlayer.SetAudioStreamType(Stream.Music);
                        MusicPlayer.SetDataSource(BaseUrl + GetState().Current_Song.Id);
                        MusicPlayer.Prepare();

                        Application.SynchronizationContext.Post(_ =>
                        {
                            PlayToggle.SetImageResource(Resource.Drawable.pause);
                        }, null);
                    }
                    catch
                    {
                        Task.Run(() => CheckSong());
                    }
                }
            });
        }

        public void CheckSong()
        {
            lock (_checkSongLock)
            {
                Application.SynchronizationContext.Post(_ => { GetState().ChangeSong(true); }, null);
            }
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            GetState().Music_play_toggle();
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
            GetState().Player_visiblibity_toggle();
        }

        #region Player events

        private void Player_Prepared(object sender, EventArgs e)
        {
            TotalSongTimeText.Visibility = ViewStates.Visible;
            TotalSongTime = new TimeSpan(0, 0, (int)MusicPlayer.Duration / 1000);
            TotalSongTimeText.Text = TotalSongTime.Minutes + ":" + (TotalSongTime.Seconds > 9 ? TotalSongTime.Seconds.ToString() : "0" + TotalSongTime.Seconds);

            if (GetState().Start_music)
            {
                PlayToggle.SetImageResource(Resource.Drawable.pause);
                MusicPlayer.Start();
                CurrentTime = new TimeSpan(0, 0, 0, 0);
                Task.Run(() => UpdateLoop());
            }

            GetState().SetSongDuration(MusicPlayer.Duration);
        }

        public void UpdateLoop()
        {
            lock (ProgressLock)
            {
                try
                {
                    if (MusicPlayer != null && MusicPlayer.IsPlaying && !Updating)
                    {
                        Application.SynchronizationContext.Post(_ => { Updating = true; }, null);
                        int Progress = 0;
                        int Position = 0;
                        string text;
                        while (MusicPlayer.IsPlaying)
                        {
                            try
                            {
                                //Toast.MakeText(this.Context, "Pasotion -" + player.CurrentPosition + " - " + player.Duration, ToastLength.Short).Show();
                                Progress = (int)(MusicPlayer.CurrentPosition * 100) / MusicPlayer.Duration;
                                Application.SynchronizationContext.Post(_ => { SongProgress.Progress = Progress; }, null);
                                Position = (int)MusicPlayer.CurrentPosition / 1000;
                                if (CurrentTime.Seconds < Position)
                                {
                                    CurrentTime = new TimeSpan(0, 0, Position);
                                    text = CurrentTime.Minutes + ":" + (CurrentTime.Seconds > 9 ? CurrentTime.Seconds.ToString() : "0" + CurrentTime.Seconds);
                                    Application.SynchronizationContext.Post(_ => { CurretSongTimeText.Text = text; }, null);
                                }

                                Thread.Sleep(RefreshRate);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
                        Application.SynchronizationContext.Post(_ => { SongEnded(); }, null);
                        Task.Run(() => UpdateLoop());
                    }
                }
                catch (Exception)
                {
                    Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
                }
            }
        }

        private void Player_Error(object sender, MediaPlayer.ErrorEventArgs e)
        {
            Toast.MakeText(this.Context, "Player error", ToastLength.Short).Show();
            Task.Run(() => UpdateLoop());
            //player.Reset();
        }

        public void SongEnded()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                if (CurrentTime.Seconds != 0)
                {
                    CurrentTime = new TimeSpan(0, 0, 0, 0);
                    CurretSongTimeText.Text = "0:00";
                    SongProgress.Progress = 0;

                    switch (Repeat_state)
                    {
                        case 1:
                            {
                                GetState().ChangeSong(true);
                                break;
                            }
                        case 2:
                            {
                                MusicPlayer.SeekTo(0);
                                MusicPlayer.Start();
                                Task.Run(() => UpdateLoop());
                                break;
                            }
                        case 3:
                            {
                                PlayToggle.SetImageResource(Resource.Drawable.play_button);
                                //Stop music
                                break;
                            }
                    }
                }
            }, null);
        }

        #endregion

        private void Repeat_Click(object sender, EventArgs e)
        {
            switch (Repeat_state)
            {
                case 0:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat);
                        Repeat_state = 1;
                        MusicPlayer.Looping = true;
                        break;
                    }
                case 1:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat_once);
                        Repeat_state = 2;
                        break;
                    }
                case 2:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat_off);
                        Repeat_state = 0;
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
    }
}