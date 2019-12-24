using System.Collections.Generic;
using Android.Media.Session;
using SpotyPie.Music.Models;
using Mobile_Api.Models;
using SpotyPie.Music.Manager;

namespace SpotyPie.Music.Helpers
{
    public static class QueueHelper
    {
        static readonly string Tag = LogHelper.MakeLogTag(typeof(QueueHelper));

        public static Songs GetPlayingSong() => SongManager.Song;

        public static List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> GetPlayingQueue(string mediaId, MusicProvider musicProvider)
        {
            // extract the browsing hierarchy from the media ID:
            string[] hierarchy = MediaIDHelper.GetHierarchy(mediaId);

            if (hierarchy.Length != 2)
            {
                return null;
            }

            string categoryType = hierarchy[0];
            string categoryValue = hierarchy[1];

            IEnumerable<Android.Support.V4.Media.MediaMetadataCompat> tracks = null;

            if (tracks == null)
            {
                return null;
            }

            return ConvertToQueue(tracks, hierarchy[0], hierarchy[1]);
        }

        public static List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> GetPlayingQueueFromSearch(string query, MusicProvider musicProvider)
        {
            return new List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem>();
        }


        public static int GetMusicIndexOnQueue(IEnumerable<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> queue, string mediaId)
        {
            int index = 0;
            foreach (var item in queue)
            {
                if (mediaId == item.Description.MediaId)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static int GetMusicIndexOnQueue(IEnumerable<MediaSession.QueueItem> queue, long queueId)
        {
            int index = 0;
            foreach (var item in queue)
            {
                if (queueId == item.QueueId)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        static List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> ConvertToQueue(IEnumerable<Android.Support.V4.Media.MediaMetadataCompat> tracks, params string[] categories)
        {
            var queue = new List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem>();
            int count = 0;
            foreach (var track in tracks)
            {
                string hierarchyAwareMediaID = MediaIDHelper.CreateMediaID(track.Description.MediaId, categories);
                Android.Support.V4.Media.MediaMetadataCompat trackCopy = new Android.Support.V4.Media.MediaMetadataCompat.Builder(track)
                    .PutString(Android.Support.V4.Media.MediaMetadataCompat.MetadataKeyMediaId, hierarchyAwareMediaID)
                    .Build();

                var item = new Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem(trackCopy.Description, count++);
                queue.Add(item);
            }
            return queue;

        }

        public static List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> GetRandomQueue(MusicProvider musicProvider)
        {
            List<string> genres = musicProvider.GetCurrentSongListGendres();

            IEnumerable<Android.Support.V4.Media.MediaMetadataCompat> tracks = musicProvider.GetCurrentSongList();

            return ConvertToQueue(tracks, MediaIDHelper.MediaIdMusicsByGenre, genres[0]);
        }

        public static bool isIndexPlayable(int index, List<Android.Support.V4.Media.Session.MediaSessionCompat.QueueItem> queue)
        {
            return (queue != null && index >= 0 && index < queue.Count);
        }
    }
}
