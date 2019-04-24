using Android.OS;

namespace SpotyPie.Services
{
    public class LocalBinder : Binder
    {
        public LocalBinder(MusicService service)
        {
            this.Service = service;
        }

        public MusicService Service { get; private set; }
    }
}