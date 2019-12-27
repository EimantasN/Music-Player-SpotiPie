using Android.Support.V4.Media.Session;
using Mobile_Api.Models;
using SpotyPie.Base;
using SpotyPie.Music.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotyPie.Music.Manager
{
    public static class SongManager
    {
        private static ServiceConnection _serviceConnection { get; set; } = new ServiceConnection();

        public static List<Songs> SongQueue { get; set; } = new List<Songs>();

        public static int Index { get; set; }

        public static int SongId { get; set; } = DefaultValues.DefaultSongId;

        private static object _nextSongLoadLock { get; set; } = new object();

        private static bool NexSongIsLoading { get; set; }

        public static bool IsPlaying
        {
            get
            {
                if (_playState == PlayState.Playing)
                    return true;
                return false;
            }
        }

        public static PlayState _playState { get; set; } = PlayState.Stopeed;

        public static Songs Song => TryGetSongByIndex(Index);

        public static bool StateLoaded { get; set; }

        //DELEGATES

        public delegate void PlayingState(PlayState state);

        public static PlayingState PlayingHandler;


        public delegate void CurrentSong(Songs song);

        public static CurrentSong SongHandler;


        public delegate void SongList(List<Songs> song);

        public static SongList SongListHandler;

        public static WeakReference<ActivityBase> ActivityRef;

        public static void SetSongs(List<Songs> songs, int index = 0)
        {
            Index = index;
            SongQueue = songs;
            SongListHandler?.Invoke(songs);

            Play();

            TryGetActivity()?.StartPlayer();

            Playback.StateHandler += OnStateChange;
        }

        public static void OnStateChange(int state)
        {
            if (state == PlaybackStateCompat.StatePlaying)
            {
                SetPlayState(PlayState.Playing);
            }
            else if (state == PlaybackStateCompat.StatePaused || state == PlaybackStateCompat.StateStopped)
            {
                SetPlayState(PlayState.Stopeed);
            }
            else
            {
                SetPlayState(PlayState.Loading);
            }
        }

        public static void Play()
        {
            SetPlayState(PlayState.Loading);
            if (Index >= 0 && Index < SongQueue.Count)
            {
                if (SongQueue[Index] != null)
                {
                    SongHandler?.Invoke(SongQueue[Index]);
                    SongId = SongQueue[Index].Id;
                    _serviceConnection.PlayerPlay();
                }
            }
        }

        public static void Pause()
        {
            _playState = PlayState.Stopeed;
            PlayingHandler?.Invoke(_playState);
        }

        public static bool Next()
        {
            if (_playState == PlayState.Loading)
            {
                return false;
            }

            if (Index + 1 >= SongQueue.Count)
            {
                return false;
                //Load next song from WS
            }
            else if (Index >= 0 && Index + 1 < SongQueue.Count)
            {
                if (SongQueue[Index + 1] != null)
                {
                    SongHandler?.Invoke(SongQueue[Index + 1]);
                    SongId = SongQueue[++Index].Id;
                    _serviceConnection.PlayerPlay();

                    if (Index + 1 == SongQueue.Count)
                    {
                        LoadNextSong();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static async void LoadNextSong()
        {
            var loadNextSong = false;
            lock (_nextSongLoadLock)
            {
                if (!NexSongIsLoading)
                {
                    NexSongIsLoading = true;
                    loadNextSong = true;
                }
            }
            if (loadNextSong)
            {
                var song = await TryGetActivity()?
                    .GetAPIService()
                    .GetNextSongAsync(SongQueue[SongQueue.Count - 1].Id);
                if (song != null)
                {
                    TryGetActivity()?.RunOnUiThread(() =>
                    {
                        SongQueue.Add(song);
                        SongListHandler?.Invoke(SongQueue);
                    });
                }
                NexSongIsLoading = false;
            }
        }

        public static bool Prev()
        {
            if (_playState == PlayState.Loading)
            {
                return false;
            }

            if (Index == 0)
            {
                //Ignore
                return false;
            }
            else if (Index > 0)
            {
                if (SongQueue[Index - 1] != null)
                {
                    SongHandler?.Invoke(SongQueue[Index - 1]);
                    SongId = SongQueue[--Index].Id;
                    _serviceConnection.PlayerPlay();
                    return true;
                }
            }
            return false;
        }

        private static void SetPlayState(PlayState state)
        {
            _playState = state;
            PlayingHandler?.Invoke(_playState);
        }

        public static void ToggleState()
        {
            if (_playState == PlayState.Playing)
            {
                _serviceConnection.PlayerPause();
            }
            else if (_playState == PlayState.Stopeed)
            {
                _serviceConnection.PlayerPlay();
            }
        }

        public static Songs TryGetSongByIndex(int index)
        {
            if (index >= 0 && index < SongQueue.Count)
            {
                return SongQueue[index];
            }
            else
            {
                return null;
            }
        }

        public static void LoadCurrentSong()
        {
            if (!StateLoaded)
            {
                StateLoaded = true;
                Task.Run(async () =>
                {
                    var activity = TryGetActivity();
                    if (activity != null)
                    {
                        var song = await activity?.GetAPIService()?.GetCurrentSong();
                        activity?.RunOnUiThread(() =>
                        {
                            SongQueue?.Add(song);

                            PlayingHandler?.Invoke(_playState = PlayState.Stopeed);
                            SongHandler?.Invoke(song);
                            SongListHandler?.Invoke(SongQueue);
                        });
                    }
                });
            }
        }

        internal static void SetAcitivityRef(ActivityBase activityBase)
        {
            ActivityRef = new WeakReference<ActivityBase>(activityBase);
            _serviceConnection.Bind(activityBase);
        }

        private static ActivityBase TryGetActivity()
        {
            if (ActivityRef == null)
                return null;

            if (ActivityRef.TryGetTarget(out ActivityBase activity))
            {
                return activity;
            }
            else
            {
                return null;
            }
        }
    }
}