using Realms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mobile_Api.Models.Realm
{
    public class ApplicationSongList : RealmObject
    {
        public int Id { get; set; }

        public int CurrentCount { get; set; }

        public int LastCount { get; set; }

        public Realm_Songs PlayingSong { get; set; }

        public bool IsPlaying { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public IList<Realm_Songs> Songs { get; private set; } = new List<Realm_Songs>();

        public ApplicationSongList()
        {
            Id = 1;
            CurrentCount = 0;
            LastCount = 0;
            PlayingSong = null;
            IsPlaying = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void Add(Realms.Realm realm, Realm_Songs song)
        {
            realm.Write(() =>
            {
                LastCount = CurrentCount;
                CurrentCount++;
                PlayingSong = song;
                UpdatedAt = DateTime.Now;

                Songs.Add(song);
            });
        }

        public Realm_Songs GetCurrentSong()
        {
            return PlayingSong;
        }

        public void Rewrite(Realms.Realm realm, List<Songs> songs, int position)
        {
            realm.Write(() =>
            {
                LastCount = 0;
                CurrentCount = songs.Count;
                UpdatedAt = DateTime.Now;
                PlayingSong = new Realm_Songs(songs[position]);
                IsPlaying = true;

                for (int i = 0; i < Songs.Count; i++)
                    Songs.RemoveAt(i);

                foreach (var song in songs)
                    Songs.Add(new Realm_Songs(song));
            });
        }

        public void UpdateCurrentSong(Realms.Realm realm, Realm_Songs song)
        {
            if (Songs.Any(x => x.Id == song.Id))
            {
                realm.Write(() =>
                {
                    PlayingSong = song;
                    UpdatedAt = DateTime.Now;
                    IsPlaying = true;
                });
            }
        }
    }
}
