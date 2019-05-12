using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
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
    public class Player : FragmentBase, View.IOnTouchListener, ServiceCallbacks, IServiceConnection
    {
        private int ViewLoadState = 0;

        private bool Bound { get; set; } = false;

        protected new MainActivity ParentActivity;

        private MusicService MusicService;

        private IServiceConnection ServiceConnection;

        private string Current_Player_Image { get; set; }

        protected float mLastPosY;
        protected static int newsId = 0;
        protected const int OffsetContainer = 250;
        protected int FragmentWidth = 0;

        public int CurrentState { get; set; } = 1;
        public override int LayoutId { get; set; } = Resource.Layout.player;

        TimeSpan CurrentTime = new TimeSpan(0, 0, 0, 0);
        TimeSpan TotalSongTime = new TimeSpan(0, 0, 0, 0);
        bool saved_to_songs = false;

        public ImageButton HidePlayerButton;
        public ImageButton PlayToggle;

        ImageButton NextSong;
        ImageButton PreviewSong;

        public TextView CurretSongTimeText;
        TextView TotalSongTimeText;

        public ImageView ImgHolder;
        public TextView Player_song_name;
        public TextView Player_artist_name;
        public TextView Player_playlist_name;

        ImageButton SongListButton;

        SeekBar SongTimeSeekBar;

        ImageButton Repeat;

        ImageButton Shuffle;

        bool Shuffle_state = false;

        ImageView Save_to_songs;

        private int CurrentSongPosition = 0;

        private bool SeekActive = false;

        protected override void InitView()
        {
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

            ImgHolder = RootView.FindViewById<ImageView>(Resource.Id.img_holder);
            Player_song_name = RootView.FindViewById<TextView>(Resource.Id.song_name);
            Player_song_name.Selected = true;
            Player_artist_name = RootView.FindViewById<TextView>(Resource.Id.artist_name);
            Player_artist_name.Selected = true;
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
        }

        public override int GetParentView()
        {
            return Resource.Id.parent_view;
        }

        public override void ForceUpdate()
        {
            if (!IsMyServiceRunning(typeof(MusicService)))
            {
                this.Activity.StartService(new Intent(this.Activity, typeof(MusicService)));
            }

            Intent intent = new Intent(this.Activity, typeof(MusicService));
            Activity.BindService(intent, this.ServiceConnection, Bind.AutoCreate);

            ImgHolder.SetOnTouchListener(this);
        }

        public override void ReleaseData()
        {
            ImgHolder.SetOnTouchListener(null);
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

        public override void LoadFragment(dynamic switcher)
        {
            switch (switcher)
            {
                case Enums.Activitys.Player.CurrentSongList:
                    ParentActivity.FManager.SetCurrentFragment(new PlaylistSongList());
                    return;
                default:
                    throw new Exception("Failed to find Fragment");
            }
        }

        #region Player events
        public void Play()
        {
            Music_play();
        }

        private void SongListButton_Click(object sender, EventArgs ee)
        {
            LoadFragmentInner(Enums.Activitys.Player.CurrentSongList);
        }

        public void NextSongPlayer()
        {
            Task.Run((Action)(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    this.ImgHolder.TranslationX = FragmentWidth * -1;
                    this.ImgHolder?.Animate().TranslationX(0);
                });
            }));
        }

        public void PrevSongPlayer()
        {
            Task.Run((Action)(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    this.ImgHolder.TranslationX = FragmentWidth;
                    this.ImgHolder?.Animate().TranslationX(0);
                });
            }));
        }

        private void HidePlayerButton_Click(object sender, EventArgs e)
        {
            GetState().Player_visiblibity_toggle();
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

        public void PlayerPrepared(int duration)
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    TotalSongTime = new TimeSpan(0, 0, (int)MusicService.MusicPlayer.Duration / 1000);
                    string totalTime = TotalSongTime.Minutes + ":" + (TotalSongTime.Seconds > 9 ? TotalSongTime.Seconds.ToString() : "0" + TotalSongTime.Seconds);
                    TotalSongTimeText.Text = totalTime;
                    TotalSongTimeText.Visibility = ViewStates.Visible;
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                });
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
                Activity.RunOnUiThread(() =>
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
                Activity.RunOnUiThread(() =>
                {
                    CurretSongTimeText.Text = "0:00";
                    SongTimeSeekBar.Progress = 0;
                });
            });
        }

        public void SongStopped()
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_button);
                });
            });
        }

        //This method must be call then song is setted to refresh main UI view
        public void SongLoadStarted(List<Songs> newSongList, int position)
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    TotalSongTimeText.Visibility = ViewStates.Invisible;

                    GetState().Current_Song_List = newSongList;
                    GetState().Current_Song = newSongList[position];

                    LoadCustomImage(newSongList, position);

                    Player_song_name.Text = GetState().Current_Song.Name;

                    CurretSongTimeText.Text = "0.00";
                    SongTimeSeekBar.Progress = 0;
                    Player_song_name.Text = newSongList[position].Name;
                    ParentActivity.SongTitle.Text = newSongList[position].Name;
                    ParentActivity.ArtistName.Text = newSongList[position].ArtistName;
                    Player_artist_name.Text = newSongList[position].ArtistName;
                    if (ParentActivity.MiniPlayer.Visibility == ViewStates.Gone)
                        ParentActivity.MiniPlayer.Visibility = ViewStates.Visible;

                    ViewLoadState = 2;

                });
            });
        }

        private void LoadCustomImage(List<Songs> newSongList, int position)
        {
            Task.Run((Func<Task>)(async () =>
            {
                var real = Realm.GetInstance();
                Database.ViewModels.Settings settings = real.All<Database.ViewModels.Settings>().First();

                if (!settings.CustomImagesSwitch)
                {
                    LoadOld();
                }
                else
                {
                    Activity.RunOnUiThread(() =>
                    {
                        ImgHolder.SetImageResource(Resource.Drawable.img_loading);
                    });

                    List<Image> imageList = await ParentActivity.GetAPIService().GetNewImageForSongAsync(newSongList[position].Id);
                    if (imageList == null || imageList.Count == 0)
                        LoadOld();
                    else
                    {
                        var img = imageList.OrderByDescending(x => x.Width).ThenByDescending(x => x.Height).First();
                        if (Current_Player_Image != newSongList[position].LargeImage)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                Current_Player_Image = img.Url;
                                Picasso.With(Activity.ApplicationContext).Load(img.Url).Resize(1200, 1200).CenterCrop().Into((ImageView)this.ImgHolder);
                            });
                        }
                    }

                }
            }));

            void LoadOld()
            {
                Activity.RunOnUiThread(() =>
                {
                    if (Current_Player_Image != newSongList[position].LargeImage)
                    {
                        Current_Player_Image = newSongList[position].LargeImage;
                        Picasso.With(Activity.ApplicationContext).Load(newSongList[position].LargeImage).Resize(1200, 1200).CenterCrop().Into((ImageView)this.ImgHolder);
                    }
                });
            }
        }

        public void SongLoadEnded()
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                });
            });
        }

        public void SongChangeStarted(List<Songs> song, int position)
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    ParentActivity.TogglePlayer(true);
                });
                MusicService?.SongChangeStarted(song, position);
            });
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

        #endregion

        //public bool CheckChildFragments()
        //{
        //    if (CurrentState == 1)
        //    {
        //    }
        //    else if (CurrentState == 2)
        //    {
        //        PlayerSongListContainer.Visibility = ViewStates.Gone;
        //        CurrentState = 1;
        //        return false;
        //    }
        //    return true;
        //}

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
            MusicServiceBinder binder = (MusicServiceBinder)service;
            Bound = true;
            MusicService = binder.Service;
            binder.Service.SetCallbacks(this);
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
    }
}