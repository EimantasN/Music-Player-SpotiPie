using System;
using Android.Content;
using SpotyPie.Base;

namespace SpotyPie.Music.Manager
{
    internal class ServiceConnection
    {
        internal WeakReference<ActivityBase> _activity { get; set; }

        internal void Bind(ActivityBase activity)
        {
            _activity = new WeakReference<ActivityBase>(activity);
        }

        internal void PlayerPlay()
        {
            TryGetActivity()?.SendBroadcast(new Intent(MediaNotificationManager.ActionPlay));
        }

        internal void PlayerPrev()
        {
            TryGetActivity()?.SendBroadcast(new Intent(MediaNotificationManager.ActionPrev));
        }

        internal void PlayerNext()
        {
            TryGetActivity()?.SendBroadcast(new Intent(MediaNotificationManager.ActionNext));
        }

        internal void PlayerPause()
        {
            TryGetActivity()?.SendBroadcast(new Intent(MediaNotificationManager.ActionPause));
        }

        private ActivityBase TryGetActivity()
        {
            if (_activity == null)
                return null;

            if (_activity.TryGetTarget(out ActivityBase activity))
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