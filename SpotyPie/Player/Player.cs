using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Mobile_Api.Models;
using Realms;
using SpotyPie.Base;
using SpotyPie.Enums;
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
    public class Player : FragmentBase, View.IOnTouchListener, IMusicPlayerUiControls
    {
        protected override Enums.LayoutScreenState ScreenState { get; set; } = LayoutScreenState.Default;
        public override NavigationColorState NavigationBtnColorState { get; set; } = NavigationColorState.Player;

        private int ViewLoadState = 0;

        private bool Bound { get; set; } = false;

        protected new MainActivity ParentActivity;

        private MusicService MusicService;

        private IServiceConnection ServiceConnection;

        private string CurrentPlayerImage { get; set; }

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
        public TextView PlayerSongName;
        public TextView PlayerArtistName;
        public TextView PlayerPlaylistName;

        ImageButton SongListButton;

        SeekBar SongTimeSeekBar;

        ImageButton Repeat;

        ImageButton Shuffle;

        bool Shuffle_state = false;

        ImageView Save_to_songs;

        private int CurrentSongPosition = 0;

        private bool SeekActive = false;


        private Realm Realm;
        private Realm_Songs RealObject;
        private IRealmCollection<Realm_Songs> Songs;

        private void SendSystemInfoData(dynamic m, EventArgs e)
        {
            if (Realm == null)
                Realm = Realm.GetInstance();

            var songList = Realm.All<Realm_Songs>().AsQueryable().ToList();

            if (RealObject != null)
            {
                RealObject.PropertyChanged -= RealObject_PropertyChanged;
            }
            if (songList.Count == 1)
                RealObject = (Realm_Songs)songList[0];
            else
            {
                foreach (var x in songList)
                {
                    if (x.IsPlaying == true)
                    {
                        RealObject = x;
                        break;
                    }
                }
                if (RealObject == null)
                {
                    RealObject = songList[0];
                }
            }
            RealObject.PropertyChanged += RealObject_PropertyChanged;
            Start(RealObject);
        }

        protected override void InitView()
        {
            RealmInit();

            GetActivity().Handler += SendSystemInfoData;

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
            PlayerSongName = RootView.FindViewById<TextView>(Resource.Id.song_name);
            PlayerSongName.Selected = true;
            PlayerArtistName = RootView.FindViewById<TextView>(Resource.Id.artist_name);
            PlayerArtistName.Selected = true;
            PlayerPlaylistName = RootView.FindViewById<TextView>(Resource.Id.playlist_name);

            CurretSongTimeText = RootView.FindViewById<TextView>(Resource.Id.current_song_time);
            TotalSongTimeText = RootView.FindViewById<TextView>(Resource.Id.total_song_time);
            TotalSongTimeText.Visibility = ViewStates.Invisible;

            HidePlayerButton = RootView.FindViewById<ImageButton>(Resource.Id.back_button);
            PlayToggle = RootView.FindViewById<ImageButton>(Resource.Id.play_stop);

            if (GetState().IsPlaying)
            {
                PlayToggle.SetImageResource(Resource.Drawable.pause);
                PlayToggle.Tag = Resource.Drawable.pause;
            }
            else
            {
                PlayToggle.SetImageResource(Resource.Drawable.play_button);
                PlayToggle.Tag = Resource.Drawable.play_button;
            }

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

        private void Songs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                //if (RealObject != null)
                //{
                //    RealObject.PropertyChanged -= RealObject_PropertyChanged;
                //}
                //if (e.NewItems.Count == 1)
                //    RealObject = (Realm_Songs)e.NewItems[0];
                //else
                //{
                //    Realm_Songs song;
                //    foreach (var x in e.NewItems)
                //    {
                //        song = (Realm_Songs)x;
                //        if (song.IsPlaying == true)
                //        {
                //            RealObject = song;
                //            break;
                //        }
                //    }
                //    if (RealObject == null)
                //    {
                //        RealObject = (Realm_Songs)e.NewItems[0];
                //    }
                //}
                //RealObject.PropertyChanged += RealObject_PropertyChanged;
                //Start(RealObject);
            }
        }

        private void RealObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Realm_Songs song = (Realm_Songs)sender;
            switch (e.PropertyName)
            {
                case "IsPlaying":
                    {
                        if (song.IsPlaying)
                        {
                            Start(song);
                        }
                        else
                        {
                            SongStopped();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void Start(Realm_Songs song)
        {
            Task.Run(() =>
            {
                Activity.RunOnUiThread(() =>
                {
                    TotalSongTimeText.Visibility = ViewStates.Invisible;

                    PlayerSongName.Text = song.Name;
                    PlayerArtistName.Text = song.ArtistName;

                    CurretSongTimeText.Text = "0.00";
                    SongTimeSeekBar.Progress = 0;
                    PlayerSongName.Text = song.Name;

                    //if (GetActivity().MiniPlayer.Visibility == ViewStates.Gone)
                    //    GetActivity().MiniPlayer.Visibility = ViewStates.Visible;
                    //ViewLoadState = 2;

                    PlayToggle.SetImageResource(Resource.Drawable.pause);
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
            LoadCustomImage(song);
        }

        public void SongLoadStarted()
        {
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

        public override void ForceUpdate()
        {
            //if (!IsMyServiceRunning(typeof(MusicService)))
            //{
            //    this.Activity.StartService(new Intent(this.Activity, typeof(MusicService)));
            //}

            //Intent intent = new Intent(this.Activity, typeof(MusicService));
            //Activity.BindService(intent, this.ServiceConnection, Bind.AutoCreate);

            if (Realm == null || Realm.IsClosed)
            {
                RealmInit();
            }

            RealObject = Realm.All<Realm_Songs>().FirstOrDefault(x => x.Id == Current_state.Id);
            if (RealObject != null)
            {
                Start(RealObject);
                RealObject.PropertyChanged += RealObject_PropertyChanged;
            }

            ImgHolder.SetOnTouchListener(this);
        }

        private void RealmInit()
        {
            Realm = Realm.GetInstance();
            Songs = Realm.All<Realm_Songs>().AsRealmCollection();
            Songs.CollectionChanged += Songs_CollectionChanged;
        }

        public override void ReleaseData()
        {
            Songs.CollectionChanged -= Songs_CollectionChanged;
            Realm?.Dispose();
            Realm = null;
            ImgHolder.SetOnTouchListener(null);
        }

        public override void LoadFragment(dynamic switcher)
        {
            switch (switcher)
            {
                case Enums.Activitys.Player.CurrentSongList:
                    GetActivity().FManager.SetCurrentFragment(new PlaylistSongList());
                    return;
            }
        }

        public override int GetParentView()
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
            SongLoadStarted();
            GetActivity().SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.prev"));
        }

        private void NextSong_Click(object sender, EventArgs e)
        {
            SongLoadStarted();
            GetActivity().SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.next"));
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
                GetActivity().SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.play"));
            }
            else if (tag == Resource.Drawable.pause)
            {
                GetActivity().SendBroadcast(new Intent("com.spotypie.adnroid.musicservice.pause"));
            }
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
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
            });
        }

        public void Music_play()
        {
            Task.Run(async () =>
            {
                while (MusicService?.MusicPlayer == null)
                    await Task.Delay(250);

                Activity.RunOnUiThread(() =>
                {
                    PlayToggle.SetImageResource(Resource.Drawable.play_loading);
                    PlayToggle.Tag = Resource.Drawable.play_loading;
                });
            });
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
                    PlayToggle.Tag = Resource.Drawable.play_button;
                });
            });
        }

        //This method must be call then song is setted to refresh main UI view
        public void SongLoadStarted(List<Songs> newSongList, int position)
        {
        }

        private void LoadCustomImage(Realm_Songs song)
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

                    List<Image> imageList = await ParentActivity.GetAPIService().GetNewImageForSongAsync(song.Id);
                    if (imageList == null || imageList.Count == 0)
                        LoadOld();
                    else
                    {
                        var img = imageList.OrderByDescending(x => x.Width).ThenByDescending(x => x.Height).First();
                        if (CurrentPlayerImage != song.LargeImage)
                        {
                            Activity.RunOnUiThread(() =>
                            {
                                CurrentPlayerImage = img.Url;
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
                    if (CurrentPlayerImage != song.LargeImage)
                    {
                        CurrentPlayerImage = song.LargeImage;
                        Picasso.With(Activity.ApplicationContext).Load(song.LargeImage).Resize(1200, 1200).CenterCrop().Into(ImgHolder);
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
                    PlayToggle.Tag = Resource.Drawable.pause;
                });
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