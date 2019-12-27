using System;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Net.Wifi;
using SpotyPie.Music.Helpers;
using Android.Widget;
using System.Threading;
using SpotyPie.Music.Manager;
using Android.Support.V4.Media.Session;
using Mobile_Api;
using Android.OS;
using Android.Media.Session;
using Android.App;

namespace SpotyPie.Music
{
    public class Playback : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener,
    MediaPlayer.IOnCompletionListener, MediaPlayer.IOnErrorListener,
    MediaPlayer.IOnPreparedListener, MediaPlayer.IOnSeekCompleteListener
    {
        public const string ActionCmd = "com.spotypie.adnroid.musicservice.ACTION_CMD";
        public const string CmdName = "com.spotypie.adnroid.musicservice.CMD_NAME";
        public const string CmdPause = "com.spotypie.adnroid.musicservice.CMD_PAUSE";

        public const float VolumeDuck = 0.2f;
        public const float VolumeNormal = 1.0f;

        const int AudioNoFocusNoDuck = 0;
        const int AudioNoFocusCanDuck = 1;
        const int AudioFocused = 2;

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

        private readonly MusicService Service;

        readonly WifiManager.WifiLock wifiLock;

        //DELEGATES

        public delegate void OnStateChange(int state);
        public static OnStateChange StateHandler;

        public delegate void OnPositionChange(int position);
        public static OnPositionChange PositionHandler;

        public delegate void OnDurationChange(int duration);
        public static OnDurationChange DurationHandler;

        private int State { get; set; }

        bool playOnFocusGain;

        volatile bool audioNoisyReceiverRegistered;
        volatile int currentPosition;
        volatile int currentMediaId;

        volatile bool IsUpdating = false;

        int audioFocus = AudioNoFocusNoDuck;
        AudioManager audioManager;
        MediaPlayer MediaPlayer;
        MediaSessionCompat Session;

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

        public Playback(MusicService _service, MediaSessionCompat _session)
        {
            Service = _service;
            Session = _session;
            SetState(PlaybackStateCompat.StateNone);

            audioManager = (AudioManager)Service.GetSystemService(Context.AudioService);
            wifiLock = ((WifiManager)Service.GetSystemService(Context.WifiService))
                .CreateWifiLock(WifiMode.Full, "spotypie_lock");

            mAudioNoisyReceiver.OnReceiveImpl = (context, intent) =>
            {
                if (AudioManager.ActionAudioBecomingNoisy == intent.Action)
                {
                    if (IsPlaying)
                    {
                        var i = new Intent(context, typeof(MusicService));
                        i.SetAction(ActionCmd);
                        i.PutExtra(CmdName, CmdPause);
                        Service.StartService(i);
                    }
                }
            };
        }

        private void SetState(int state)
        {
            if (State != state)
            {
                State = state;
                StateHandler?.Invoke(State);
                UpdatePlaybackState();
            }
        }

        public void Stop(bool notifyListeners)
        {
            SetState(PlaybackStateCompat.StateStopped);

            currentPosition = CurrentStreamPosition;
            GiveUpAudioFocus();
            UnregisterAudioNoisyReceiver();
            RelaxResources(true);
            if (wifiLock.IsHeld)
            {
                wifiLock.Release();
            }
        }

        public void Play()
        {
            playOnFocusGain = true;
            TryToGetAudioFocus();
            TryToRegisterAudioNoisyReceiver();

            if (IsSongChange())
            {
                if (State == PlaybackStateCompat.StatePaused && MediaPlayer != null)
                {
                    ConfigMediaPlayerState();
                }
                else
                {
                    RelaxResources(false);
                    try
                    {
                        SetState(PlaybackStateCompat.StateBuffering);

                        bool Starting = true;
                        while (Starting)
                        {
                            try
                            {
                                GetMediaPlayer();

                                Starting = false;
                                MediaPlayer.SetDataSource($"{BaseClient.BaseUrl}api/stream/play/{SongManager.SongId}");
                                MediaPlayer.PrepareAsync();

                                wifiLock.Acquire();
                            }
                            catch (Exception)
                            {
                                Starting = true;
                            }
                        }
                    }
                    catch (Exception)
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
        }

        private bool IsSongChange()
        {
            bool mediaHasChanged = SongManager.SongId != currentMediaId;
            if (mediaHasChanged) //Song Changed
            {
                currentPosition = 0;
                currentMediaId = SongManager.SongId;

                return true;
            }
            else //Resuming old song
            {
                MediaPlayer.Start();

                return false;
            }
        }

        public void GetMediaPlayer()
        {
            if (MediaPlayer == null)
            {
                MediaPlayer = new MediaPlayer();
                MediaPlayer.SetAudioStreamType(Stream.Music);

                MediaPlayer.SetWakeMode(Service.ApplicationContext,
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
            if (foward && SongManager.Next())
            {
                //Do something after next song play
            }
            else if(SongManager.Prev())
            {
                //Do something after prev song play
            }
        }

        public void Pause()
        {
            SetState(PlaybackStateCompat.StatePaused);
            if (State == PlaybackStateCompat.StatePlaying)
            {
                if (MediaPlayer != null && MediaPlayer.IsPlaying)
                {
                    MediaPlayer.Pause();
                    currentPosition = MediaPlayer.CurrentPosition;
                }
                RelaxResources(false);
                GiveUpAudioFocus();
            }
            UnregisterAudioNoisyReceiver();
        }

        public void SeekTo(int position)
        {
            if (MediaPlayer != null)
            {
                if (MediaPlayer.IsPlaying)
                {
                    SetState(PlaybackStateCompat.StateBuffering);
                }
                MediaPlayer.SeekTo(position);
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
                if (State == PlaybackStateCompat.StatePlaying)
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
                            SetState(PlaybackStateCompat.StatePlaying);
                            MediaPlayer.Start();
                        }
                        else
                        {
                            SetState(PlaybackStateCompat.StateBuffering);
                            MediaPlayer.SeekTo(currentPosition);
                        }
                    }
                    playOnFocusGain = false;
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

                playOnFocusGain |= State == PlaybackStateCompat.StatePlaying && !canDuck;
            }
            else
            {
            }
            ConfigMediaPlayerState();
        }

        private bool OnCompleteCalled { get; set; }
        public void OnCompletion(MediaPlayer mp)
        {
            if (!OnCompleteCalled)
            {
                OnCompleteCalled = true;
                Skip(true);
            }
        }

        public bool OnError(MediaPlayer mp, MediaError what, int extra)
        {
            Toast.MakeText(Service.ApplicationContext, "Media player error: what=" + what + ", extra=" + extra, ToastLength.Long).Show();
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            OnCompleteCalled = false;
            IsUpdating = false;

            ConfigMediaPlayerState();
            MediaPlayerPositionWacher();

            SetDuration(mp.Duration);
            SetPosition(mp.CurrentPosition);
        }

        private void SetDuration(int duration)
        {
            DurationHandler?.Invoke(duration);
        }

        private void SetPosition(int position)
        {
            PositionHandler?.Invoke(position);
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
                    while (MediaPlayer != null && MediaPlayer.IsPlaying)
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
                                Application.SynchronizationContext.Post(_ =>
                                {
                                    SetPosition(MediaPlayer.CurrentPosition);
                                }, null);
                            }
                            sleepTime = 1000 - (MediaPlayer.CurrentPosition - Position * 1000) + 25;
                            if (sleepTime <= 1025)
                            {
                                if (MediaPlayer.AudioSessionId != audioSession)
                                    break;
                                var leftTimeToWait = 1000 - (MediaPlayer.CurrentPosition - Position * 1000) + 25;
                                if (leftTimeToWait > 0)
                                {
                                    Thread.Sleep(leftTimeToWait);
                                }
                            }
                            else
                            {
                                if (MediaPlayer.AudioSessionId != audioSession)
                                    break;

                                Thread.Sleep(250);
                            }

                        }
                        catch (Exception)
                        {
                            if (MediaPlayer.AudioSessionId != audioSession)
                                break;

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
            if (State == PlaybackStateCompat.StateBuffering)
            {
                MediaPlayer.Start();
                SetState(PlaybackStateCompat.StatePlaying);
                SetPosition(mp.CurrentPosition);
            }
        }

        private void RelaxResources(bool releaseMediaPlayer)
        {
            Service.StopForeground(true);

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

        void TryToRegisterAudioNoisyReceiver()
        {
            if (!audioNoisyReceiverRegistered)
            {
                Service.RegisterReceiver(mAudioNoisyReceiver, mAudioNoisyIntentFilter);
                audioNoisyReceiverRegistered = true;
            }
        }

        void UnregisterAudioNoisyReceiver()
        {
            if (audioNoisyReceiverRegistered)
            {
                Service.UnregisterReceiver(mAudioNoisyReceiver);
                audioNoisyReceiverRegistered = false;
            }
        }

        void UpdatePlaybackState()
        {
            var position = PlaybackStateCompat.PlaybackPositionUnknown;
            if (IsConnected)
            {
                position = CurrentStreamPosition;
            }

            var stateBuilder = new PlaybackStateCompat.Builder().SetActions(GetAvailableActions());

            stateBuilder.SetState(State, position, 1.0f, SystemClock.ElapsedRealtime());

            Session?.SetPlaybackState(stateBuilder.Build());
        }

        long GetAvailableActions()
        {
            long actions = PlaybackState.ActionPlay | PlaybackState.ActionPlayFromMediaId | PlaybackState.ActionPlayFromSearch;
            if (SongManager.SongQueue == null || SongManager.SongQueue.Count == 0)
            {
                return actions;
            }
            if (IsPlaying)
            {
                actions |= PlaybackState.ActionPause;
            }
            if (SongManager.Index > 0)
            {
                actions |= PlaybackState.ActionSkipToPrevious;
            }
            if (SongManager.Index < SongManager.SongQueue.Count - 1)
            {
                actions |= PlaybackState.ActionSkipToNext;
            }
            return actions;
        }
    }
}