using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Media.Browse;
using Android.Media.Session;
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
    public class MusicService : MediaBrowserServiceCompat, Playback.ICallback
    {
        public bool InitNotification = true;

        public const string ActionCmd = "com.spotypie.adnroid.musicservice.ACTION_CMD";
        public const string CmdName = "CMD_NAME";
        public const string CmdPause = "CMD_PAUSE";

        static readonly string Tag = LogHelper.MakeLogTag(typeof(MusicService));
        const string CustomActionThumbsUp = "com.spotypie.adnroid.musicservice.THUMBS_UP";
        const int StopDelay = 30000;

        int currentIndexOnQueue;
        bool serviceStarted;

        public Playback.ICallback CallBack;

        MediaSessionCompat session;
        List<MediaSessionCompat.QueueItem> PlayingQueue;
        MusicProvider musicProvider;
        public Playback playback;
        MediaNotificationManager mediaNotificationManager;
        //DelayedStopHandler delayedStopHandler;
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

        public MusicService()
        {
            //delayedStopHandler = new DelayedStopHandler(this);
        }

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
                LogHelper.Debug(Tag, "OnSkipToQueueItem:" + id);

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
                if (CustomActionThumbsUp == action)
                {
                    var track = GetCurrentPlayingMusic();
                    if (track != null)
                    {
                        var musicId = track.GetString(MediaMetadata.MetadataKeyMediaId);
                        musicProvider.SetFavorite(musicId, !musicProvider.IsFavorite(musicId));
                    }
                    UpdatePlaybackState(null);
                }
                else
                {
                    Toast.MakeText(ApplicationContext, $"Unsuported action {action}", ToastLength.Short).Show();
                }
            };

            //MUSIC PLAYER PLAYSEARCH ACTION
            mediaCallback.OnPlayFromSearchImpl = (query, extras) =>
            {
                LogHelper.Debug(Tag, "playFromSearch  query=", query);

                if (string.IsNullOrEmpty(query))
                {
                    PlayingQueue = new List<MediaSessionCompat.QueueItem>(QueueHelper.GetRandomQueue(musicProvider));
                }
                else
                {
                    PlayingQueue = new List<MediaSessionCompat.QueueItem>(QueueHelper.GetPlayingQueueFromSearch(query, musicProvider));
                }

                LogHelper.Debug(Tag, "playFromSearch  playqueue.length=" + PlayingQueue.Count);
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

            playback = new Playback(this, musicProvider);
            playback.State = Android.Support.V4.Media.Session.PlaybackStateCompat.StateNone;
            playback.Callback = this;

            var intent = new Intent(ApplicationContext, typeof(MainActivity));
            var pi = PendingIntent.GetActivity(ApplicationContext, 99 /*request code*/, intent, PendingIntentFlags.UpdateCurrent);
            session.SetSessionActivity(pi);

            var extraBundle = new Bundle();
            CarHelper.SetSlotReservationFlags(extraBundle, true, true, true);
            session.SetExtras(extraBundle);

            mediaNotificationManager = new MediaNotificationManager(this);

            UpdatePlaybackState(null);

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
                var command = intent.GetStringExtra(CmdName);
                if (ActionCmd == action)
                {
                    if (CmdPause == command)
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
                        UpdatePlaybackState("error_no_metadata");
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
            //delayedStopHandler.RemoveCallbacksAndMessages(null);
            //delayedStopHandler.SendEmptyMessageDelayed(0, StopDelay);
        }

        void HandlePlayRequest()
        {
            //delayedStopHandler?.RemoveCallbacksAndMessages(null);
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
            //delayedStopHandler.RemoveCallbacksAndMessages(null);
            //delayedStopHandler.SendEmptyMessageDelayed(0, StopDelay);

            UpdatePlaybackState(withError);

            StopSelf();
            serviceStarted = false;
        }

        void UpdateMetadata()
        {
            if (!QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                UpdatePlaybackState("error_no_metadata");
                return;
            }
            MediaSessionCompat.QueueItem queueItem = PlayingQueue[currentIndexOnQueue];

            MediaMetadataCompat track = musicProvider.GetMetadata();

            string trackId = track.GetString(MediaMetadata.MetadataKeyMediaId);
            session.SetMetadata(track);
        }

        void UpdatePlaybackState(String error)
        {
            var position = Android.Support.V4.Media.Session.PlaybackStateCompat.PlaybackPositionUnknown;
            if (playback != null && playback.IsConnected)
            {
                position = playback.CurrentStreamPosition;
            }

            var stateBuilder = new Android.Support.V4.Media.Session.PlaybackStateCompat.Builder().SetActions(GetAvailableActions());

            SetCustomAction(stateBuilder);

            //TODO REMOVE
            if (InitNotification)
            {
                playback.State = Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying;
                InitNotification = false;
            }

            var state = playback.State;

            if (error != null)
            {
                stateBuilder.SetErrorMessage(error);
                state = Android.Support.V4.Media.Session.PlaybackStateCompat.StateError;
            }
            stateBuilder.SetState(state, position, 1.0f, SystemClock.ElapsedRealtime());

            if (QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                try
                {
                    var item = PlayingQueue[currentIndexOnQueue];
                    stateBuilder.SetActiveQueueItemId(item.QueueId);
                }
                catch (Exception e)
                {
                    
                }
            }

            session.SetPlaybackState(stateBuilder.Build());

            if (state == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePlaying || state == Android.Support.V4.Media.Session.PlaybackStateCompat.StatePaused)
            {
                mediaNotificationManager.StartNotification();
            }
        }

        void SetCustomAction(Android.Support.V4.Media.Session.PlaybackStateCompat.Builder stateBuilder)
        {
            MediaMetadataCompat currentMusic = GetCurrentPlayingMusic();
            if (currentMusic != null)
            {
                var musicId = currentMusic.GetString(MediaMetadata.MetadataKeyMediaId);
                //TODO
                var favoriteIcon = Resource.Drawable.abc_ic_star_black_16dp;
                if (musicProvider.IsFavorite(musicId))
                {
                    //TODO
                    favoriteIcon = Resource.Drawable.abc_ic_star_half_black_16dp;
                }
                stateBuilder.AddCustomAction(CustomActionThumbsUp, "Favorite", favoriteIcon);
            }
        }

        long GetAvailableActions()
        {
            long actions = PlaybackState.ActionPlay | PlaybackState.ActionPlayFromMediaId | PlaybackState.ActionPlayFromSearch;
            if (PlayingQueue == null || PlayingQueue.Count == 0)
            {
                return actions;
            }
            if (playback.IsPlaying)
            {
                actions |= PlaybackState.ActionPause;
            }
            if (currentIndexOnQueue > 0)
            {
                actions |= PlaybackState.ActionSkipToPrevious;
            }
            if (currentIndexOnQueue < PlayingQueue.Count - 1)
            {
                actions |= PlaybackState.ActionSkipToNext;
            }
            return actions;
        }

        MediaMetadataCompat GetCurrentPlayingMusic()
        {
            try
            {
                if (QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
                {
                    var item = PlayingQueue[currentIndexOnQueue];
                    if (item != null)
                    {
                        return musicProvider.GetMetadata();
                    }
                }
            }
            catch (Exception e)
            {
                
            }
            return musicProvider.GetMetadata();
        }

        public void OnCompletion()
        {
            OnNextSong();
            //if (PlayingQueue != null && PlayingQueue.Count != 0)
            //{
            //    // In this sample, we restart the playing queue when it gets to the end:
            //    currentIndexOnQueue++;
            //    if (currentIndexOnQueue >= PlayingQueue.Count)
            //    {
            //        currentIndexOnQueue = 0;
            //    }
            //HandlePlayRequest();
            //}
            //else
            //{
            //    // If there is nothing to play, we stop and release the resources:
            //    HandleStopRequest(null);
            //}
        }

        public void OnPlaybackStatusChanged(int state)
        {
            UpdatePlaybackState(null);
            CallBack?.OnPlaybackStatusChanged(state);
        }

        public void OnPlaybackMetaDataChanged(MediaMetadataCompat meta)
        {
            if (mediaNotificationManager != null)
            {
                mediaNotificationManager.Metadata = meta;
            }
        }

        public void OnError(string error)
        {
            UpdatePlaybackState(error);
        }

        public void OnPositionChanged(int miliseconds, TimeSpan currentTime)
        {
            CallBack?.OnPositionChanged(miliseconds, currentTime);
        }

        internal void SetCallback(Player.Player player)
        {
            CallBack = player;
        }

        public void OnDurationChanged(int miliseconds)
        {
            CallBack?.OnDurationChanged(miliseconds);
        }

        public void OnPlay()
        {
            
        }

        public void OnStop()
        {
        }
    }
}