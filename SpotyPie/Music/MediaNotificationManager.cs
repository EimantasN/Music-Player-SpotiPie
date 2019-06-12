using System;
using Android.Content;
using Android.App;
using Android.Graphics;
using SpotyPie.Music.Helpers;
using Android.OS;
using RestSharp;
using Android.Support.V4.Media.App;
using Android.Support.V4.Media.Session;
using Android.Support.V4.Media;
using System.Threading.Tasks;

namespace SpotyPie.Music
{
    public class MediaNotificationManager : BroadcastReceiver
    {
        static readonly string Tag = LogHelper.MakeLogTag(typeof(MediaNotificationManager));

        private string LastImgUrl { get; set; }

        private Object NotificationLock { get; set; } = new object();

        public int CountSkip { get; set; } = 0;

        const int NotificationId = 1;
        const int RequestCode = 100;

        public const string ActionPause = "com.spotypie.adnroid.musicservice.pause";
        public const string ActionPlay = "com.spotypie.adnroid.musicservice.play";
        public const string ActionPrev = "com.spotypie.adnroid.musicservice.prev";
        public const string ActionNext = "com.spotypie.adnroid.musicservice.next";
        public const string ActionFavorite = "com.spotypie.adnroid.musicservice.favorite";
        public const string ActionTrash = "com.spotypie.adnroid.musicservice.trash";

        readonly MusicService service;
        MediaSessionCompat.Token sessionToken;
        MediaControllerCompat controller;
        MediaControllerCompat.TransportControls transportControls;

        PlaybackStateCompat playbackState;
        public MediaMetadataCompat metadata;

        readonly Android.Support.V4.App.NotificationManagerCompat notificationManager;

        private PendingIntent PauseIntent { get; set; }
        private PendingIntent PlayIntent { get; set; }
        private PendingIntent PreviousIntent { get; set; }
        private PendingIntent NextIntent { get; set; }
        private PendingIntent FavoriteIntent { get; set; }
        private PendingIntent TrashIntent { get; set; }

        MediaController mCb = new MediaController();

        int notificationColor;

        bool started;

        public MediaNotificationManager(MusicService serv)
        {
            service = serv;
            UpdateSessionToken();

            notificationColor = ResourceHelper.GetThemeColor(service, Android.Resource.Attribute.ColorPrimary, Color.DarkGray);

            notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(this.service);

            CreateNotificationChannel();

            string pkg = service.PackageName;
            PauseIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionPause).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            PlayIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionPlay).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            PreviousIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionPrev).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            NextIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionNext).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            FavoriteIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionFavorite).SetPackage(pkg), PendingIntentFlags.CancelCurrent);
            TrashIntent = PendingIntent.GetBroadcast(service, RequestCode, new Intent(ActionTrash).SetPackage(pkg), PendingIntentFlags.CancelCurrent);

            notificationManager.CancelAll();

            mCb.OnPlaybackStateChangedImpl = (state) =>
            {
                playbackState = state;
                var notification = CreateNotification();
                if (notification != null)
                {
                    //NotificationManager.FromContext(service.ApplicationContext).Notify(NotificationId, notification);
                    notificationManager.Notify(NotificationId, notification);
                }
            };

            mCb.OnMetadataChangedImpl = (meta) =>
            {
                metadata = meta;
                var notification = CreateNotification();
                if (notification != null)
                {
                    notificationManager.Notify(NotificationId, notification);
                }
            };

            mCb.OnSessionDestroyedImpl = () =>
            {
                UpdateSessionToken();
            };
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            string channelName = service.Resources.GetString(Resource.String.channelName);
            string channelDescription = service.Resources.GetString(Resource.String.channelDescription);
            string ChannelId = service.Resources.GetString(Resource.String.ChannelId);

            NotificationChannel mChannel = new NotificationChannel(ChannelId, channelName, NotificationImportance.Max);
            mChannel.Description = channelDescription;
            mChannel.EnableLights(true);
            mChannel.LightColor = Color.Red;
            mChannel.EnableVibration(true);
            mChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
            mChannel.SetShowBadge(false);
            mChannel.SetSound(null, null);
            var manager = service.GetSystemService(Context.NotificationService) as NotificationManager;
            manager.CreateNotificationChannel(mChannel);
        }

        public void StartNotification()
        {
            if (!started)
            {
                started = true;
                playbackState = controller.PlaybackState;

                controller.RegisterCallback(mCb);
                var filter = new IntentFilter();
                filter.AddAction(ActionNext);
                filter.AddAction(ActionPause);
                filter.AddAction(ActionPlay);
                filter.AddAction(ActionPrev);
                filter.AddAction(ActionFavorite);
                filter.AddAction(ActionTrash);
                service.RegisterReceiver(this, filter);
                Notification notification = CreateNotification();
                if (notification != null)
                {
                    NotificationManager.FromContext(service.ApplicationContext).Notify(NotificationId, notification);
                }
            }
            else
            {
                //metadata = controller.Metadata;
                Notification notification = CreateNotification();
                if (notification != null)
                {
                    NotificationManager.FromContext(service.ApplicationContext).Notify(NotificationId, notification);
                }
                playbackState = controller.PlaybackState;
            }
        }

        public void StopNotification()
        {
            if (started)
            {
                started = false;
                controller.UnregisterCallback(mCb);
                try
                {
                    notificationManager.Cancel(NotificationId);
                    service.UnregisterReceiver(this);
                }
                catch (ArgumentException)
                {
                }
                service.StopForeground(true);
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
                default:
                    break;
            }
        }

        void UpdateSessionToken()
        {
            var freshToken = service.SessionToken;
            if (sessionToken == null || sessionToken != freshToken)
            {
                if (controller != null)
                {
                    controller.UnregisterCallback(mCb);
                }
                sessionToken = freshToken;
                controller = new MediaControllerCompat(service, sessionToken);
                transportControls = controller.GetTransportControls();
                if (started)
                {
                    controller.RegisterCallback(mCb);
                }
            }
        }

        PendingIntent CreateContentIntent()
        {
            var openUI = new Intent(service, typeof(MainActivity));

            openUI.SetFlags(ActivityFlags.SingleTop);

            return PendingIntent.GetActivity(service, RequestCode, openUI, PendingIntentFlags.CancelCurrent);
        }

        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder;

        //public Android.Support.V4.App.NotificationCompat.Builder GetNotificationBuilder()
        //{
        //    //if (NotificationBuilder == null)
        //    //{
        //        NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(service, service.Resources.GetString(Resource.String.ChannelId));
        //        SetupNotificationButtons(NotificationBuilder);
        //    //}

        //    return NotificationBuilder;
        //}

        private NotificationCompat.MediaStyle SpotyPieMediaStyle;

        public NotificationCompat.MediaStyle GetMediaStyle()
        {
            if (SpotyPieMediaStyle == null)
            {
                SpotyPieMediaStyle = new NotificationCompat.MediaStyle();
                SpotyPieMediaStyle.SetMediaSession(sessionToken);
                SpotyPieMediaStyle.SetShowActionsInCompactView(new[] { 3 });
                Intent intent = new Intent(service.ApplicationContext, typeof(MusicService));
                intent.SetAction("spotypie_stop");
                PendingIntent pendingCancelIntent = PendingIntent.GetService(service.ApplicationContext, 1, intent, PendingIntentFlags.CancelCurrent);

                SpotyPieMediaStyle.SetShowCancelButton(true);
                SpotyPieMediaStyle.SetCancelButtonIntent(pendingCancelIntent);
            }
            return SpotyPieMediaStyle;
        }

        Notification CreateNotification()
        {
            lock (NotificationLock)
            {
                if (metadata == null || playbackState == null)
                {
                    return null;
                }
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(service, service.Resources.GetString(Resource.String.ChannelId));
                SetupNotificationButtons(NotificationBuilder);

                NotificationBuilder
                    .SetStyle(GetMediaStyle())
                    .SetColor(notificationColor)
                    .SetSmallIcon(Resource.Drawable.logo_spotify)
                    .SetVisibility(Android.Support.V4.App.NotificationCompat.VisibilityPublic)
                    .SetUsesChronometer(true)
                    .SetSound(null)
                    .SetDefaults(0)
                    .SetContentIntent(CreateContentIntent())
                    .SetContentTitle(metadata.Description.Title)
                    .SetContentText(metadata.Description.Subtitle);

                Task.Run(() =>
                {
                    if (LastImgUrl == null || LastImgUrl != metadata.GetString("SpotyPieImgUrl"))
                    {
                        var id = metadata.Description.MediaId;
                        GetLargeImage(NotificationBuilder);
                        if (id == metadata.Description.MediaId && NotificationBuilder != null)
                        {
                            NotificationManager.FromContext(service.ApplicationContext).Notify(NotificationId, NotificationBuilder.Build());
                        }
                    }
                });

                SetNotificationPlaybackState(NotificationBuilder);

                return NotificationBuilder.Build();
            }
        }

        public void GetLargeImage(Android.Support.V4.App.NotificationCompat.Builder builder)
        {
            RestClient client = new RestClient(metadata.GetString("SpotyPieImgUrl"));
            RestRequest request = new RestRequest(Method.GET);
            byte[] image = client.DownloadData(request);
            if (image.Length > 1000)
            {
                builder.SetLargeIcon(BitmapFactory.DecodeByteArray(image, 0, image.Length));
            }
        }

        void SetupNotificationButtons(Android.Support.V4.App.NotificationCompat.Builder builder)
        {
            //FAVORITE BUTTON
            builder.AddAction(new Android.Support.V4.App.NotificationCompat.Action(Resource.Drawable.baseline_favorite_black_18, "Favorite", FavoriteIntent));

            //REVIOUS SONG BUTTON
            builder.AddAction(Resource.Drawable.baseline_skip_previous_black_24, "Previuos Song", PreviousIntent);

            //PLAY OR PAUSE BUTTON
            if (playbackState.State == PlaybackStateCompat.StatePlaying)
            {
                builder.AddAction(new Android.Support.V4.App.NotificationCompat.Action(Resource.Drawable.baseline_pause_black_36, "Pause", PauseIntent));
            }
            else
            {
                builder.AddAction(new Android.Support.V4.App.NotificationCompat.Action(Resource.Drawable.baseline_play_arrow_black_36, "Play", PlayIntent));
            }

            //NEXT SONG BUTTON
            builder.AddAction(Resource.Drawable.baseline_skip_next_black_24, "Next Song", NextIntent);

            //DELETE
            builder.AddAction(new Android.Support.V4.App.NotificationCompat.Action(Resource.Drawable.baseline_delete_forever_black_18, "To Trash", TrashIntent));
        }

        void SetNotificationPlaybackState(Android.Support.V4.App.NotificationCompat.Builder builder)
        {
            var beginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            if (playbackState == null || !started)
            {
                service.StopForeground(true);
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