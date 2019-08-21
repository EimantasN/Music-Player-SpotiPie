using Android.OS;

namespace SpotyPie.Music
{
    public class MusicServiceBinder : Binder
    {
        private MusicService Service { get; set; }

        public MusicServiceBinder(MusicService service)
        {
            Service = service;
        }

        internal void SetMusicServiceUpdateCallback(Player.Player player)
        {
            Service?.SetCallback(player);
        }
    }
}