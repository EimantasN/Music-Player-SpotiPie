using System;

namespace Models.BackEnd
{
    public class AudioBindError
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Song { get; set; }
        public string Message { get; set; }

        public AudioBindError(string fileName, string artist, string album, string song, string msg)
        {
            this.FileName = fileName;
            this.Artist = artist;
            this.Album = album;
            this.Song = song;
            this.Message = msg;
        }

        public AudioBindError(Exception e)
        {
            this.Message = e.Message;
        }
    }
}
