using System;
using Android.OS;

namespace SpotyPie.Music
{
    public class DelayedStopHandler : Handler
    {
        readonly WeakReference<MusicService> weakReference;

        public DelayedStopHandler(MusicService service)
        {
            weakReference = new WeakReference<MusicService>(service);
        }

        public override void HandleMessage(Message msg)
        {
            MusicService service;
            weakReference.TryGetTarget(out service);
            if (service != null && service.playback != null)
            {
                if (service.playback.IsPlaying)
                {
                    //Ignoring delayed stop since the media player is in use.
                    return;
                }
                service.StopSelf();
                service.SetServiceIsStopped();
            }
        }
    }
}