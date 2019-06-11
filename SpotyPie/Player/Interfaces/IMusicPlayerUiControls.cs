using Mobile_Api.Models;

namespace SpotyPie.Player.Interfaces
{
    public interface IMusicPlayerUiControls
    {
        void Start(Realm_Songs song);

        void SongLoadStarted();

        void SkipToNext();
        void SkipToPrevious();
        void Pause();
        void SongLoadEnded();
    }
}