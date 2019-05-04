using Android.Content;

namespace SpotyPie.Services.Restarters
{
    [BroadcastReceiver]
    public class MusicServiceRestart : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //context.StartService(new Intent(context, typeof(MusicService)));
        }
    }
}