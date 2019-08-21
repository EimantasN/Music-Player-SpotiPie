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

                if(!realm.All<Music>().Any(x => x.Id == song.Id))
                    realm.Add(new Music(song, true), true);
            });
        }

        public Realm_Songs GetCurrentSong()
        {
            return PlayingSong;
        }

        public void Rewrite(Realms.Realm realm, List<Songs> songs, int position)
        {
            if (songs != null && songs.Count != 0)
            {
                realm.Write(() =>
                {
                    LastCount = 0;
                    CurrentCount = songs.Count;
                    UpdatedAt = DateTime.Now;
                    PlayingSong = new Realm_Songs(songs[position]);
                    IsPlaying = true;

                    realm.RemoveAll<Music>();

                    foreach (var song in songs)
                        realm.Add(new Music(song));
                });
            }
            else
                throw new Exception("song list can't be null");
        }

        public List<Music> GetSongList(Realms.Realm realm)
        {
            return realm.All<Music>().ToList();
        }

        public void UpdateCurrentSong(Realms.Realm realm, Realm_Songs song)
        {
            if (realm.All<Music>().Any(x => x.Id == song.Id))
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
