﻿namespace Models.BackEnd
{
    public class CurrentSong
    {
        public int Id { get; set; }

        public int ArtistId { get; set; }

        public int PlaylistId { get; set; }

        public int AlbumId { get; set; }

        public int SongId { get; set; }

        public long DurationMs { get; set; }

        public int CurrentMs { get; set; }

        public string Name { get; set; }

        public string LocalUrl { get; set; }

        public string Image { get; set; }

        public CurrentSong() { }

        public CurrentSong(Song song)
        {
            ArtistId = song.ArtistId;
            AlbumId = song.AlbumId;
            SongId = song.Id;
        }

    }
}
