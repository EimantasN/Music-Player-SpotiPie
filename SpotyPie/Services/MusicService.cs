using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Media.Session;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Mobile_Api.Models;
using RestSharp;
using SpotyPie.Models;
using SpotyPie.Services.Restarters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpotyPie.Services
{
    [Service]
    public class MusicService : Service
    {
        private Mobile_Api.Service API { get; set; }

        private IBinder binder;

        private LockScreenMusicPlayer LockScreenPlayer;

        private bool Updating { get; set; } = false;

        private int RefreshRate = 250;

        private TimeSpan CurrentTime { get; set; } = new TimeSpan(0, 0, 0, 0, 0);

        private Object ProgressLock { get; set; } = new Object();

        public bool Start_music { get; set; }

        public bool PlayerIsVisible { get; set; }


        private ServiceCallbacks serviceCallbacks;

        public SongUpdate SongUpdate { get; set; }

        private Bth bluetooth;

        private AudioManager AudioManager { get; set; }

        private int Counter { get; set; } = 0;

        public bool Destoyed { get; set; } = true;

        public DateTime LastChecked { get; set; } = DateTime.Now.AddDays(-10);

        public DateTime Started { get; set; } = DateTime.Now;

        public DateTime RecievedInputFromHeadSet { get; private set; } = DateTime.Now;

        public int Repeat_state = 0;

        public MediaPlayer MusicPlayer;

        public int PrevId { get; set; }


        private const string BaseUrl = "https://pie.pertrauktiestaskas.lt/api/stream/play/";

        public Songs Current_Song { get; set; }

        public List<Songs> Current_Song_List { get; set; } = new List<Songs>();

        private Object _checkSongLock { get; set; } = new Object();

        public Mobile_Api.Service GetAPIService()
        {
            if (API == null)
                API = new Mobile_Api.Service();
            return API;
        }

        public override IBinder OnBind(Intent intent)
        {
            return binder;
        }

        public void SetCallbacks(ServiceCallbacks callbacks)
        {
            serviceCallbacks = callbacks;
        }

        private bool InputIn { get; set; } = false;

        public void SetSong(int position)
        {
            try
            {
                if (Current_Song == null || Current_Song.Id != Current_Song_List[position].Id)
                {
                    SetCurrentSong(position);

                    //if (serviceCallbacks == null)
                    Notification($"Now playing {Current_Song.Name}", $"{Current_Song.AlbumName} - by {Current_Song.ArtistName}", Current_Song.MediumImage);
                }
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        private void SetCurrentSong(int position)
        {
            try
            {
                if (SetNewSongActive(position))
                {
                    serviceCallbacks?.SetViewLoadState();
                    InformUiSongChanged(Current_Song_List, position);
                    LockScreenPlayer.SongLoadStarted();
                    LoadSong();
                }
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        private bool SetNewSongActive(int position)
        {
            lock (this)
            {
                if (Current_Song == null || Current_Song.Id != Current_Song_List[position].Id)
                {
                    PrevId = Current_Song == null ? Current_Song_List.Last().Id : Current_Song.Id;
                    Current_Song = Current_Song_List[position];
                    Current_Song.SetIsPlaying(true);
                    SongUpdate = new SongUpdate(Current_Song?.Id);
                    return true;
                }
                return false;
            }
        }

        private void LoadSong()
        {
            Task.Run(() =>
            {
                while (serviceCallbacks != null && serviceCallbacks.GetViewLoadState() != 2)
                    Thread.Sleep(25);

                try
                {
                    bool Starting = true;
                    while (Starting)
                    {
                        try
                        {
                            Starting = false;
                            if (MusicPlayer == null)
                                MusicPlayer = new MediaPlayer();

                            if (MusicPlayer.IsPlaying)
                                MusicPlayer.Stop();
                            MusicPlayer.Reset();
                            MusicPlayer.SetAudioStreamType(Android.Media.Stream.Music);
                            MusicPlayer.SetDataSource(BaseUrl + Current_Song.Id);
                            MusicPlayer.PrepareAsync();
                        }
                        catch (Exception e)
                        {
                            Starting = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Task.Run(async () =>
                    {
                        await GetAPIService().Report(e);
                        await GetAPIService().Corruped(Current_Song.Id);
                        await Task.Delay(500);
                        CheckSong();
                    });
                }
                finally
                {
                    Task.Run(async () =>
                    {
                        await GetAPIService().SetState(songId: Current_Song.Id);
                    });
                }
            });
        }

        public void CheckSong()
        {
            lock (_checkSongLock)
            {
                Application.SynchronizationContext.Post(_ => { ChangeSong(true); }, null);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (Destoyed == true)
            {
                MusicPlayer = new MediaPlayer();
                MusicPlayer.Prepared += MusicPlayer_Prepared;
                Destoyed = false;
                //bluetooth = new Bth();
                //bluetooth.Start();
                //var a = bluetooth.PairedDevices();
                CreateNotificationChannel();
            }
            if (!InputIn)
            {
                InputIn = true;
                HeadSetInput(intent.GetStringExtra("Data"));
            }
            return StartCommandResult.Sticky;
        }

        public void HeadSetInput(string input)
        {
            try
            {
                Enum.TryParse(input, out Keycode key);
                switch (key)
                {
                    case Keycode.MediaPlay:
                        {
                            PlayerPlay();
                            break;
                        }
                    case Keycode.MediaPause:
                        {
                            PlayerPause();
                            break;
                        }
                    case Keycode.MediaNext:
                        {
                            ChangeSong(true);
                            break;
                        }
                    case Keycode.MediaPrevious:
                        {
                            ChangeSong(false);
                            break;
                        }
                    default:
                        Console.WriteLine(key);
                        break;
                }
                Thread.Sleep(1000);
                InputIn = false;
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        private void PlayerPlay()
        {
            if (MusicPlayer == null)
                return;

            if (!MusicPlayer.IsPlaying)
            {
                serviceCallbacks?.Music_play();
                MusicPlayer.Start();
            }
        }

        private void PlayerPause()
        {
            if (MusicPlayer == null)
                return;

            if (MusicPlayer.IsPlaying)
            {
                serviceCallbacks?.Music_pause();
                MusicPlayer.Stop();
            }
        }

        public void SeekToPlayer(int position)
        {
            MusicPlayer?.SeekTo(position);
        }

        private void MusicPlayer_Prepared(object sender, EventArgs e)
        {
            try
            {
                serviceCallbacks?.PlayerPrepared(MusicPlayer == null ? 999 : MusicPlayer.Duration);

                LockScreenPlayer.SongLoaded(Current_Song);

                MusicPlayer?.Start();
                CurrentTime = new TimeSpan(0, 0, 0, 0);
                Task.Run(() => UpdateLoop());
            }
            catch (Exception ex)
            {
                Task.Run(() => GetAPIService().Report(ex));
            }
        }

        public override void OnCreate()
        {
            try
            {
                binder = new LocalBinder(this);
                LockScreenPlayer = new LockScreenMusicPlayer(this);
                base.OnCreate();
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        public override void OnDestroy()
        {
            Destoyed = true;
            Intent broadcastIntent = new Intent(this, typeof(MusicServiceRestart));
            SendBroadcast(broadcastIntent);
            base.OnDestroy();
        }

        private void CreateNotificationChannel()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    return;
                }

                string channelName = Resources.GetString(Resource.String.channelName);
                string channelDescription = Resources.GetString(Resource.String.channelDescription);
                string ChannelId = Resources.GetString(Resource.String.ChannelId);
                var channel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        private void Notification(string title, string content, string imgUrl)
        {
            try
            {
                //TODO set image to notification
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "SpotyPie hight quality music now possible")
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Drawable.img_loading);

                //Bitmap image = GetImage(imgUrl);
                //if (image != null)
                //    builder.SetLargeIcon(image);

                // Build the notification:
                Notification notification = builder.Build();

                // Get the notification manager:
                NotificationManager notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        public void ChangeSong(bool Foward)
        {
            try
            {
                if (Foward)
                    serviceCallbacks?.PrevSongPlayer();
                else
                    serviceCallbacks?.NextSongPlayer();

                if (Current_Song_List == null) return;

                for (int i = 0; i < Current_Song_List.Count; i++)
                {
                    if (Current_Song_List[i].IsPlayingNow())
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if (Foward)
                        {
                            if ((i + 1) == Current_Song_List.Count)
                            {
                                //Task run because in this method requst is made so it will block main thread
                                Task.Run(() => SongListEndCheckForAutoPlayAsync());
                            }
                            else
                            {
                                Current_Song_List[i + 1].SetIsPlaying(true);
                                SetSong(i + 1);
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                Current_Song_List[0].SetIsPlaying(true);
                                SetSong(0);
                            }
                            else
                            {

                                Current_Song_List[i - 1].SetIsPlaying(true);
                                SetSong(i - 1);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
        }

        private async Task SongListEndCheckForAutoPlayAsync()
        {
            try
            {
                Songs song = await GetAPIService().GetNextSong();
                if (!Current_Song_List.Any(x => x.Id == song.Id))
                {
                    Application.SynchronizationContext.Post(_ =>
                    {
                        song.SetModelType(Mobile_Api.Models.Enums.RvType.SongWithImage);
                        song.IsPlaying = true;
                        Current_Song_List.Add(song);
                        SetSong(Current_Song_List.Count - 1);
                    }, null);
                }
                else
                {
                    Application.SynchronizationContext.Post(_ => { SetSong(0); }, null);
                }
            }
            catch (Exception e)
            {
                Application.SynchronizationContext.Post(_ => { SetSong(0); }, null);
            }
        }

        public void SongChangeStarted(List<Songs> newSongList, int position)
        {
            Current_Song_List = newSongList;
            SetSong(position);
        }

        private void InformUiSongChanged(List<Songs> newSongList, int position)
        {
            serviceCallbacks?.SongLoadStarted(Current_Song_List, position);
        }

        public void SongEnded()
        {
            try
            {
                Application.SynchronizationContext.Post(_ =>
                {
                    if (CurrentTime.Seconds != 0)
                    {
                        CurrentTime = new TimeSpan(0, 0, 0, 0);
                        switch (Repeat_state)
                        {
                            case 0:
                                {
                                    ChangeSong(true);
                                    break;
                                }
                            case 1:
                                {
                                    MusicPlayer.SeekTo(0);
                                    MusicPlayer.Start();
                                    Task.Run(() => UpdateLoop());
                                    break;
                                }
                            case 2:
                                {
                                    serviceCallbacks?.SongStopped();
                                    //Stop music
                                    break;
                                }
                        }
                    }
                }, null);
            }
            catch (Exception e)
            {
                Task.Run(() => GetAPIService().Report(e));
            }
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
                                Progress = (int)(MusicPlayer.CurrentPosition * 100) / MusicPlayer.Duration;
                                Position = (int)MusicPlayer.CurrentPosition / 1000;
                                if (CurrentTime.Seconds < Position)
                                {

                                    Application.SynchronizationContext.Post(_ =>
                                    {
                                        SongUpdate.CalculateTime(MusicPlayer.Duration,
                                       async () => { await GetAPIService().Update<Songs>(Current_Song.Id); }
                                       );
                                    }, null);

                                    CurrentTime = new TimeSpan(0, 0, Position);
                                    text = CurrentTime.Minutes + ":" + (CurrentTime.Seconds > 9 ? CurrentTime.Seconds.ToString() : "0" + CurrentTime.Seconds);
                                    serviceCallbacks?.SetSeekBarProgress(Progress, text);
                                }

                                Thread.Sleep(RefreshRate);
                            }
                            catch (Exception e)
                            {
                                Task.Run(() => GetAPIService().Report(e));
                            }
                        }
                        Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
                        serviceCallbacks?.SongEnded();
                        if (Progress > 98)
                            Application.SynchronizationContext.Post(_ => { SongEnded(); }, null);
                        Task.Run(() => UpdateLoop());
                    }
                }
                catch (Exception e)
                {
                    Application.SynchronizationContext.Post(_ => { Updating = false; }, null);
                    Task.Run(() => GetAPIService().Report(e));
                }
            }
        }
    }
}