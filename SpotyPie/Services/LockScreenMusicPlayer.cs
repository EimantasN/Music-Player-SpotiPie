using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;
using RestSharp;
using System;
using System.Threading;
using static Android.Support.V4.Media.App.NotificationCompat;

namespace SpotyPie.Services
{
    public class LockScreenMusicPlayer
    {
        public const string ActionPlay = "com.xamarin.action.PLAY";
        public const string ActionPause = "com.xamarin.action.PAUSE";
        public const string ActionStop = "com.xamarin.action.STOP";
        public const string ActionTogglePlayback = "com.xamarin.action.TOGGLEPLAYBACK";
        public const string ActionNext = "com.xamarin.action.NEXT";
        public const string ActionPrevious = "com.xamarin.action.PREVIOUS";

        private MusicService _musicService;
        private MediaSessionCompat mediaSessionCompat;
        public MediaControllerCompat mediaControllerCompat;
        private ComponentName remoteComponentName;

        private const int NotificationId = 1;

        public int MediaPlayerState
        {
            get
            {
                return (mediaControllerCompat?.PlaybackState != null ?
                    mediaControllerCompat.PlaybackState.State :
                    PlaybackStateCompat.StateNone);
            }
        }

        public LockScreenMusicPlayer(MusicService musicService)
        {
            _musicService = musicService;
            remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);
        }

        public void SetStateBuffering()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                UpdatePlaybackState(PlaybackStateCompat.StateBuffering);
            }, null);
        }

        public void SetStateStopped()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                UpdatePlaybackState(PlaybackStateCompat.StateStopped);
            }, null);
        }

        public void SetStatePlaying()
        {
            Application.SynchronizationContext.Post(_ =>
            {
                UpdatePlaybackState(PlaybackStateCompat.StatePlaying);
                Play();
            }, null);
        }

        private void Play()
        {
            try
            {
                if (mediaSessionCompat == null)
                    InitMediaSession();

                UpdateMediaMetadataCompat();
                StartNotification();
            }
            catch (Exception e)
            {

            }
        }

        private void UpdatePlaybackState(int state)
        {
            //while (_musicService == null)
            //    Thread.Sleep(250);

            if (mediaSessionCompat == null)
                InitMediaSession();

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
                    .SetState(state, 0, 1.0f, SystemClock.ElapsedRealtime());

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

                if (state == PlaybackStateCompat.StatePlaying || state == PlaybackStateCompat.StatePaused)
                {
                    //StartNotification();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void InitMediaSession()
        {
            try
            {
                if (mediaSessionCompat == null && _musicService != null)
                {
                    Intent nIntent = new Intent(_musicService.ApplicationContext, typeof(MainActivity));
                    PendingIntent pIntent = PendingIntent.GetActivity(_musicService.ApplicationContext, 0, nIntent, 0);

                    remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

                    mediaSessionCompat = new MediaSessionCompat(_musicService.ApplicationContext, "SpotyPie", remoteComponentName, pIntent);
                    mediaControllerCompat = new MediaControllerCompat(_musicService.ApplicationContext, mediaSessionCompat.SessionToken);
                }

                mediaSessionCompat.Active = true;
                mediaSessionCompat.SetCallback(new MediaSessionCustomCallback(_musicService));

                mediaSessionCompat.SetFlags(MediaSessionCompat.FlagHandlesMediaButtons | MediaSessionCompat.FlagHandlesTransportControls);
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

                NotificationCompat.Builder builder = new NotificationCompat.Builder(_musicService.ApplicationContext);

                // if (Android.OS.Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
                // {
                //     builder.SetSmallIcon(Re.drawable.icon_transperent);
                //    builder.SetColor(Resources.GetColor(Resource.Color.notification_color));
                // }
                //else
                //{
                //builder.SetSmallIcon(Resource.Drawable.logo_spotify);
                //}

                builder.SetSmallIcon(Resource.Drawable.logo_spotify);

                builder.SetStyle(style);
                builder.SetContentTitle(currentTrack.GetString(MediaMetadata.MetadataKeyTitle));
                builder.SetContentText(currentTrack.GetString(MediaMetadata.MetadataKeyArtist));
                builder.SetContentInfo(currentTrack.GetString(MediaMetadata.MetadataKeyAlbum));
                builder.SetContentIntent(pendingIntent);
                var image = GetLargeImage();
                if(image != null)
                    builder.SetLargeIcon(image);
                builder.SetColorized(true);
                builder.SetShowWhen(false);
                builder.SetOngoing(MediaPlayerState == PlaybackStateCompat.StatePlaying);
                builder.SetVisibility(NotificationCompat.VisibilityPublic);

                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaPrevious, "Previous", ActionPrevious));
                AddPlayPauseActionCompat(builder);
                builder.AddAction(GenerateActionCompat(Android.Resource.Drawable.IcMediaNext, "Next", ActionNext));
                style.SetShowActionsInCompactView(0, 1, 2);

                NotificationManagerCompat.From(_musicService.ApplicationContext).Notify(NotificationId, builder.Build());
            }
            catch (Exception e)
            {

            }
        }

        public Bitmap GetLargeImage()
        {
            try
            {
                RestClient client = new RestClient(_musicService.Current_Song.LargeImage);
                RestRequest request = new RestRequest(Method.GET);
                byte[] image = client.DownloadData(request);
                return BitmapFactory.DecodeByteArray(image, 0, image.Length);
            }
            catch (Exception e)
            {
                return null;
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

        private void UpdateMediaMetadataCompat()
        {
            try
            {
                if (mediaSessionCompat == null)
                    return;

                MediaMetadataCompat.Builder builder = new MediaMetadataCompat.Builder();

                if (_musicService?.Current_Song != null)
                {
                    builder
                    .PutString(MediaMetadata.MetadataKeyAlbum, _musicService.Current_Song.AlbumName)
                    .PutString(MediaMetadata.MetadataKeyArtist, _musicService.Current_Song.ArtistName)
                    .PutString(MediaMetadata.MetadataKeyTitle, _musicService.Current_Song.Name);
                }
                else
                {
                    builder
                        .PutString(MediaMetadata.MetadataKeyAlbum, "Failed")
                        .PutString(MediaMetadata.MetadataKeyArtist, "Failed")
                        .PutString(MediaMetadata.MetadataKeyTitle, "Failed");
                }
                mediaSessionCompat.SetMetadata(builder.Build());
            }
            catch (Exception e)
            {

            }
        }

        public void ControlInput(string action = "")
        {
            HandleIntent(action);
        }

        private void HandleIntent(string actionCustom)
        {
            if (actionCustom == "Play")
            {
                mediaControllerCompat.GetTransportControls().Play();
            }
            else if (actionCustom == "Pause")
            {
                mediaControllerCompat.GetTransportControls().Pause();
            }
            else if (actionCustom == "SkipToPrevious")
            {
                mediaControllerCompat.GetTransportControls().SkipToPrevious();
            }
            else if (actionCustom == "SkipToNext")
            {
                mediaControllerCompat.GetTransportControls().SkipToNext();
            }
            else if (actionCustom == "Stop")
            {
                mediaControllerCompat.GetTransportControls().Stop();
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
    }
}