using Android.OS;

namespace SpotyPie.Music
{
    public class MusicServiceBinder : Binder
    {
        private MusicService Service { get; set; }

        private bool Connected { get; set; }

        public MusicServiceBinder(MusicService service)
        {
            Service = service;
        }

        internal void SetConnectionStatus(bool status)
        {
            Connected = status;
        }
    }
}