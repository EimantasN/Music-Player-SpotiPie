using Android.Support.V4.Media.Session;
using Mobile_Api.Models;

namespace SpotyPie.Services
{
    public class MediaSessionCustomCallback : MediaSessionCompat.Callback
    {
        private MusicService _musicService;
        private Songs _song;
        public MediaSessionCustomCallback(MusicService service)
        {
            _musicService = service;
        }

        public void SetSoong(Songs song)
        {
            _song = song;
        }

        public override void OnPause()
        {
            _musicService.PlayerPause();
            base.OnPause();
        }

        public override void OnPlay()
        {
            _musicService.PlayerPlay();
            base.OnPlay();
        }

        public override void OnSkipToNext()
        {
            _musicService.ChangeSong(true);
            base.OnSkipToNext();
        }

        public override void OnSkipToPrevious()
        {
            _musicService.ChangeSong(false);
            base.OnSkipToPrevious();
        }

        public override void OnStop()
        {
            _musicService.PlayerPause();
            base.OnStop();
        }
    }
}