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
using SpotyPie.Music.Interface;
using Android.Support.V4.Media;
using MediaSessionCompat = Android.Support.V4.Media.Session.MediaSessionCompat;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;

namespace SpotyPie.Music
{
    [Service(Exported = true)]
    public class MusicService : MediaBrowserServiceCompat, Playback.ICallback, MusicControlInterface
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

        MediaSessionCompat session;
        List<MediaSessionCompat.QueueItem> PlayingQueue;
        MusicProvider musicProvider;
        public Playback playback;
        MediaNotificationManager mediaNotificationManager;
        DelayedStopHandler delayedStopHandler;
        PackageValidator packageValidator;
        MediaSessionCallback mediaCallback;

        public MusicService()
        {
            delayedStopHandler = new DelayedStopHandler(this);
        }

        internal void SetServiceIsStopped() { serviceStarted = false; }

        public override void OnCreate()
        {
            base.OnCreate();

            PlayingQueue = new List<MediaSessionCompat.QueueItem>();
            musicProvider = new MusicProvider();
            packageValidator = new PackageValidator(this);

            session = new MediaSessionCompat(this, "MusicService");
            SessionToken = session.SessionToken;
            mediaCallback = new MediaSessionCallback();

            mediaCallback.OnPlayImpl = () =>
            {
                Toast.MakeText(ApplicationContext, "OnPlayFromMediaIdImpl", ToastLength.Long).Show();
                HandlePlayRequest();
            };

            mediaCallback.OnSkipToQueueItemImpl = (id) =>
            {
                LogHelper.Debug(Tag, "OnSkipToQueueItem:" + id);

                if (PlayingQueue != null && PlayingQueue.Count != 0)
                {
                    currentIndexOnQueue = QueueHelper.GetMusicIndexOnQueue(PlayingQueue, id.ToString());
                    HandlePlayRequest();
                }
            };

            mediaCallback.OnSeekToImpl = (pos) =>
            {
                playback.SeekTo((int)pos);
            };

            mediaCallback.OnPlayFromMediaIdImpl = (mediaId, extras) =>
            {
                Toast.MakeText(ApplicationContext, "OnPlayFromMediaIdImpl", ToastLength.Long).Show();
                return;

                LogHelper.Debug(Tag, "playFromMediaId mediaId:", mediaId, "  extras=", extras);

                PlayingQueue = QueueHelper.GetPlayingQueue(mediaId, musicProvider);
                session.SetQueue(PlayingQueue);
                var queueTitle = GetString(Resource.String.browse_musics_by_genre_subtitle,
                                     MediaIDHelper.ExtractBrowseCategoryValueFromMediaID(mediaId));
                session.SetQueueTitle(queueTitle);

                if (PlayingQueue != null && PlayingQueue.Count != 0)
                {
                    currentIndexOnQueue = QueueHelper.GetMusicIndexOnQueue(PlayingQueue, mediaId);

                    if (currentIndexOnQueue < 0)
                    {
                        LogHelper.Error(Tag, "playFromMediaId: media ID ", mediaId,
                            " could not be found on queue. Ignoring.");
                    }
                    else
                    {
                        HandlePlayRequest();
                    }
                }
            };

            mediaCallback.OnPauseImpl = () =>
            {
                Toast.MakeText(ApplicationContext, "OnPauseImpl", ToastLength.Long).Show();
                HandlePauseRequest();
            };

            mediaCallback.OnStopImpl = () =>
            {
                Toast.MakeText(ApplicationContext, "OnStopImpl", ToastLength.Long).Show();
                HandleStopRequest(null);
            };

            mediaCallback.OnSkipToNextImpl = () =>
            {
                Toast.MakeText(ApplicationContext, "OnSkipToNextImpl", ToastLength.Long).Show();
                mediaNotificationManager.CountSkip++;
                playback.Skip(null);

                Task.Run(async () =>
                {
                    await musicProvider.GetNextSongAsync();
                    Application.SynchronizationContext.Post(_ =>
                    {
                        playback.Play(null);
                    }, null);
                });
                return;
            };

            mediaCallback.OnSkipToPreviousImpl = () =>
            {
                Toast.MakeText(ApplicationContext, "OnSkipToPreviousImpl", ToastLength.Long).Show();
                mediaNotificationManager.CountSkip--;
                playback.Skip(null);

                Task.Run(async () =>
                {
                    await musicProvider.GetNextSongAsync();
                    Application.SynchronizationContext.Post(_ =>
                    {
                        playback.Play(null);
                    }, null);
                });
                return;
            };

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
                    LogHelper.Error(Tag, "Unsupported action: ", action);
                }
            };

            mediaCallback.OnPlayFromSearchImpl = (query, extras) =>
            {
                LogHelper.Debug(Tag, "playFromSearch  query=", query);

                if (string.IsNullOrEmpty(query))
                {
                    PlayingQueue = new List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem>(QueueHelper.GetRandomQueue(musicProvider));
                }
                else
                {
                    PlayingQueue = new List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem>(QueueHelper.GetPlayingQueueFromSearch(query, musicProvider));
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
            playback.Start();

            Context context = ApplicationContext;
            var intent = new Intent(context, typeof(MainActivity));
            var pi = PendingIntent.GetActivity(context, 99 /*request code*/, intent, PendingIntentFlags.UpdateCurrent);
            session.SetSessionActivity(pi);

            var extraBundle = new Bundle();
            CarHelper.SetSlotReservationFlags(extraBundle, true, true, true);
            session.SetExtras(extraBundle);

            mediaNotificationManager = new MediaNotificationManager(this);

            UpdatePlaybackState(null);
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
            HandleStopRequest(null);

            delayedStopHandler.RemoveCallbacksAndMessages(null);
            session.Release();
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

        public override async void OnLoadChildren(string parentId, Result result)
        {
            if (!musicProvider.IsInitialized)
            {
                result.Detach();

                await musicProvider.RetrieveMediaAsync(success =>
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
            delayedStopHandler.RemoveCallbacksAndMessages(null);
            delayedStopHandler.SendEmptyMessageDelayed(0, StopDelay);
        }

        void HandlePlayRequest()
        {
            delayedStopHandler.RemoveCallbacksAndMessages(null);
            if (!serviceStarted)
            {
                StartService(new Intent(ApplicationContext, typeof(MusicService)));
                serviceStarted = true;
            }

            if (!session.Active)
                session.Active = true;

            if (QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                UpdateMetadata();
                playback.Play(PlayingQueue[currentIndexOnQueue]);
            }
            playback.Play(null);
        }

        void HandleStopRequest(String withError)
        {
            playback.Stop(true);
            delayedStopHandler.RemoveCallbacksAndMessages(null);
            delayedStopHandler.SendEmptyMessageDelayed(0, StopDelay);

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

            // If there is an error message, send it to the playback state:
            if (error != null)
            {
                // Error states are really only supposed to be used for errors that cause playback to
                // stop unexpectedly and persist until the user takes action to fix it.
                stateBuilder.SetErrorMessage(error);
                state = Android.Support.V4.Media.Session.PlaybackStateCompat.StateError;
            }
            stateBuilder.SetState(state, position, 1.0f, SystemClock.ElapsedRealtime());

            // Set the activeQueueItemId if the current index is valid.
            if (QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                var item = PlayingQueue[currentIndexOnQueue];
                stateBuilder.SetActiveQueueItemId(item.QueueId);
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
                // Set appropriate "Favorite" icon on Custom action:
                var musicId = currentMusic.GetString(MediaMetadata.MetadataKeyMediaId);
                //TODO
                var favoriteIcon = Resource.Drawable.abc_ic_star_black_16dp;
                if (musicProvider.IsFavorite(musicId))
                {
                    //TODO
                    favoriteIcon = Resource.Drawable.abc_ic_star_half_black_16dp;
                }
                LogHelper.Debug(Tag, "updatePlaybackState, setting Favorite custom action of music ",
                    musicId, " current favorite=", musicProvider.IsFavorite(musicId));
                stateBuilder.AddCustomAction(CustomActionThumbsUp, "Favorite",
                    favoriteIcon);
            }
        }

        long GetAvailableActions()
        {
            long actions = PlaybackState.ActionPlay | PlaybackState.ActionPlayFromMediaId |
                           PlaybackState.ActionPlayFromSearch;
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
            if (QueueHelper.isIndexPlayable(currentIndexOnQueue, PlayingQueue))
            {
                var item = PlayingQueue[currentIndexOnQueue];
                if (item != null)
                {
                    return musicProvider.GetMetadata();
                }
            }
            return musicProvider.GetMetadata();
        }

        public void OnCompletion()
        {
            if (PlayingQueue != null && PlayingQueue.Count != 0)
            {
                // In this sample, we restart the playing queue when it gets to the end:
                currentIndexOnQueue++;
                if (currentIndexOnQueue >= PlayingQueue.Count)
                {
                    currentIndexOnQueue = 0;
                }
                HandlePlayRequest();
            }
            else
            {
                // If there is nothing to play, we stop and release the resources:
                HandleStopRequest(null);
            }
        }

        public void OnPlaybackStatusChanged(int state)
        {
            UpdatePlaybackState(null);
        }

        public void OnError(string error)
        {
            UpdatePlaybackState(error);
        }
    }
}