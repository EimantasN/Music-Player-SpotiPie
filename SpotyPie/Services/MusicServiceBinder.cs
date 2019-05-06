using Android.OS;

namespace SpotyPie.Services
{
    public class MusicServiceBinder : Binder
    {
        public MusicServiceBinder(MusicService service)
        {
            this.Service = service;
        }

        public MusicService Service { get; private set; }
    }
}