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
        private IBinder binder;

        private MediaSession mediaSession;

        private ComponentName remoteComponentName;

        private RemoteControlClient remoteControlClient;

        private bool Updating { get; set; } = false;

        private int RefreshRate = 100;

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
            if (Current_Song == null || Current_Song.Id != Current_Song_List[position].Id)
            {
                SetCurrentSong(position);

                if (serviceCallbacks == null)
                    Notification($"Now playing {Current_Song.Name}", $"", Current_Song.MediumImage);
            }
        }

        private void SetCurrentSong(int position)
        {
            InformUiSongChanged(Current_Song_List, position);
            PrevId = Current_Song == null ? Current_Song_List.Last().Id : Current_Song.Id;
            Current_Song = Current_Song_List[position];
            Current_Song.SetIsPlaying(true);
            SongUpdate = new SongUpdate(Current_Song?.Id);
            LoadSong();

            SetBthHeadSetButtons();
            remoteControlClient.SetPlaybackState(RemoteControlPlayState.Buffering);
            UpdateMetadata();
        }

        private void LoadSong()
        {
            Task.Run(() =>
            {
                try
                {
                    if (MusicPlayer == null)
                        MusicPlayer = new MediaPlayer();

                    MusicPlayer.Reset();
                    MusicPlayer.SetAudioStreamType(Android.Media.Stream.Music);
                    MusicPlayer.SetDataSource(BaseUrl + Current_Song.Id);
                    MusicPlayer.Prepare();
                }
                catch (Exception e)
                {
                    Task.Run(() =>
                    {
                        var API = new Mobile_Api.Service();
                        API.Corruped(Current_Song.Id);
                        CheckSong();
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
                bluetooth = new Bth();
                bluetooth.Start();
                var a = bluetooth.PairedDevices();
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
            if (remoteControlClient != null)
                remoteControlClient.SetPlaybackState(RemoteControlPlayState.Playing);
            UpdateMetadata();

            MusicPlayer?.Start();
            CurrentTime = new TimeSpan(0, 0, 0, 0);
            Task.Run(() => UpdateLoop());
            serviceCallbacks?.PlayerPrepared(MusicPlayer == null ? 999 : MusicPlayer.Duration);
        }

        public override void OnCreate()
        {
            try
            {
                binder = new LocalBinder(this);
                SetBthHeadSetButtons();
                base.OnCreate();
            }
            catch (Exception e)
            {
            }
        }

        private void SetBthHeadSetButtons()
        {
            try
            {
                if (AudioManager == null)
                    AudioManager = (AudioManager)this.GetSystemService("audio");
                remoteComponentName = new ComponentName(PackageName, new MediaButtonBroadcastReceiver().ComponentName);

                if (remoteControlClient == null)
                {
                    AudioManager.RegisterMediaButtonEventReceiver(remoteComponentName);
                    //Create a new pending intent that we want triggered by remote control client
                    var mediaButtonIntent = new Intent(Intent.ActionMediaButton);
                    mediaButtonIntent.SetComponent(remoteComponentName);
                    // Create new pending intent for the intent
                    var mediaPendingIntent = PendingIntent.GetBroadcast(this, 0, mediaButtonIntent, 0);
                    // Create and register the remote control client
                    remoteControlClient = new RemoteControlClient(mediaPendingIntent);
                    AudioManager.RegisterRemoteControlClient(remoteControlClient);
                }
                //add transport control flags we can to handle
                remoteControlClient.SetTransportControlFlags(RemoteControlFlags.Play |
                                         RemoteControlFlags.Pause |
                                         RemoteControlFlags.PlayPause |
                                         RemoteControlFlags.Stop |
                                         RemoteControlFlags.Previous |
                                         RemoteControlFlags.Next);
            }
            catch (Exception e)
            {

            }
        }

        private void UnregisterRemoteClient()
        {
            try
            {
                if (AudioManager != null)
                {
                    AudioManager.UnregisterMediaButtonEventReceiver(remoteComponentName);
                    AudioManager.UnregisterRemoteControlClient(remoteControlClient);
                    remoteControlClient.Dispose();
                    remoteControlClient = null;
                    AudioManager = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void UpdateMetadata()
        {
            if (remoteControlClient == null)
                return;

            var metadataEditor = remoteControlClient.EditMetadata(true);
            metadataEditor.PutString(MetadataKey.Album, Current_Song.Name);
            metadataEditor.PutString(MetadataKey.Artist, Current_Song.Popularity.ToString());
            metadataEditor.PutString(MetadataKey.Albumartist, "Raw Stiles");
            metadataEditor.PutString(MetadataKey.Title, Current_Song.Name);
            metadataEditor.PutBitmap(BitmapKey.Artwork, GetImage(Current_Song.MediumImage));
            metadataEditor.Apply();
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

                var channelName = "SpotyPie";
                var channelDescription = "SpotyPie hight quality music now possible";
                var ChannelId = "987456123";
                var channel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
            catch (Exception e)
            {

            }
        }

        public Bitmap GetImage(string url)
        {
            try
            {
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                var fileBytes = client.DownloadData(request);
                return BitmapFactory.DecodeStream(new MemoryStream(fileBytes));
            }
            catch (Exception e)
            {
                return null;
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
                .SetSmallIcon(Resource.Drawable.logo_spotify);

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
                Console.WriteLine(e.Message);
            }
        }

        public void ChangeSong(bool Foward)
        {
            if (serviceCallbacks != null)
            {
                if (Foward)
                    serviceCallbacks.PrevSongPlayer();
                else
                    serviceCallbacks.NextSongPlayer();
            }

            if (Current_Song_List == null) return;

            for (int i = 0; i < Current_Song_List.Count; i++)
            {
                if (Current_Song_List[i].IsPlayingNow())
                {
                    if (Foward)
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if ((i + 1) == Current_Song_List.Count)
                        {
                            Current_Song_List[0].SetIsPlaying(true);
                            SetSong(0);
                        }
                        else
                        {
                            Current_Song_List[i + 1].SetIsPlaying(true);
                            SetSong(i + 1);
                        }
                    }
                    else
                    {
                        Current_Song_List[i].SetIsPlaying(false);
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
            Application.SynchronizationContext.Post(_ =>
            {
                if (CurrentTime.Seconds != 0)
                {
                    CurrentTime = new TimeSpan(0, 0, 0, 0);
                    serviceCallbacks?.SongEnded();
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
                                    //TODO set song popularity update
                                    //Application.SynchronizationContext.Post(_ =>
                                    //{
                                    //    SongUpdate.CalculateTime(MusicPlayer.Duration,
                                    //   async () => { await ParentActivity.GetAPIService().UpdateSongPopularity(Current_Song.Id); }
                                    //   );
                                    //}, null);

                                    CurrentTime = new TimeSpan(0, 0, Position);
                                    text = CurrentTime.Minutes + ":" + (CurrentTime.Seconds > 9 ? CurrentTime.Seconds.ToString() : "0" + CurrentTime.Seconds);
                                    serviceCallbacks?.SetSeekBarProgress(Progress, text);
                                }

                                Thread.Sleep(RefreshRate);
                            }
                            catch (Exception ex)
                            {
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
    }
}