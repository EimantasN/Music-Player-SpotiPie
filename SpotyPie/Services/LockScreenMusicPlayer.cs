using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Media.Session;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using Mobile_Api.Models;
using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using static Android.Support.V4.Media.App.NotificationCompat;

namespace SpotyPie.Services
{
    public class LockScreenMusicPlayer
    {
        private const string BaseUrl = "https://pie.pertrauktiestaskas.lt/api/stream/play/";

        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";


        public AudioManager _audioManager;

        public MediaSessionCompat mediaSessionCompat;
        public MediaControllerCompat mediaControllerCompat;
        public MediaSessionCustomCallback _mediaSessionCallback;

        IBinder binder;

        public int MediaPlayerState
        {
            get
            {
                return (mediaControllerCompat.PlaybackState != null ?
                    mediaControllerCompat.PlaybackState.State :
                    PlaybackStateCompat.StateNone);
            }
        }

        private ComponentName _remoteComponentName;

        //public event StatusChangedEventHandler StatusChanged;

        //public event CoverReloadedEventHandler CoverReloaded;

        private Handler PlayingHandler;
        private Java.Lang.Runnable PlayingHandlerRunnable;


        private MusicService _musicService;

        //private RemoteControlClient _remoteControlClient;

        public LockScreenMusicPlayer(MusicService musicService)
        {
            _musicService = musicService;
            //Find our audio and notificaton managers
            _audioManager = (AudioManager)_musicService.GetSystemService("audio");

            _remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);
        }

        private void InitMediaSession()
        {
            try
            {
                if (mediaSessionCompat == null)
                {
                    Intent nIntent = new Intent(_musicService.ApplicationContext, typeof(MainActivity));
                    PendingIntent pIntent = PendingIntent.GetActivity(_musicService.ApplicationContext, 0, nIntent, 0);

                    _remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

                    mediaSessionCompat = new MediaSessionCompat(_musicService.ApplicationContext, "SpotyPie", _remoteComponentName, pIntent);
                    mediaControllerCompat = new MediaControllerCompat(_musicService.ApplicationContext, mediaSessionCompat.SessionToken);
                }

                mediaSessionCompat.Active = true;
                //mediaSessionCompat.SetCallback(new MediaSessionCustomCallback(new MediaPlayerServiceBinder(_musicService), this));

                mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected virtual void OnStatusChanged(EventArgs e)
        {
            //if (StatusChanged != null)
                //StatusChanged(this, e);
        }

        protected virtual void OnCoverReloaded(EventArgs e)
        {
            //if (CoverReloaded != null)
           // {
                //CoverReloaded(this, e);
            //    StartNotification();
                //UpdateMediaMetadataCompat();
           // }
        }

        protected virtual void OnPlaying(EventArgs e)
        {
           // if (Playing != null)
                //Playing(this, e);
        }

        protected virtual void OnBuffering(EventArgs e)
        {
            //if (Buffering != null)
                //Buffering(this, e);
        }

        private void InitializePlayer()
        {
            //Already is initilized in music service class
        }

        internal void LoadSongInSession(Songs current_Song)
        {
            try
            {
                if (_musicService?.GetMediaPlayer() != null)
                {
                    if (_musicService.GetMediaPlayer().IsPlaying)
                        _musicService.GetMediaPlayer().Stop();
                    _musicService.GetMediaPlayer().Reset();
                    _musicService.GetMediaPlayer().SetDataSourceAsync(BaseUrl + current_Song.Id);
                    _musicService.GetMediaPlayer().PrepareAsync();
                }
            }
            catch (Exception e)
            {

            }
        }

        public int Position
        {
            get
            {
                if (_musicService?.GetMediaPlayer() == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return -1;
                else
                    return _musicService.GetMediaPlayer().CurrentPosition;
            }
        }

        public int Duration
        {
            get
            {
                if (_musicService?.GetMediaPlayer() == null
                    || (MediaPlayerState != PlaybackStateCompat.StatePlaying
                        && MediaPlayerState != PlaybackStateCompat.StatePaused))
                    return 0;
                else
                    return _musicService.GetMediaPlayer().Duration;
            }
        }

        private Bitmap cover;

        public object Cover
        {
            get
            {
                if (cover == null)
                    cover = BitmapFactory.DecodeResource(_musicService.Resources, Resource.Drawable.img_loading);
                return cover;
            }
            private set
            {
                cover = value as Bitmap;
                OnCoverReloaded(EventArgs.Empty);
            }
        }

        public void SetState(int state)
        {
            UpdatePlaybackState(state);
        }

        public void UpdateMetaData(Songs song)
        {
            SetState(PlaybackStateCompat.StatePlaying);
            UpdateMediaMetadataCompat(song);
            StartNotification();
        }

        public void HandleIntent(Intent intent)
        {
            if (intent == null || intent.Action == null)
                return;

            String action = intent.Action;

            if (action.Equals(ActionPlay))
            {
                mediaControllerCompat.GetTransportControls().Play();
            }
            else if (action.Equals(ActionPause))
            {
                mediaControllerCompat.GetTransportControls().Pause();
            }
            else if (action.Equals(ActionPrevious))
            {
                mediaControllerCompat.GetTransportControls().SkipToPrevious();
            }
            else if (action.Equals(ActionNext))
            {
                mediaControllerCompat.GetTransportControls().SkipToNext();
            }
            else if (action.Equals(ActionStop))
            {
                mediaControllerCompat.GetTransportControls().Stop();
            }
        }

        public async Task Play(Songs song)
        {
            if (_musicService?.GetMediaPlayer() != null && MediaPlayerState == PlaybackStateCompat.StatePaused)
            {
                //We are simply paused so just start again
                _musicService.GetMediaPlayer().Start();
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                StartNotification();

                //Update the metadata now that we are playing
                UpdateMediaMetadataCompat(song);
                return;
            }

            if (_musicService?.GetMediaPlayer() == null)
                InitializePlayer();

            if (mediaSessionCompat == null)
                InitMediaSession();

            if (_musicService.GetMediaPlayer().IsPlaying)
            {
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                return;
            }

            try
            {
                UpdatePlaybackState(PlaybackStateCompat.StateBuffering);
                _musicService.GetMediaPlayer().PrepareAsync();

                UpdateMediaMetadataCompat(song);
                StartNotification();
                LoadSongInSession(song);
            }
            catch (Exception ex)
            {
                UpdatePlaybackState(PlaybackStateCompat.StateStopped);

                _musicService.GetMediaPlayer().Reset();
                _musicService.GetMediaPlayer().Release();
            }
        }

        public async Task Seek(int position)
        {
            await Task.Run(() => {
                if (_musicService?.GetMediaPlayer() != null)
                {
                    _musicService.GetMediaPlayer().SeekTo(position);
                }
            });
        }

        public async Task PlayNext()
        {
            if (_musicService?.GetMediaPlayer() != null)
            {
                _musicService.GetMediaPlayer().Reset();
                _musicService.GetMediaPlayer().Release();
            }

            UpdatePlaybackState(PlaybackStateCompat.StateSkippingToNext);

            //await Play();
        }


        public async Task PlayPause()
        {
            if (_musicService?.GetMediaPlayer() == null || (_musicService?.GetMediaPlayer() != null && MediaPlayerState == PlaybackStateCompat.StatePaused))
            {
                //await Play();
            }
            else
            {
                //await Pause();
            }
        }

        public async Task Pause()
        {
            await Task.Run(() => {
                if (_musicService?.GetMediaPlayer() == null)
                    return;

                if (_musicService.GetMediaPlayer().IsPlaying)
                    _musicService.GetMediaPlayer().Pause();

                UpdatePlaybackState(PlaybackStateCompat.StatePaused);
            });
        }

        public async Task Stop()
        {
            await Task.Run(() => {
                if (_musicService?.GetMediaPlayer() == null)
                    return;

                if (_musicService.GetMediaPlayer().IsPlaying)
                {
                    _musicService.GetMediaPlayer().Stop();
                }

                UpdatePlaybackState(PlaybackStateCompat.StateStopped);
                _musicService.GetMediaPlayer().Reset();
                StopNotification();
                //StopForeground(true);
                UnregisterMediaSessionCompat();
            });
        }

        private void UpdatePlaybackState(int state)
        {
            if (mediaSessionCompat == null || _musicService?.GetMediaPlayer() == null)
                return;

            try
            {
                PlaybackStateCompat.Builder stateBuilder = new PlaybackStateCompat.Builder()
                    .SetActions(
                        PlaybackStateCompat.ActionPause |
                        PlaybackStateCompat.ActionPlay |
                        PlaybackStateCompat.ActionPlayPause |
                        PlaybackStateCompat.ActionSkipToNext |
                        PlaybackStateCompat.ActionSkipToPrevious |
                        PlaybackStateCompat.ActionStop
                    )
                    .SetState(state, Position, 1.0f, SystemClock.ElapsedRealtime());

                mediaSessionCompat.SetPlaybackState(stateBuilder.Build());

                //Used for backwards compatibility
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    if (mediaSessionCompat.RemoteControlClient != null && mediaSessionCompat.RemoteControlClient.Equals(typeof(RemoteControlClient)))
                    {
                        RemoteControlClient remoteControlClient = (RemoteControlClient)mediaSessionCompat.RemoteControlClient;

                        RemoteControlFlags flags = RemoteControlFlags.Play
                            | RemoteControlFlags.Pause
                            | RemoteControlFlags.PlayPause
                            | RemoteControlFlags.Previous
                            | RemoteControlFlags.Next
                            | RemoteControlFlags.Stop;

                        remoteControlClient.SetTransportControlFlags(flags);
                    }
                }

                OnStatusChanged(EventArgs.Empty);

                if (state == PlaybackStateCompat.StatePlaying || state == PlaybackStateCompat.StatePaused)
                {
                    StartNotification();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void StartNotification()
        {
            try
            {
                if (mediaSessionCompat == null)
                    return;

                var pendingIntent = PendingIntent.GetActivity(_musicService.ApplicationContext, 0, new Intent(_musicService.ApplicationContext, typeof(MainActivity)), PendingIntentFlags.UpdateCurrent);
                MediaMetadataCompat currentTrack = mediaControllerCompat.Metadata;

                var style = new MediaStyle();
                style.SetMediaSession(mediaSessionCompat.SessionToken);

                Intent intent = new Intent(_musicService.ApplicationContext, typeof(MusicService));
                intent.SetAction(ActionStop);
                PendingIntent pendingCancelIntent = PendingIntent.GetService(_musicService.ApplicationContext, 1, intent, PendingIntentFlags.CancelCurrent);

                style.SetShowCancelButton(true);
                style.SetCancelButtonIntent(pendingCancelIntent);

                NotificationCompat.Builder builder = new NotificationCompat.Builder(context: _musicService.ApplicationContext)
                    .SetStyle(style)
                    .SetContentTitle(currentTrack.GetString(MediaMetadata.MetadataKeyTitle))
                    .SetContentText(currentTrack.GetString(MediaMetadata.MetadataKeyArtist))
                    .SetContentInfo(currentTrack.GetString(MediaMetadata.MetadataKeyAlbum))
                    .SetSmallIcon(Resource.Drawable.img_loading)
                    //.SetLargeIcon(Cover as Bitmap)
                    .SetContentIntent(pendingIntent)
                    .SetShowWhen(false)
                    .SetOngoing(MediaPlayerState == PlaybackStateCompat.StatePlaying)
                    .SetVisibility(NotificationCompat.VisibilityPublic);

                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPrevious, "Previous", ActionPrevious));
                AddPlayPauseActionCompat(builder);
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaNext, "Next", ActionNext));
                style.SetShowActionsInCompactView(0, 1, 2);

                NotificationManagerCompat.From(_musicService.ApplicationContext).Notify(1, builder.Build());
            }
            catch (Exception e)
            {

            }
        }

        private NotificationCompat.Action GenerateActionCompat(int icon, String title, String intentAction)
        {
            Intent intent = new Intent(_musicService.ApplicationContext, typeof(MusicService));
            intent.SetAction(intentAction);

            PendingIntentFlags flags = PendingIntentFlags.UpdateCurrent;
            if (intentAction.Equals(ActionStop))
                flags = PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetService(_musicService.ApplicationContext, 1, intent, flags);

            return new NotificationCompat.Action.Builder(icon, title, pendingIntent).Build();
        }

        private void AddPlayPauseActionCompat(NotificationCompat.Builder builder)
        {
            if (MediaPlayerState == PlaybackStateCompat.StatePlaying)
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPause, "Pause", ActionPause));
            else
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPlay, "Play", ActionPlay));
        }

        public void StopNotification()
        {
            NotificationManagerCompat nm = NotificationManagerCompat.From(_musicService.ApplicationContext);
            nm.CancelAll();
        }

        private void UpdateMediaMetadataCompat(Songs song = null)
        {
            try
            {
                if (mediaSessionCompat == null)
                    return;

                MediaMetadataCompat.Builder builder = new MediaMetadataCompat.Builder();

                if (song != null)
                {
                    builder
                    .PutString(MediaMetadata.MetadataKeyAlbum, song.AlbumName)
                    .PutString(MediaMetadata.MetadataKeyArtist, song.ArtistName)
                    .PutString(MediaMetadata.MetadataKeyTitle, song.Name);
                }
                else
                {
                    builder
                        .PutString(MediaMetadata.MetadataKeyAlbum, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyAlbum))
                        .PutString(MediaMetadata.MetadataKeyArtist, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyArtist))
                        .PutString(MediaMetadata.MetadataKeyTitle, mediaSessionCompat.Controller.Metadata.GetString(MediaMetadata.MetadataKeyTitle));
                }
                //builder.PutBitmap(MediaMetadata.MetadataKeyAlbumArt, Cover as Bitmap);

                mediaSessionCompat.SetMetadata(builder.Build());
            }
            catch (Exception e)
            {

            }
        }

        private void UnregisterMediaSessionCompat()
        {
            try
            {
                if (mediaSessionCompat != null)
                {
                    mediaSessionCompat.Dispose();
                    mediaSessionCompat = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnDestroy()
        {
            if (_musicService?.GetMediaPlayer() != null)
            {
                _musicService.GetMediaPlayer().Release();

                StopNotification();
                //StopForeground(true);
                UnregisterMediaSessionCompat();
            }
        }

        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (_musicService?.GetMediaPlayer() == null)
                        InitializePlayer();

                    if (!_musicService.GetMediaPlayer().IsPlaying)
                    {
                        _musicService.GetMediaPlayer().Start();
                    }

                    _musicService.GetMediaPlayer().SetVolume(1.0f, 1.0f);//Turn it up!
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if (_musicService.GetMediaPlayer().IsPlaying)
                        _musicService.GetMediaPlayer().SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }

        //private void SetupMediaSession()
        //{
        //    ComponentName mediaButtonReceiverComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

        //    Intent mediaButtonIntent = new Intent(Intent.ActionMediaButton);
        //    mediaButtonIntent.SetComponent(mediaButtonReceiverComponentName);


        //    PendingIntent mediaButtonReceiverPendingIntent = PendingIntent.GetBroadcast(_musicService.ApplicationContext, 0, mediaButtonIntent, 0);

        //    var mediaSession = new MediaSessionCompat(_musicService, "SpotyPieMusicPlayer", mediaButtonReceiverComponentName, mediaButtonReceiverPendingIntent);


        //    //mediaSession.SetFlags(MediaSession.FLAG_HANDLES_TRANSPORT_CONTROLS
        //    //        | MediaSession.FLAG_HANDLES_MEDIA_BUTTONS);

        //    //mediaSession.setMediaButtonReceiver(mediaButtonReceiverPendingIntent);
        //}

        //public void SongLoadStarted()
        //{
        //    _remoteControlClient.SetPlaybackState(RemoteControlPlayState.Buffering);
        //}

        //public void SongLoaded(Songs song)
        //{
        //    if (_remoteControlClient != null)
        //    {
        //        _remoteControlClient.SetPlaybackState(RemoteControlPlayState.Playing);
        //        UpdateMetadata(song);
        //    }
        //}

        //private void UpdateMetadata(Songs song)
        //{
        //    try
        //    {
        //        MediaMetadata.Builder metadataBuilder = new MediaMetadata.Builder();
        //        metadataBuilder.PutString(MediaMetadata.MetadataKeyAlbum, song.AlbumName);
        //        metadataBuilder.PutString(MediaMetadata.MetadataKeyAlbumArtist, song.ArtistName);
        //        metadataBuilder.PutLong(MediaMetadata.MetadataKeyDiscNumber, song.DiscNumber);
        //        metadataBuilder.PutString(MediaMetadata.MetadataKeyDisplayTitle, song.Name);
        //        metadataBuilder.PutString(MediaMetadata.MetadataKeyTitle, song.Name);

        //        //RestClient client = new RestClient(song.LargeImage);
        //        //RestRequest request = new RestRequest(Method.GET);
        //        //using (MemoryStream str = new MemoryStream(client.DownloadData(request)))
        //        //{
        //        //    if (str.CanRead && str.Length > 0)
        //        //        metadataBuilder.PutBitmap(MediaMetadata.MetadataKeyArt, BitmapFactory.DecodeStream(str));
        //        //}
        //        _mediaSession.SetMetadata(metadataBuilder.Build());

        //        ////metadataBuilder.putBitmap(MediaMetadata.MetadataKeyArt, bitmap);



        //        ////if (_remoteControlClient == null)
        //        ////    return;

        //        ////var metadataEditor = _remoteControlClient.EditMetadata(false);
        //        ////metadataEditor.PutString(MetadataKey.Album, song.AlbumName);
        //        ////metadataEditor.PutString(MetadataKey.Artist, song.ArtistName);
        //        ////metadataEditor.PutString(MetadataKey.Albumartist, $"{song.AlbumName} - {song.ArtistName}");
        //        ////metadataEditor.PutString(MetadataKey.Title, song.Name);


        //        ////metadataEditor.Apply();

        //        ////_mediaSession.SetMetadata(metadataEditor);
        //    }
        //    catch (Exception e)
        //    {
        //        //Task.Run(() => GetAPIService().Report(e));
        //    }
        //}

        //private void SetBthHeadSetButtons()
        //{
        //    try
        //    {
        //        if (_audioManager == null)
        //            _audioManager = (AudioManager)_musicService.GetSystemService("audio");
        //        _remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

        //        if (_remoteControlClient == null)
        //        {
        //            _audioManager.RegisterMediaButtonEventReceiver(_remoteComponentName);
        //            //Create a new pending intent that we want triggered by remote control client
        //            var mediaButtonIntent = new Intent(Intent.ActionMediaButton);
        //            mediaButtonIntent.SetComponent(_remoteComponentName);
        //            // Create new pending intent for the intent
        //            var mediaPendingIntent = PendingIntent.GetBroadcast(_musicService, 0, mediaButtonIntent, 0);
        //            // Create and register the remote control client
        //            _remoteControlClient = new RemoteControlClient(mediaPendingIntent);
        //            _audioManager.RegisterRemoteControlClient(_remoteControlClient);
        //        }
        //        //add transport control flags we can to handle
        //        _remoteControlClient.SetTransportControlFlags(RemoteControlFlags.Play |
        //                                 RemoteControlFlags.Pause |
        //                                 RemoteControlFlags.PlayPause |
        //                                 RemoteControlFlags.Stop |
        //                                 RemoteControlFlags.Previous |
        //                                 RemoteControlFlags.Next);
        //    }
        //    catch (Exception e)
        //    {
        //        //Task.Run(() => GetAPIService().Report(e));
        //    }
        //}

        //private void UnregisterRemoteClient()
        //{
        //    try
        //    {
        //        if (_audioManager != null)
        //        {
        //            _audioManager.UnregisterMediaButtonEventReceiver(_remoteComponentName);
        //            _audioManager.UnregisterRemoteControlClient(_remoteControlClient);
        //            _remoteControlClient.Dispose();
        //            _remoteControlClient = null;
        //            _audioManager = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Task.Run(() => GetAPIService().Report(e));
        //    }
        //}
    }
}