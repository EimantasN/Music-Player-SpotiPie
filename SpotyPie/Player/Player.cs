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
    public class Player : SupportFragment
    {
        View RootView;
        SupportFragment PlayerSongList;

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);
        bool saved_to_songs = false;
        public int RefreshRate = 100;
        public bool Updating = false;

        ImageButton HidePlayerButton;
        public static ImageButton PlayToggle;
        public static MediaPlayer player;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public static TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        public static Android.Content.Context contextStatic;

        public ProgressBar SongProgress;

        public static ImageView Player_Image;
        public static TextView Player_song_name;
        public static TextView Player_artist_name;
        public static TextView Player_playlist_name; //July talk - Touch

        ImageButton SongListButton;

        ImageButton Repeat;
        int Repeat_state = 1;
        bool RepetedOnce = false;
        ImageButton Shuffle;
        bool Shuffle_state = false;

        ImageView Save_to_songs;

        public static FrameLayout PlayerSongListContainer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(Resource.Layout.player, container, false);
            contextStatic = this.Context;
            PlayerSongList = new player_song_list();
            PlayerSongListContainer = RootView.FindViewById<FrameLayout>(Resource.Id.player_frame);
            PlayerSongListContainer.TranslationX = MainActivity.widthInDp;
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

            player = new MediaPlayer();
            player.Prepared += Player_Prepared;
            player.BufferingUpdate += Player_BufferingUpdate;
            player.Completion += Player_Completion;
            StartPlayMusic();

            HidePlayerButton = RootView.FindViewById<ImageButton>(Resource.Id.back_button);
            PlayToggle = RootView.FindViewById<ImageButton>(Resource.Id.play_stop);

            if (Current_state.IsPlaying)
                PlayToggle.SetImageResource(Resource.Drawable.pause);
            else
                PlayToggle.SetImageResource(Resource.Drawable.play_button);

            HidePlayerButton.Click += HidePlayerButton_Click;
            PlayToggle.Click += PlayToggle_Click;
            Repeat_Click(null, null);
            Shuffle_Click(null, null);
            return RootView;
        }

        private void SongListButton_Click(object sender, EventArgs e)
        {
            try
            {
                ChildFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.player_frame, PlayerSongList)
                    .Commit();
                PlayerSongListContainer.TranslationX = 0;
            }
            catch (Exception)
            {
            }
        }

        private void SongProgress_Touch(object sender, View.TouchEventArgs e)
        {
            var c = SongProgress.Width;

            float Procent = (e.Event.GetX() * 100) / SongProgress.Width;
            int Second = (int)(Current_state.Current_Song.DurationMs * Procent / 100);
            Player.player.SeekTo(Second);
        }

        private void PreviewSong_Click(object sender, EventArgs e)
        {
            RepetedOnce = false;
            Current_state.ChangeSong(false);
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            RepetedOnce = false;
            Current_state.ChangeSong(true);
        }

        public static void StartPlayMusic()
        {
            Task.Run(() =>
            {
                try
                {
                    if (Current_state.Start_music)
                    {
                        Application.SynchronizationContext.Post(_ =>
                            {
                                try
                                {
                                    player.Reset();
                                    player.SetAudioStreamType(Stream.Music);
                                    player.SetDataSource("http://pie.pertrauktiestaskas.lt/api/stream/play/" + Current_state.Current_Song.Id);
                                    player.Prepare();
                                }
                                catch (Exception e)
                                {
                                    Toast.MakeText(contextStatic, "Can't play " + Current_state.Current_Song.Id.ToString(), ToastLength.Short).Show();
                                }
                            }, null);
                    }
                }
                catch (Exception)
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        Toast.MakeText(contextStatic, "Can't play " + Current_state.Current_Song.Id.ToString(), ToastLength.Short).Show();
                    }, null);
                }
            });
        }

        private void PlayToggle_Click(object sender, EventArgs e)
        {
            Current_state.Music_play_toggle();
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
            Current_state.Player_visiblibity_toggle();
        }

        #region Player events
        private void Player_Prepared(object sender, EventArgs e)
        {
            TotalSongTimeText.Visibility = ViewStates.Visible;
            TotalSongTime = new TimeSpan(0, 0, (int)player.Duration / 1000);
            TotalSongTimeText.Text = TotalSongTime.Minutes + ":" + (TotalSongTime.Seconds > 9 ? TotalSongTime.Seconds.ToString() : "0" + TotalSongTime.Seconds);

            if (Current_state.Start_music)
            {
                PlayToggle.SetImageResource(Resource.Drawable.pause);
                player.Start();
                CurrentTime = new TimeSpan(0, 0, 0, 0);
                Task.Run(() => UpdateLoop());
            }
        }

        private void Player_BufferingUpdate(object sender, MediaPlayer.BufferingUpdateEventArgs e)
        {
        }

        public async Task UpdateLoop()
        {
            try
            {
                if (player != null && player.IsPlaying && !Updating)
                {
                    Application.SynchronizationContext.Post(_ => { Updating = true; }, null);
                    int Progress = 0;
                    int Position = 0;
                    while (player.IsPlaying)
                    {
                        try
                        {
                            //Toast.MakeText(this.Context, "Pasotion -" + player.CurrentPosition + " - " + player.Duration, ToastLength.Short).Show();
                            Progress = (int)(player.CurrentPosition * 100) / player.Duration;
                            Application.SynchronizationContext.Post(_ => { SongProgress.Progress = Progress; }, null);
                            Position = (int)player.CurrentPosition / 1000;
                            if (CurrentTime.Seconds < Position)
                            {
                                CurrentTime = new TimeSpan(0, 0, Position);
                                var text = CurrentTime.Minutes + ":" + (CurrentTime.Seconds > 9 ? CurrentTime.Seconds.ToString() : "0" + CurrentTime.Seconds);
                                Application.SynchronizationContext.Post(_ => { CurretSongTimeText.Text = text; }, null);
                            }

                            if (player.Looping == true && CurrentTime.TotalSeconds == TotalSongTime.TotalSeconds)
                            {
                                Application.SynchronizationContext.Post(_ =>
                                {
                                    CurretSongTimeText.Text = "0:00";
                                    SongProgress.Progress = 0;
                                    CurrentTime = new TimeSpan(0, 0, 0);
                                }, null);
                            }

                            await Task.Delay(RefreshRate);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    Thread.Sleep(250);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(() => UpdateLoop());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
                }
            }
            catch (Exception)
            {
                Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
            }
            finally
            {
                Application.SynchronizationContext.Post(_ => { Current_state.Music_play_toggle(); }, null);
            }
        }

        private void Player_Error(object sender, MediaPlayer.ErrorEventArgs e)
        {
            Toast.MakeText(this.Context, "Player error", ToastLength.Short).Show();
            Task.Run(() => UpdateLoop());
            //player.Reset();
        }

        private void Player_Completion(object sender, EventArgs e)
        {
            if (!RepetedOnce)
            {
                if (Repeat_state == 2)
                {
                    RepetedOnce = true;
                    player.SeekTo(0);
                    player.Start();
                }
            }
            CurrentTime = new TimeSpan(0, 0, 0, 0);
            CurretSongTimeText.Text = "0:00";
            SongProgress.Progress = 0;
        }
        #endregion


        private void Repeat_Click(object sender, EventArgs e)
        {
            RepetedOnce = false;
            switch (Repeat_state)
            {
                case 0:
                    {
                        Repeat.SetImageResource(Resource.Drawable.repeat);
                        Repeat_state = 1;
                        player.Looping = true;
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

    }
}