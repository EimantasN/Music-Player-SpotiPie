using Realms;

namespace Mobile_Api.Models.Realm
{
    public class Music : RealmObject
    {
        public int Id { get; set; }

        public bool IsPlaying { get; set; }

        public Realm_Songs Song { get; set; }

        public Music() { }

        public Music(Realm_Songs song, bool playing = false)
        {
            Id = song.Id;
            IsPlaying = playing;
            song.IsPlaying = playing;
            Song = song;
        }

        public Music(Songs song, bool playing = false)
        {
            Id = song.Id;
            IsPlaying = playing;
            song.IsPlaying = playing;
            Song = new Realm_Songs(song);
        }
    }
}
