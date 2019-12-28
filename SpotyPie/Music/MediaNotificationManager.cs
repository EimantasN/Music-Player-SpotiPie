using System;
using Android.Content;
using Android.App;
using Android.Graphics;
using SpotyPie.Music.Helpers;
using Android.OS;
using Android.Support.V4.Media.Session;
using Android.Support.V4.Media;
using Android.Support.V4.App;
using NotificationCompatMusic = Android.Support.V4.Media.App.NotificationCompat;
using RestSharp;
using SpotyPie.Music.Manager;
using Mobile_Api.Models;
using System.Threading.Tasks;

namespace SpotyPie.Music
{
    public class MediaNotificationManager : BroadcastReceiver
    {
        private readonly RestClient _client = new RestClient();

        private readonly RestRequest request = new RestRequest(Method.GET);

        private string LastImgUrl { get; set; }

        private Object NotificationLock { get; set; } = new object();

        public int CountSkip { get; set; } = 0;

        const int NotificationId = 352619;
        const int RequestCode = 100;

        public const string ActionPause = "com.spotypie.adnroid.musicservice.pause";

        public const string ActionPlay = "com.spotypie.adnroid.musicservice.play";

        public const string ActionPrev = "com.spotypie.adnroid.musicservice.prev";

        public const string ActionNext = "com.spotypie.adnroid.musicservice.next";

        public const string ActionFavorite = "com.spotypie.adnroid.musicservice.favorite";

        public const string ActionTrash = "com.spotypie.adnroid.musicservice.trash";

        public const string ActionSeek = "com.spotypie.adnroid.musicservice.seek";

        readonly MusicService Service;

        private MediaSessionCompat.Token SessionToken;

        private MediaControllerCompat SPController;

        private MediaControllerCompat.TransportControls transportControls;

        private SpotyPieMediaControllerCallback SPMediaControllerCallback;

        private PlaybackStateCompat playbackState;

        public MediaMetadataCompat Metadata;

        readonly NotificationManagerCompat NotificationManager;

        private PendingIntent PauseIntent { get; set; }

        private PendingIntent PlayIntent { get; set; }

        private PendingIntent PreviousIntent { get; set; }

        private PendingIntent NextIntent { get; set; }

        private PendingIntent FavoriteIntent { get; set; }

        private PendingIntent TrashIntent { get; set; }

        private PendingIntent SeekIntent { get; set; }

        int notificationColor;

        bool started;

        public MediaNotificationManager(MusicService serv)
        {
            Service = serv;
            SPMediaControllerCallback = new SpotyPieMediaControllerCallback();

            UpdateSessionToken();

            notificationColor = ResourceHelper.GetThemeColor(Service, Android.Resource.Attribute.ColorPrimary, Color.DarkGray);

            NotificationManager = NotificationManagerCompat.From(this.Service);

            CreateNotificationChannel();

            string pkg = Service.PackageName;
            PauseIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionPause).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            PlayIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionPlay).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            PreviousIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionPrev).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            NextIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionNext).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            FavoriteIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionFavorite).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            TrashIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionTrash).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            SeekIntent = PendingIntent.GetBroadcast(Service, RequestCode, new Intent(ActionSeek).SetPackage(pkg), PendingIntentFlags.CancelCurrent);

            NotificationManager.CancelAll();

            SPMediaControllerCallback.OnPlaybackStateChangedImpl = (state) =>
            {
                playbackState = state;
                var notification = CreateNotification(SongManager.Song);
                if (notification != null)
                {
                    NotificationManager.Notify(NotificationId, notification);
                }
            };

            SPMediaControllerCallback.OnMetadataChangedImpl = (meta) =>
            {
            };

            SPMediaControllerCallback.OnSessionDestroyedImpl = () =>
            {
            };

            SongManager.SongHandler += OnSongChange;
        }

        private void OnSongChange(Songs song)
        {
            var notification = CreateNotification(song);
            if (notification != null)
            {
                NotificationManager.Notify(NotificationId, notification);
            }
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            string channelName = Service.Resources.GetString(Resource.String.channelName);
            string channelDescription = Service.Resources.GetString(Resource.String.channelDescription);
            string ChannelId = Service.Resources.GetString(Resource.String.ChannelId);

            NotificationChannel mChannel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Default);
            mChannel.Description = channelDescription;
            mChannel.EnableLights(true);
            mChannel.LightColor = Color.Red;
            mChannel.EnableVibration(true);
            mChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
            mChannel.SetShowBadge(false);
            mChannel.SetSound(null, null);
            var manager = Service.GetSystemService(Context.NotificationService) as NotificationManager;
            manager.CreateNotificationChannel(mChannel);
        }

        public void StartNotification()
        {
            SPController.RegisterCallback(SPMediaControllerCallback);
            if (!started)
            {
                started = true;
                playbackState = SPController.PlaybackState;

                var filter = new IntentFilter();
                filter.AddAction(ActionNext);
                filter.AddAction(ActionPause);
                filter.AddAction(ActionPlay);
                filter.AddAction(ActionPrev);
                filter.AddAction(ActionFavorite);
                filter.AddAction(ActionTrash);
                filter.AddAction(ActionSeek);
                Service.RegisterReceiver(this, filter);
            }
            else
            {
                playbackState = SPController.PlaybackState;
            }
        }

        public void StopNotification()
        {
            if (started)
            {
                started = false;
                SPController.UnregisterCallback(SPMediaControllerCallback);
                try
                {
                    NotificationManager.Cancel(NotificationId);
                    Service.UnregisterReceiver(this);
                }
                catch (ArgumentException)
                {
                }
                Service.StopForeground(true);
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            switch (action)
            {
                case ActionPause:
                    transportControls?.Pause();
                    break;
                case ActionPlay:
                    transportControls?.Play();
                    break;
                case ActionNext:
                    transportControls?.SkipToNext();
                    break;
                case ActionPrev:
                    transportControls?.SkipToPrevious();
                    break;
                case ActionFavorite:
                    transportControls?.SendCustomAction(ActionFavorite, null);
                    break;
                case ActionTrash:
                    transportControls?.SendCustomAction(ActionTrash, null);
                    break;
                case ActionSeek:
                    {
                        var data = intent.GetStringExtra("PLAYER_SEEK");
                        if (!string.IsNullOrEmpty(data))
                        {
                            transportControls?.SeekTo(int.Parse(data));
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        void UpdateSessionToken(bool ignore = false)
        {
            var freshToken = Service.SessionToken;
            if (SessionToken == null || SessionToken != freshToken)
            {
                SPController?.UnregisterCallback(SPMediaControllerCallback);

                SessionToken = freshToken;
                SPController = new MediaControllerCompat(Service, SessionToken);
                SPController.RegisterCallback(SPMediaControllerCallback);
                transportControls = SPController.GetTransportControls();
            }
        }

        private PendingIntent _pendingIntent;
        PendingIntent CreateContentIntent()
        {
            if (_pendingIntent == null)
            {
                var openUI = new Intent(Service, typeof(MainActivity));

                openUI.SetFlags(ActivityFlags.SingleTop);

                _pendingIntent = PendingIntent.GetActivity(Service, RequestCode, openUI, PendingIntentFlags.CancelCurrent);
            }
            return _pendingIntent;
        }

        private NotificationCompat.Builder NotificationBuilder;

        //public Android.Support.V4.App.NotificationCompat.Builder GetNotificationBuilder()
        //{
        //    //if (NotificationBuilder == null)
        //    //{
        //        NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(service, service.Resources.GetString(Resource.String.ChannelId));
        //        SetupNotificationButtons(NotificationBuilder);
        //    //}

        //    return NotificationBuilder;
        //}

        private NotificationCompatMusic.MediaStyle SpotyPieMediaStyle;
        public NotificationCompatMusic.MediaStyle GetMediaStyle()
        {
            if (SpotyPieMediaStyle == null)
            {
                SpotyPieMediaStyle = new NotificationCompatMusic.MediaStyle();
                SpotyPieMediaStyle.SetMediaSession(SessionToken);
                SpotyPieMediaStyle.SetShowActionsInCompactView(new[] { 3 });
                Intent intent = new Intent(Service.ApplicationContext, typeof(MusicService));
                intent.SetAction("spotypie_stop");
                PendingIntent pendingCancelIntent = PendingIntent.GetService(Service.ApplicationContext, 1, intent, PendingIntentFlags.UpdateCurrent);

                SpotyPieMediaStyle.SetShowCancelButton(true);
                SpotyPieMediaStyle.SetCancelButtonIntent(pendingCancelIntent);
            }
            return SpotyPieMediaStyle;
        }

        Notification CreateNotification(Songs song)
        {
            lock (NotificationLock)
            {
                UpdateMetadata(song);

                if (Metadata == null || playbackState == null)
                {
                    return null;
                }

                NotificationBuilder = new NotificationCompat.Builder(Service, Service.Resources.GetString(Resource.String.ChannelId));
                SetupNotificationButtons(NotificationBuilder);

                NotificationBuilder
                    .SetStyle(GetMediaStyle())
                    .SetPriority(1)
                    .SetColor(notificationColor)
                    .SetSmallIcon(Resource.Drawable.logo_spotify)
                    .SetVisibility(NotificationCompat.VisibilityPublic)
                    .SetUsesChronometer(true)
                    .SetSound(null)
                    .SetDefaults(0)
                    .SetContentIntent(CreateContentIntent())
                    .SetContentTitle(Metadata.Description.Title)
                    .SetContentText(Metadata.Description.Subtitle);

                LoadLargeImage();

                SetNotificationPlaybackState(NotificationBuilder);

                return NotificationBuilder.Build();
            }
        }

        private void LoadLargeImage()
        {
            var id = Metadata.Description.MediaId;
            if (LastImgUrl == null || LastImgUrl != Metadata.GetString("SpotyPieImgUrl"))
            {
                LastImgUrl = Metadata.GetString("SpotyPieImgUrl");
                Task.Run(() =>
                {
                    GetLargeImage(NotificationBuilder);
                    if (id == Metadata.Description.MediaId && NotificationBuilder != null)
                    {
                        Android.App.NotificationManager.FromContext(Service.ApplicationContext).Notify(NotificationId, NotificationBuilder.Build());
                    }
                });
            }
            else if (LastImg != null)
            {
                NotificationBuilder.SetLargeIcon(LastImg);
                if (id == Metadata.Description.MediaId && NotificationBuilder != null)
                {
                    Android.App.NotificationManager.FromContext(Service.ApplicationContext).Notify(NotificationId, NotificationBuilder.Build());
                }
            }
        }

        private void UpdateMetadata(Songs song)
        {
            var currentSong = song;
            if (currentSong == null)
            {
                Metadata = null;
            }
            else if (Metadata == null || Metadata.Description.MediaId != currentSong.Id.ToString())
            {
                Metadata = new MediaMetadataCompat.Builder()
                    .PutString(MediaMetadataCompat.MetadataKeyMediaId, currentSong.Id.ToString())
                    .PutString(MediaMetadataCompat.MetadataKeyAlbum, currentSong.AlbumName)
                    .PutString(MediaMetadataCompat.MetadataKeyArtist, currentSong.ArtistName)
                    .PutLong(MediaMetadataCompat.MetadataKeyDuration, currentSong.DurationMs)
                    .PutString(MediaMetadataCompat.MetadataKeyGenre, "Rock")
                    .PutString(MediaMetadataCompat.MetadataKeyAlbumArtUri, currentSong.MediumImage)
                    .PutString(MediaMetadataCompat.MetadataKeyTitle, currentSong.Name)
                    .PutLong(MediaMetadataCompat.MetadataKeyTrackNumber, currentSong.TrackNumber)
                    .PutLong(MediaMetadataCompat.MetadataKeyNumTracks, 10)
                    .PutLong(MediaMetadataCompat.MetadataKeyDiscNumber, currentSong.DiscNumber)
                    .PutString("SpotyPieImgUrl", currentSong.MediumImage)
                    .Build();
            }
        }

        private Bitmap LastImg;
        public void GetLargeImage(NotificationCompat.Builder builder)
        {
            _client.BaseUrl = new Uri(Metadata.GetString("SpotyPieImgUrl"));
            byte[] image = _client.DownloadData(request, false);
            if (image.Length > 1000)
            {
                LastImg = BitmapFactory.DecodeByteArray(image, 0, image.Length);
                builder.SetLargeIcon(LastImg);
            }
        }

        void SetupNotificationButtons(NotificationCompat.Builder builder)
        {
            //FAVORITE BUTTON
            builder.AddAction(new NotificationCompat.Action(Resource.Drawable.baseline_favorite_black_18, "Favorite", FavoriteIntent));

            //REVIOUS SONG BUTTON
            builder.AddAction(Resource.Drawable.baseline_skip_previous_black_24, "Previuos Song", PreviousIntent);

            //PLAY OR PAUSE BUTTON
            if (playbackState.State == PlaybackStateCompat.StatePlaying)
            {
                builder.AddAction(new NotificationCompat.Action(Resource.Drawable.baseline_pause_black_36, "Pause", PauseIntent));
            }
            else if (playbackState.State == PlaybackStateCompat.StatePaused)
            {
                builder.AddAction(new NotificationCompat.Action(Resource.Drawable.baseline_play_arrow_black_36, "Play", PlayIntent));
            }
            else
            {
                builder.AddAction(new NotificationCompat.Action(Resource.Drawable.baseline_loop_black_18, "Loading", null));
            }

            //NEXT SONG BUTTON
            builder.AddAction(Resource.Drawable.baseline_skip_next_black_24, "Next Song", NextIntent);

            //DELETE
            builder.AddAction(new NotificationCompat.Action(Resource.Drawable.baseline_delete_forever_black_18, "To Trash", TrashIntent));
        }

        void SetNotificationPlaybackState(NotificationCompat.Builder builder)
        {
            var beginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (playbackState == null || !started)
            {
                Service.StopForeground(true);
                return;
            }
            if (playbackState.State == PlaybackStateCompat.StatePlaying && playbackState.Position >= 0)
            {
                var timespan = ((long)(DateTime.UtcNow - beginningOfTime).TotalMilliseconds) - playbackState.Position;
                builder.SetWhen(timespan).SetShowWhen(true).SetUsesChronometer(true);
            }
            else
            {
                builder.SetWhen(0).SetShowWhen(false).SetUsesChronometer(false);
            }
            builder.SetOngoing(playbackState.State == PlaybackStateCompat.StatePlaying);
        }
    }
}