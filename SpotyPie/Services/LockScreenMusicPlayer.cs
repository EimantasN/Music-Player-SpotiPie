using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Media.Session;
using Android.OS;
using Android.Support.V4.Media.Session;
using Mobile_Api.Models;
using RestSharp;
using System;
using System.IO;

namespace SpotyPie.Services
{
    public class LockScreenMusicPlayer
    {
        private AudioManager _audioManager;

        private MusicService _musicService;

        private MediaSession _mediaSession;
        private MediaSessionCustomCallback _mediaSessionCallback;

        private ComponentName _remoteComponentName;

        private RemoteControlClient _remoteControlClient;

        public LockScreenMusicPlayer(MusicService musicService)
        {
            _musicService = musicService;

            _mediaSession = new MediaSession(musicService.ApplicationContext, this.GetType().Name);
            _mediaSessionCallback = new MediaSessionCustomCallback();
            _mediaSession.SetCallback(_mediaSessionCallback);
            _mediaSession.SetFlags(flags: MediaSession.FlagHandlesMediaButtons | MediaSession.FlagHandlesTransportControls);

            if (!_mediaSession.Active)
            {
                _mediaSession.Active = true;
            }

            if ((Build.VERSION.SdkInt >= Build.VERSION_CODES.IceCreamSandwich) && (Build.VERSION.SdkInt < Build.VERSION_CODES.Lollipop))
            {
                // your code using RemoteControlClient API here - is between 14-20
            }
            else if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                // your code using MediaSession API here - is api 21 or higher
            }

            SetBthHeadSetButtons();
        }

        private void SetupMediaSession()
        {
            ComponentName mediaButtonReceiverComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

            Intent mediaButtonIntent = new Intent(Intent.ActionMediaButton);
            mediaButtonIntent.SetComponent(mediaButtonReceiverComponentName);


            PendingIntent mediaButtonReceiverPendingIntent = PendingIntent.GetBroadcast(_musicService.ApplicationContext, 0, mediaButtonIntent, 0);

            var mediaSession = new MediaSessionCompat(_musicService, "SpotyPieMusicPlayer", mediaButtonReceiverComponentName, mediaButtonReceiverPendingIntent);
            //    mediaSession.SetCallback(new MediaSessionCompat.Callback() {
            //@Override
            //public void onPlay()
            //    {
            //        play();
            //    }

            //    @Override
            //public void onPause()
            //    {
            //        pause();
            //    }

            //    @Override
            //public void onSkipToNext()
            //    {
            //        playNextSong(true);
            //    }

            //    @Override
            //public void onSkipToPrevious()
            //    {
            //        back(true);
            //    }

            //    @Override
            //public void onStop()
            //    {
            //        quit();
            //    }

            //    @Override
            //public void onSeekTo(long pos)
            //    {
            //        seek((int)pos);
            //    }

            //    @Override
            //public boolean onMediaButtonEvent(Intent mediaButtonEvent)
            //    {
            //        return MediaButtonIntentReceiver.handleIntent(MusicService.this, mediaButtonEvent);
            //    }
            //});

            //mediaSession.SetFlags(MediaSession.FLAG_HANDLES_TRANSPORT_CONTROLS
            //        | MediaSession.FLAG_HANDLES_MEDIA_BUTTONS);

            //mediaSession.setMediaButtonReceiver(mediaButtonReceiverPendingIntent);
        }

        public void SongLoadStarted()
        {
            _remoteControlClient.SetPlaybackState(RemoteControlPlayState.Buffering);
        }

        public void SongLoaded(Songs song)
        {
            if (_remoteControlClient != null)
            {
                _remoteControlClient.SetPlaybackState(RemoteControlPlayState.Playing);
                UpdateMetadata(song);
            }
        }

        private void UpdateMetadata(Songs song)
        {
            try
            {
                MediaMetadata.Builder metadataBuilder = new MediaMetadata.Builder();
                metadataBuilder.PutString(MediaMetadata.MetadataKeyAlbum, song.AlbumName);
                metadataBuilder.PutString(MediaMetadata.MetadataKeyAlbumArtist, song.ArtistName);
                metadataBuilder.PutLong(MediaMetadata.MetadataKeyDiscNumber, song.DiscNumber);
                metadataBuilder.PutString(MediaMetadata.MetadataKeyDisplayTitle, song.Name);
                metadataBuilder.PutString(MediaMetadata.MetadataKeyTitle, song.Name);

                RestClient client = new RestClient(song.LargeImage);
                RestRequest request = new RestRequest(Method.GET);
                using (MemoryStream str = new MemoryStream(client.DownloadData(request)))
                {
                    if (str.CanRead && str.Length > 0)
                        metadataBuilder.PutBitmap(MediaMetadata.MetadataKeyArt, BitmapFactory.DecodeStream(str));
                }
                _mediaSession.SetMetadata(metadataBuilder.Build());

                ////metadataBuilder.putBitmap(MediaMetadata.MetadataKeyArt, bitmap);



                ////if (_remoteControlClient == null)
                ////    return;

                ////var metadataEditor = _remoteControlClient.EditMetadata(false);
                ////metadataEditor.PutString(MetadataKey.Album, song.AlbumName);
                ////metadataEditor.PutString(MetadataKey.Artist, song.ArtistName);
                ////metadataEditor.PutString(MetadataKey.Albumartist, $"{song.AlbumName} - {song.ArtistName}");
                ////metadataEditor.PutString(MetadataKey.Title, song.Name);


                ////metadataEditor.Apply();

                ////_mediaSession.SetMetadata(metadataEditor);
            }
            catch (Exception e)
            {
                //Task.Run(() => GetAPIService().Report(e));
            }
        }

        private void SetBthHeadSetButtons()
        {
            try
            {
                if (_audioManager == null)
                    _audioManager = (AudioManager)_musicService.GetSystemService("audio");
                _remoteComponentName = new ComponentName(_musicService.PackageName, new MediaButtonBroadcastReceiver().ComponentName);

                if (_remoteControlClient == null)
                {
                    _audioManager.RegisterMediaButtonEventReceiver(_remoteComponentName);
                    //Create a new pending intent that we want triggered by remote control client
                    var mediaButtonIntent = new Intent(Intent.ActionMediaButton);
                    mediaButtonIntent.SetComponent(_remoteComponentName);
                    // Create new pending intent for the intent
                    var mediaPendingIntent = PendingIntent.GetBroadcast(_musicService, 0, mediaButtonIntent, 0);
                    // Create and register the remote control client
                    _remoteControlClient = new RemoteControlClient(mediaPendingIntent);
                    _audioManager.RegisterRemoteControlClient(_remoteControlClient);
                }
                //add transport control flags we can to handle
                _remoteControlClient.SetTransportControlFlags(RemoteControlFlags.Play |
                                         RemoteControlFlags.Pause |
                                         RemoteControlFlags.PlayPause |
                                         RemoteControlFlags.Stop |
                                         RemoteControlFlags.Previous |
                                         RemoteControlFlags.Next);
            }
            catch (Exception e)
            {
                //Task.Run(() => GetAPIService().Report(e));
            }
        }

        private void UnregisterRemoteClient()
        {
            try
            {
                if (_audioManager != null)
                {
                    _audioManager.UnregisterMediaButtonEventReceiver(_remoteComponentName);
                    _audioManager.UnregisterRemoteControlClient(_remoteControlClient);
                    _remoteControlClient.Dispose();
                    _remoteControlClient = null;
                    _audioManager = null;
                }
            }
            catch (Exception e)
            {
                //Task.Run(() => GetAPIService().Report(e));
            }
        }
    }
}