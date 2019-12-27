using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Media.Browse;
using Android.OS;
using Android.Runtime;
using SpotyPie.Music.Helpers;
using SpotyPie.Music.Models;
using Android.Support.V4.Media;
using MediaSessionCompat = Android.Support.V4.Media.Session.MediaSessionCompat;
using Android.Widget;

namespace SpotyPie.Music
{
    [Service(Exported = true)]
    public class MusicService : MediaBrowserServiceCompat
    {
        private int currentIndexOnQueue;
        private bool serviceStarted;

        MediaSessionCompat session;

        List<MediaSessionCompat.QueueItem> PlayingQueue;

        MusicProvider musicProvider;

        public Playback playback;

        MediaNotificationManager mediaNotificationManager;

        PackageValidator packageValidator;

        MediaSessionCallback mediaCallback;

        public bool ServiceCreated { get; set; } = false;

        public IBinder Binder { get; private set; }

        public MusicService GetMusicService() { return this; }

        public override IBinder OnBind(Intent intent)
        {
            this.Binder = new MusicServiceBinder(this);
            return this.Binder;
        }

        public MusicService() { }

        internal void SetServiceIsStopped() { serviceStarted = false; }

        public override void OnCreate()
        {
            base.OnCreate();

            PlayingQueue = new List<MediaSessionCompat.QueueItem>();
            musicProvider = new MusicProvider();
            packageValidator = new PackageValidator(this);

            session = new MediaSessionCompat(this, DateTime.Now.Ticks.ToString());
            SessionToken = session.SessionToken;
            mediaCallback = new MediaSessionCallback();

            //MUSIC PLAYER PLAY ACTION
            mediaCallback.OnPlayImpl = () =>
            {
                HandlePlayRequest();
            };

            //MUSIC PLAYER SKIP TO QUEUE ITEM ACTION
            mediaCallback.OnSkipToQueueItemImpl = (id) =>
            {
                if (PlayingQueue != null && PlayingQueue.Count != 0)
                {
                    currentIndexOnQueue = QueueHelper.GetMusicIndexOnQueue(PlayingQueue, id.ToString());
                    HandlePlayRequest();
                }
            };

            //MUSIC PLAYER SEEEK TO ACTION
            mediaCallback.OnSeekToImpl = (pos) =>
            {
                playback.SeekTo((int)pos);
            };

            //MUSIC PLAYER PLAY FROM MEDIA ID ACTION
            mediaCallback.OnPlayFromMediaIdImpl = (mediaId, extras) =>
            {
                //Toast.MakeText(ApplicationContext, "OnPlayFromMediaIdImpl", ToastLength.Long).Show();
                return;
            };

            //MUSIC PLAYER PAUSE ACTION
            mediaCallback.OnPauseImpl = () =>
            {
                //Toast.MakeText(ApplicationContext, "OnPauseImpl", ToastLength.Long).Show();
                HandlePauseRequest();
            };

            //MUSIC PLAYER STOP ACTION
            mediaCallback.OnStopImpl = () =>
            {
                //Toast.MakeText(ApplicationContext, "OnStopImpl", ToastLength.Long).Show();
                HandleStopRequest(null);
            };

            //MUSIC PLAYER SKIP TO NEXT ACTION
            mediaCallback.OnSkipToNextImpl = () =>
            {
                //Toast.MakeText(ApplicationContext, "OnSkipToNextImpl", ToastLength.Long).Show();
                OnNextSong();
                return;
            };

            //MUSIC PLAYER SKIP TO PREVIUOS ACTION
            mediaCallback.OnSkipToPreviousImpl = () =>
            {
                //Toast.MakeText(ApplicationContext, "OnSkipToPreviousImpl", ToastLength.Long).Show();
                mediaNotificationManager.CountSkip--;
                playback.Skip(false);
                return;
            };

            //MUSIC PLAYER CUSTOM ACTION
            mediaCallback.OnCustomActionImpl = (action, extras) =>
            {
                Toast.MakeText(ApplicationContext, $"Unsuported action {action}", ToastLength.Short).Show();
            };

            //MUSIC PLAYER PLAYSEARCH ACTION
            mediaCallback.OnPlayFromSearchImpl = (query, extras) =>
            {
                if (string.IsNullOrEmpty(query))
                {
                    PlayingQueue = new List<MediaSessionCompat.QueueItem>(QueueHelper.GetRandomQueue(musicProvider));
                }
                else
                {
                    PlayingQueue = new List<MediaSessionCompat.QueueItem>(QueueHelper.GetPlayingQueueFromSearch(query, musicProvider));
                }

                session.SetQueue(PlayingQueue);

                if (PlayingQueue != null && PlayingQueue.Count != 0)
                {
                    currentIndexOnQueue = 0;

                    HandlePlayRequest();
                }
                else
                {
                    HandleStopRequest("No search results");
                }
            };

            session.SetCallback(mediaCallback);

            //TODO FIX
            session.SetFlags(1 | 2);

            playback = new Playback(this, session);

            var intent = new Intent(ApplicationContext, typeof(MainActivity));
            var pi = PendingIntent.GetActivity(ApplicationContext, 99 /*request code*/, intent, PendingIntentFlags.UpdateCurrent);
            session.SetSessionActivity(pi);

            var extraBundle = new Bundle();
            CarHelper.SetSlotReservationFlags(extraBundle, true, true, true);
            session.SetExtras(extraBundle);

            mediaNotificationManager = new MediaNotificationManager(this);

            new Handler().PostDelayed(() =>
            {
                mediaNotificationManager.StartNotification();
            }, 1000);

            ServiceCreated = true;
        }

        private void OnNextSong()
        {
            mediaNotificationManager.CountSkip++;
            playback.Skip(true);
        }

        [Obsolete("deprecated")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent != null)
            {
                var action = intent.Action;
                var command = intent.GetStringExtra(Playback.CmdName);
                if (Playback.ActionCmd == action)
                {
                    if (Playback.CmdPause == command)
                    {
                        if (playback != null && playback.IsPlaying)
                        {
                            HandlePauseRequest();
                        }
                    }
                }
            }
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            ServiceCreated = false;
            Binder = null;

            HandleStopRequest(null);
            session.Release();
            session.Dispose();
            session = null;
            serviceStarted = false;
            base.OnDestroy();
        }

        public override BrowserRoot OnGetRoot(string clientPackageName, int clientUid, Bundle rootHints)
        {
            if (!packageValidator.IsCallerAllowed(this, clientPackageName, clientUid))
            {
                return null;
            }
            if (CarHelper.IsValidCarPackage(clientPackageName))
            {
            }
            return new BrowserRoot(MediaIDHelper.MediaIdRoot, null);
        }

        public override void OnLoadChildren(string parentId, Result result)
        {
            if (!musicProvider.IsInitialized)
            {
                result.Detach();

                musicProvider.RetrieveMedia(success =>
                {
                    if (success)
                    {
                        LoadChildrenImpl(parentId, result);
                    }
                    else
                    {
                        result.SendResult(new JavaList<MediaBrowser.MediaItem>());
                    }
                });
            }
            else
            {
                LoadChildrenImpl(parentId, result);
            }
        }

        void LoadChildrenImpl(string parentId, Result result)
        {
        }

        void HandlePauseRequest()
        {
            playback.Pause();
        }

        void HandlePlayRequest()
        {
            if (!serviceStarted && !ServiceCreated)
            {
                serviceStarted = true;
                StartService(new Intent(ApplicationContext, typeof(MusicService)));
            }

            if (ServiceCreated)
            {
                if (session != null && !session.Active)
                    session.Active = true;

                playback?.Play();
            }
        }

        void HandleStopRequest(String withError)
        {
            playback.Stop(true);

            StopSelf();
            serviceStarted = false;
        }

        void UpdateMetadata()
        {
            if (!QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                return;
            }
            MediaSessionCompat.QueueItem queueItem = PlayingQueue[currentIndexOnQueue];

            MediaMetadataCompat track = musicProvider.GetMetadata();

            string trackId = track.GetString(MediaMetadata.MetadataKeyMediaId);
            session.SetMetadata(track);
        }
    }
}