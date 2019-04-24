using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Mobile_Api.Models;
using SpotyPie.Services.Restarters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotyPie.Services
{
    [Service]
    public class MusicService : Service
    {
        private IBinder binder;

        private ServiceCallbacks serviceCallbacks;

        private Bth bluetooth;

        private AudioManager AudioManager { get; set; }

        private int Counter { get; set; } = 0;

        public bool Destoyed { get; set; } = true;

        public DateTime LastChecked { get; set; } = DateTime.Now.AddDays(-10);

        public DateTime Started { get; set; } = DateTime.Now;

        public DateTime RecievedInputFromHeadSet { get; private set; } = DateTime.Now;

        public MediaPlayer MusicPlayer;

        private int PrevId { get; set; }


        private const string BaseUrl = "https://pie.pertrauktiestaskas.lt/api/stream/play/";

        private int CurrentSong { get; set; }

        public Songs Current_Song { get; set; }

        public List<Songs> Current_Song_List { get; private set; } = new List<Songs>();

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

        public void SetSong(int? id)
        {
            if (id != null && CurrentSong != id)
            {
                SetCurrentSong((int)id);
                CurrentSong = (int)id;
            }
        }

        public void SetCurrentSong(int id)
        {
            if (serviceCallbacks?.GetSongList() != null)
            {
                Current_Song_List = serviceCallbacks.GetSongList();
            }
            Current_Song = Current_Song_List.First(x => x.Id == id);
            Current_Song.SetIsPlaying(true);

            if (CurrentSong != id)
            {
                PrevId = id;
                CurrentSong = Current_Song.Id;
                LoadSong();
            }
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
                    MusicPlayer.SetAudioStreamType(Stream.Music);
                    MusicPlayer.SetDataSource(BaseUrl + CurrentSong);
                    MusicPlayer.Prepare();
                }
                catch
                {
                    Task.Run(() => CheckSong());
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
                Task.Run(() => RunNotification());
                SetBthHeadSetButtons();
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
            }
            catch (Exception e)
            {

            }
            finally
            {
                InputIn = false;
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
            MusicPlayer?.Start();
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
                AudioManager.RegisterMediaButtonEventReceiver(new ComponentName(PackageName, new MediaButtonBroadcastReceiver().ComponentName));
            }
            catch (Exception e)
            {

            }
        }

        public override void OnDestroy()
        {
            Destoyed = true;
            Intent broadcastIntent = new Intent(this, typeof(MusicServiceRestart));
            SendBroadcast(broadcastIntent);

            base.OnDestroy();
        }

        private async Task RunNotification()
        {
            while (!Destoyed)
            {
                if (DateTime.Now.Subtract(LastChecked).Seconds > 10)
                {
                    Counter++;
                    LastChecked = DateTime.Now;
                    Notification("SpotyPie", $"Sec -> {DateTime.Now.Subtract(Started).TotalSeconds} | Count -> {Counter}");
                }
                else
                    await Task.Delay(100);

            }
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

        private void Notification(string title, string content)
        {
            try
            {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "SpotyPie hight quality music now possible")
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Drawable.logo_spotify);

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
                            SetSong(Current_Song_List[0].Id);
                        }
                        else
                        {
                            Current_Song_List[i + 1].SetIsPlaying(true);
                            SetSong(Current_Song_List[i + 1].Id);
                        }
                    }
                    else
                    {
                        Current_Song_List[i].SetIsPlaying(false);
                        if (i == 0)
                        {
                            Current_Song_List[0].SetIsPlaying(true);
                            SetSong(Current_Song_List[0].Id);
                        }
                        else
                        {

                            Current_Song_List[i - 1].SetIsPlaying(true);
                            SetSong(Current_Song_List[i - 1].Id);
                        }
                    }
                    break;
                }
            }
        }
    }
}