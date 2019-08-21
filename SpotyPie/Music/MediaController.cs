using System;
using Android.Support.V4.Media;
using Android.Support.V4.Media.Session;

namespace SpotyPie.Music
{
    public class SpotyPieMediaControllerCallback : MediaControllerCompat.Callback
    {
        protected MediaMetadataCompat Metadata { get; set; }
        public Action<PlaybackStateCompat> OnPlaybackStateChangedImpl { get; set; }
        public Action<MediaMetadataCompat> OnMetadataChangedImpl { get; set; }
        public Action OnSessionDestroyedImpl { get; set; }

        public override void OnPlaybackStateChanged(PlaybackStateCompat state)
        {
            OnPlaybackStateChangedImpl(state);
        }

        public override void OnMetadataChanged(MediaMetadataCompat meta)
        {
            Metadata = meta;
            OnMetadataChangedImpl(meta);
        }

        public override void OnSessionDestroyed()
        {
            base.OnSessionDestroyed();
            OnSessionDestroyedImpl();
        }
    }
}