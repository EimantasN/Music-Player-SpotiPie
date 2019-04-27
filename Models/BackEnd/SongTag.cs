using IdSharp.Tagging.VorbisComment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.BackEnd
{
    public class SongTag
    {
        public int Id { get; set; }
        public string Artist { get; set; }

        public string Album { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public int Bitrate { get; set; }

        public string Gendre { get; set; }

        public int TrackNumber { get; set; }

        public SongTag(string path)
        {
            var tfile = TagLib.File.Create(path);
            if (!string.IsNullOrEmpty(tfile.Tag.FirstAlbumArtist))
            {
                this.Artist = tfile.Tag.FirstAlbumArtist;
            }
            else
            {
                this.Artist = "Nenurodyta";
            }

            if (!string.IsNullOrEmpty(tfile.Tag.Album))
            {
                this.Album = tfile.Tag.Album;
            }
            else
            {
                this.Album = "Nenurodyta";
            }

            if (!string.IsNullOrEmpty(tfile.Tag.Title))
            {
                this.Title = tfile.Tag.Title;
            }
            else
            {
                this.Title = "Nenurodyta";
            }

            if (!string.IsNullOrEmpty(tfile.Tag.FirstGenre))
            {
                this.Gendre = tfile.Tag.FirstGenre;
            }
            else
            {
                this.Gendre = "Nenurodyta";
            }

            try
            {
                this.Year = (int)tfile.Tag.Year;
                this.TrackNumber = (int)tfile.Tag.Track;
                this.Bitrate = tfile.Properties.AudioBitrate;
            }
            catch
            { }
        }

        //public SongTag(string url)
        //{
        //    var tfile = TagLib.File.Create(url);

        //    //if (!string.IsNullOrEmpty(tfile.Properties))
        //    //{
        //    //    this.Album = tfile.Artist;
        //    //}

        //    //if (!string.IsNullOrEmpty(tfile.Album))
        //    //{
        //    //    this.Album = tfile.Album;
        //    //}

        //    //if (!string.IsNullOrEmpty(tfile.Title))
        //    //{
        //    //    this.Album = tfile.Title;
        //    //}
        //}
    }
}
