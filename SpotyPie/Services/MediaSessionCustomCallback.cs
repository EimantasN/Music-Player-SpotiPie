using Android.Support.V4.Media.Session;

namespace SpotyPie.Services
{
    public class MediaSessionCustomCallback : MediaSessionCompat.Callback
    {
        private MediaPlayerServiceBinder mediaPlayerService;
        public MediaSessionCustomCallback(MediaPlayerServiceBinder service)
        {
            mediaPlayerService = service;
        }

        public override void OnPause()
        {
            mediaPlayerService.GetMediaPlayerService().Pause();
            base.OnPause();
        }

        public override void OnPlay()
        {
            mediaPlayerService.GetMediaPlayerService().Play();
            base.OnPlay();
        }

        public override void OnSkipToNext()
        {
            mediaPlayerService.GetMediaPlayerService().PlayNext();
            base.OnSkipToNext();
        }

        public override void OnSkipToPrevious()
        {
            mediaPlayerService.GetMediaPlayerService().PlayPrevious();
            base.OnSkipToPrevious();
        }

        public override void OnStop()
        {
            mediaPlayerService.GetMediaPlayerService().Stop();
            base.OnStop();
        }
    }
}