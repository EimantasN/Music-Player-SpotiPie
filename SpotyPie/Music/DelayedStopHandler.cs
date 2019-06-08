using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SpotyPie.Music.Helpers;

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
                    LogHelper.Debug("DelayedStopHandler", "Ignoring delayed stop since the media player is in use.");
                    return;
                }
                LogHelper.Debug("DelayedStopHandler", "Stopping service with delay handler.");
                service.StopSelf();
                service.SetServiceIsStopped();
            }
        }
    }
}