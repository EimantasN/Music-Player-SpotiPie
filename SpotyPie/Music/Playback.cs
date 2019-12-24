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
using System.Threading;
using SpotyPie.Music.Manager;

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
        volatile int currentMediaId;

        volatile bool IsUpdating = false;

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
                .CreateWifiLock(WifiMode.Full, "spotypie_lock");
            mAudioNoisyReceiver.OnReceiveImpl = (context, intent) =>
            {
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

        public void Stop(bool notifyListeners)
        {
            State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateStopped;

            if (notifyListeners)
            {
                Callback?.OnPlaybackStatusChanged(State);
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

        public void Play()
        {
            playOnFocusGain = true;
            TryToGetAudioFocus();
            RegisterAudioNoisyReceiver();
            int mediaId = musicProvider.GetCurrentSong();
            Callback?.OnPlaybackMetaDataChanged(musicProvider.GetMetadata());
            bool mediaHasChanged = mediaId != currentMediaId;
            if (mediaHasChanged)
            {
                currentPosition = 0;
                currentMediaId = mediaId;
            }
            else
            {
                musicProvider.SongResumed();
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

                    Callback?.OnPlaybackStatusChanged(State);

                    bool Starting = true;
                    while (Starting)
                    {
                        try
                        {
                            GetMediaPlayer();

                            Starting = false;
                            MediaPlayer.SetDataSource(musicProvider.CurrentSongSource());
                            MediaPlayer.PrepareAsync();

                            wifiLock.Acquire();
                        }
                        catch (Exception)
                        {
                            Starting = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    //musicProvider.GetApiService().Report(e);
                    //musicProvider.GetApiService().Corrupted(musicProvider.Id);
                }
                finally
                {
                    //musicProvider.GetApiService().SetState(songId: musicProvider.Id);
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

                MediaPlayer.SetOnCompletionListener(this);
                //MediaPlayer.SetOnErrorListener(this);
                MediaPlayer.SetOnSeekCompleteListener(this);
            }
            else
            {
                MediaPlayer.Reset();
            }
        }

        public void Skip(bool foward)
        {
            if (SongManager.Next())
            {
                Play();
            }
        }

        public void Pause()
        {
            musicProvider.SongPaused();
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
            Callback?.OnPlaybackStatusChanged(State);
            UnregisterAudioNoisyReceiver();
        }

        public void SeekTo(int position)
        {
            position = MediaPlayer.Duration * position / 100;
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
                Callback?.OnPlaybackStatusChanged(State);
            }
        }

        void TryToGetAudioFocus()
        {
            if (audioFocus != AudioFocused)
            {
                AudioFocusRequest result = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
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
                Callback?.OnPlaybackStatusChanged(State);
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
            Callback?.OnCompletion();
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            Toast.MakeText(service.ApplicationContext, "Media player error: what=" + what + ", extra=" + extra, ToastLength.Long).Show();

            Callback?.OnError("MediaPlayer error " + what + " (" + extra + ")");
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            ConfigMediaPlayerState();
            IsUpdating = false;
            MediaPlayerPositionWacher();
            Callback?.OnDurationChanged(MediaPlayer.Duration);
        }

        private void MediaPlayerPositionWacher()
        {
            new Thread(() =>
            {
                int audioSession = MediaPlayer.AudioSessionId;
                if (MediaPlayer != null && MediaPlayer.IsPlaying && !IsUpdating)
                {
                    IsUpdating = true;
                    TimeSpan CurrentTime = new TimeSpan(0, 0, 0);
                    int Progress = 0;
                    int Position = 0;
                    int sleepTime;
                    while (MediaPlayer.IsPlaying)
                    {
                        if (MediaPlayer.AudioSessionId != audioSession)
                            break;

                        try
                        {
                            Progress = (int)(MediaPlayer.CurrentPosition * 100) / MediaPlayer.Duration;
                            Position = (int)MediaPlayer.CurrentPosition / 1000;
                            if (CurrentTime.Seconds < Position)
                            {
                                CurrentTime = new TimeSpan(0, 0, Position);
                                Callback?.OnPositionChanged(Progress, CurrentTime);
                            }
                            sleepTime = 1000 - (MediaPlayer.CurrentPosition - Position * 1000) + 25;
                            if (sleepTime <= 1025)
                            {
                                Thread.Sleep(1000 - (MediaPlayer.CurrentPosition - Position * 1000) + 25);
                            }
                            else
                                Thread.Sleep(250);

                        }
                        catch (Exception e)
                        {
                            Thread.Sleep(500);
                        }
                    }
                }
                IsUpdating = false;
            }).Start();
        }

        public void OnSeekComplete(MediaPlayer mp)
        {

            currentPosition = mp.CurrentPosition;
            if (State == Android.Support.V4.Media.Session.PlaybackStateCompat.StateBuffering)
            {
                MediaPlayer.Start();
                State = Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying;
            }
            Callback?.OnPlaybackStatusChanged(State);
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
            void OnPlay();
            void OnDurationChanged(int miliseconds);
            void OnPositionChanged(int progress, TimeSpan currentTime);
            void OnCompletion();
            void OnPlaybackStatusChanged(int state);
            void OnPlaybackMetaDataChanged(Android.Support.V4.Media.MediaMetadataCompat meta);
            void OnError(string error);
        }
    }
}