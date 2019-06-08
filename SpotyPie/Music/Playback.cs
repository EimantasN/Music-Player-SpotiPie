using System;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using SpotyPie.Music.Helpers;
using SpotyPie.Music.Models;
using Android.Widget;
using System.Threading.Tasks;
using Android.App;

namespace SpotyPie.Music
{
    public class Playback : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener,
    MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener,
    MediaPlayer.IOnPreparedListener, MediaPlayer.IOnSeekCompleteListener
    {
        static readonly string Tag = LogHelper.MakeLogTag(typeof(Playback));

        public const float VolumeDuck = 0.2f;
        public const float VolumeNormal = 1.0f;

        const int AudioNoFocusNoDuck = 0;
        const int AudioNoFocusCanDuck = 1;
        const int AudioFocused = 2;

        readonly MusicService service;
        readonly WifiManager.WifiLock wifiLock;
        public int State { get; set; }
        bool playOnFocusGain;
        public ICallback Callback { get; set; }
        readonly MusicProvider musicProvider;
        volatile bool audioNoisyReceiverRegistered;
        volatile int currentPosition;
        volatile string currentMediaId;

        int audioFocus = AudioNoFocusNoDuck;
        AudioManager audioManager;
        MediaPlayer MediaPlayer;

        IntentFilter mAudioNoisyIntentFilter = new IntentFilter(AudioManager.ActionAudioBecomingNoisy);

        readonly BroadcastReceiver mAudioNoisyReceiver = new BroadcastReceiver();

        class BroadcastReceiver : Android.Content.BroadcastReceiver
        {
            public Action<Context, Intent> OnReceiveImpl { get; set; }
            public override void OnReceive(Context context, Intent intent)
            {
                OnReceiveImpl(context, intent);
            }
        }

        public Playback(MusicService service, MusicProvider musicProvider)
        {
            this.service = service;
            this.musicProvider = musicProvider;
            audioManager = (AudioManager)service.GetSystemService(Context.AudioService);
            wifiLock = ((WifiManager)service.GetSystemService(Context.WifiService))
                .CreateWifiLock(WifiMode.Full, "sample_lock");
            mAudioNoisyReceiver.OnReceiveImpl = (context, intent) => {
                if (AudioManager.ActionAudioBecomingNoisy == intent.Action)
                {
                    LogHelper.Debug(Tag, "Headphones disconnected.");
                    if (IsPlaying)
                    {
                        var i = new Intent(context, typeof(MusicService));
                        i.SetAction(MusicService.ActionCmd);
                        i.PutExtra(MusicService.CmdName, MusicService.CmdPause);
                        service.StartService(i);
                    }
                }
            };
        }

        public void Start()
        {

        }

        public void Stop(bool notifyListeners)
        {
            State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateStopped;

            if (notifyListeners && Callback != null)
            {
                Callback.OnPlaybackStatusChanged(State);
            }

            currentPosition = CurrentStreamPosition;
            GiveUpAudioFocus();
            UnregisterAudioNoisyReceiver();
            RelaxResources(true);
            if (wifiLock.IsHeld)
            {
                wifiLock.Release();
            }
        }

        public bool IsConnected = true;

        public bool IsPlaying
        {
            get
            {
                return playOnFocusGain || (MediaPlayer != null && MediaPlayer.IsPlaying);
            }
        }

        public int CurrentStreamPosition
        {
            get
            {
                return MediaPlayer != null ? MediaPlayer.CurrentPosition : currentPosition;
            }
        }

        public void Play(Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem item)
        {
            playOnFocusGain = true;
            TryToGetAudioFocus();
            RegisterAudioNoisyReceiver();
            string mediaId = musicProvider.GetCurrentSongId;
            bool mediaHasChanged = mediaId != currentMediaId;
            if (mediaHasChanged)
            {
                currentPosition = 0;
                currentMediaId = mediaId;
            }

            if (State == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePaused && !mediaHasChanged && MediaPlayer != null)
            {
                ConfigMediaPlayerState();
            }
            else
            {
                State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateStopped;
                RelaxResources(false);

                try
                {
                    State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering;

                    if (Callback != null)
                    {
                        Callback.OnPlaybackStatusChanged(State);
                    }

                    bool Starting = true;
                    while (Starting)
                    {
                        try
                        {
                            GetMediaPlayer();

                            Starting = false;
                            MediaPlayer.SetDataSourceAsync(musicProvider.CurrentSongSource());
                            MediaPlayer.PrepareAsync();

                            wifiLock.Acquire();
                        }
                        catch (Exception e)
                        {
                            Starting = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    Task.Run(() =>
                    {
                        //await GetAPIService().Report(e);
                        //await GetAPIService().Corruped(Current_Song.Id);
                        //await Task.Delay(500);
                        //CheckSong();
                    });

                    Callback?.OnError(e.Message);
                }
                finally
                {
                    Task.Run(() =>
                    {
                        //await GetAPIService().SetState(songId: Current_Song.Id);

                        //Application.SynchronizationContext.Post(_ =>
                        //{
                        //    LockScreenPlayer?.SetStatePlaying();
                        //}, null);
                    });
                    //State = Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying;
                    //if (Callback != null)
                    //{
                    //    Callback.OnPlaybackStatusChanged(State);
                    //}
                }
            }
        }

        public void GetMediaPlayer()
        {
            if (MediaPlayer == null)
            {
                MediaPlayer = new MediaPlayer();
                MediaPlayer.SetAudioStreamType(Stream.Music);

                MediaPlayer.SetWakeMode(service.ApplicationContext,
                    Android.OS.WakeLockFlags.Partial);

                MediaPlayer.SetOnPreparedListener(this);

                //MediaPlayer.SetOnCompletionListener(this);
                //MediaPlayer.SetOnErrorListener(this);
                //MediaPlayer.SetOnSeekCompleteListener(this);
            }
            else
            {
                MediaPlayer.Reset();
            }
        }

        public void Skip(object p)
        {
            State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateSkippingToNext;
            if (Callback != null)
            {
                Callback.OnPlaybackStatusChanged(State);
            }
        }

        public void Pause()
        {
            if (State == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying)
            {
                if (MediaPlayer != null && MediaPlayer.IsPlaying)
                {
                    MediaPlayer.Pause();
                    currentPosition = MediaPlayer.CurrentPosition;
                }
                RelaxResources(false);
                GiveUpAudioFocus();
            }
            State = Android.Support.V4.Media.Session.PlaybackStateCompat.StatePaused;
            if (Callback != null)
            {
                Callback.OnPlaybackStatusChanged(State);
            }
            UnregisterAudioNoisyReceiver();
        }

        public void SeekTo(int position)
        {
            if (MediaPlayer == null)
            {
                currentPosition = position;
            }
            else
            {
                if (MediaPlayer.IsPlaying)
                {
                    State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering;
                }
                MediaPlayer.SeekTo(position);
                if (Callback != null)
                {
                    Callback.OnPlaybackStatusChanged(State);
                }
            }
        }

        void TryToGetAudioFocus()
        {
            if (audioFocus != AudioFocused)
            {
                var result = audioManager.RequestAudioFocus(this, Android.Media.Stream.Music, AudioFocus.Gain);
                if (result == AudioFocusRequest.Granted)
                {
                    audioFocus = AudioFocused;
                }
            }
        }

        void GiveUpAudioFocus()
        {
            if (audioFocus == AudioFocused)
            {
                if (audioManager.AbandonAudioFocus(this) == AudioFocusRequest.Granted)
                {
                    audioFocus = AudioNoFocusNoDuck;
                }
            }
        }

        void ConfigMediaPlayerState()
        {
            if (audioFocus == AudioNoFocusNoDuck)
            {
                if (State == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying)
                {
                    Pause();
                }
            }
            else
            {
                if (audioFocus == AudioNoFocusCanDuck)
                {
                    MediaPlayer.SetVolume(VolumeDuck, VolumeDuck);
                }
                else
                {
                    if (MediaPlayer != null)
                    {
                        MediaPlayer.SetVolume(VolumeNormal, VolumeNormal);
                    }
                }
                if (playOnFocusGain)
                {
                    if (MediaPlayer != null && !MediaPlayer.IsPlaying)
                    {
                        if (currentPosition == MediaPlayer.CurrentPosition)
                        {
                            SetState(Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying);
                            MediaPlayer.Start();
                        }
                        else
                        {
                            SetState(Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering);
                            MediaPlayer.SeekTo(currentPosition);
                        }
                    }
                    playOnFocusGain = false;
                }
            }

            void SetState(int state)
            {
                State = state;
                if (Callback != null)
                {
                    Callback.OnPlaybackStatusChanged(State);
                }
            }
        }

        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            if (focusChange == AudioFocus.Gain)
            {
                audioFocus = AudioFocused;
            }
            else if (focusChange == AudioFocus.Loss || focusChange == AudioFocus.LossTransient || focusChange == AudioFocus.LossTransientCanDuck)
            {
                bool canDuck = focusChange == AudioFocus.LossTransientCanDuck;
                audioFocus = canDuck ? AudioNoFocusCanDuck : AudioNoFocusNoDuck;

                playOnFocusGain |= State == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying && !canDuck;
            }
            else
            {
            }
            ConfigMediaPlayerState();
        }

        public void OnCompletion(MediaPlayer mp)
        {
            if (Callback != null)
            {
                Callback.OnCompletion();
            }
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            Toast.MakeText(service.ApplicationContext, "Media player error: what=" + what + ", extra=" + extra, ToastLength.Long).Show();

            if (Callback != null)
            {
                Callback.OnError("MediaPlayer error " + what + " (" + extra + ")");
            }
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            Toast.MakeText(service.ApplicationContext, "onPrepared from MediaPlayer", ToastLength.Long).Show();

            ConfigMediaPlayerState();
        }

        public void OnSeekComplete(MediaPlayer mp)
        {
            Toast.MakeText(service.ApplicationContext, "OnSeekComplete", ToastLength.Long).Show();

            currentPosition = mp.CurrentPosition;
            if (State == Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering)
            {
                MediaPlayer.Start();
                State = Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying;
            }
            if (Callback != null)
            {
                Callback.OnPlaybackStatusChanged(State);
            }
        }

        private void RelaxResources(bool releaseMediaPlayer)
        {
            service.StopForeground(true);

            if (releaseMediaPlayer && MediaPlayer != null)
            {
                MediaPlayer.Reset();
                MediaPlayer.Release();
                MediaPlayer = null;
            }

            if (wifiLock.IsHeld)
            {
                wifiLock.Release();
            }
        }

        void RegisterAudioNoisyReceiver()
        {
            if (!audioNoisyReceiverRegistered)
            {
                service.RegisterReceiver(mAudioNoisyReceiver, mAudioNoisyIntentFilter);
                audioNoisyReceiverRegistered = true;
            }
        }

        void UnregisterAudioNoisyReceiver()
        {
            if (audioNoisyReceiverRegistered)
            {
                service.UnregisterReceiver(mAudioNoisyReceiver);
                audioNoisyReceiverRegistered = false;
            }
        }

        public interface ICallback
        {
            void OnCompletion();
            void OnPlaybackStatusChanged(int state);
            void OnError(string error);
        }
    }
}