using System;

using Android.App;
using Android.Content;
using Android.Views;
using SpotyPie.Services;

namespace SpotyPie
{
    [IntentFilter(new[] { Intent.ActionMediaButton })]
    public class MediaButtonBroadcastReceiver : BroadcastReceiver
    {
        public string ComponentName { get { return Class.Name; } }

        public interface LocationDataInterface
        {
            void OnLocationChanged(Keycode keyEvent);
        }

        public LocationDataInterface mInterface;

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != Intent.ActionMediaButton)
                    return;

                KeyEvent keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);
                if (keyEvent.Action != KeyEventActions.Down)
                    return;

                Intent intend = new Intent(context, typeof(MediaPlayerServiceBinder));
                intend.PutExtra("Data", keyEvent.KeyCode.ToString());
                context.StartService(intend);
            }
            catch (Exception e)
            {

            }
        }
    }
}