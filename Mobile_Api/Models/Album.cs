using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile_Api.Models
{
    public class Album
    {

        public int Id { get; set; }

        public string SpotifyId { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public List<Song> Songs { get; set; }

        public int Popularity { get; set; }

        public bool IsPlayable { get; set; }

        public DateTime LastActiveTime { get; set; }

        public int Tracks { get; set; }


        public Album()
        {

        }

        public Album(Models.Spotify.Album al)
        {
            //Id = 0;
            //Created = DateTime.Now;
            //AlbumType = al.AlbumType;
            //Artists = al.Artists != null ? JsonConvert.SerializeObject(Helpers.GetArtist(al.Artists)) : null;
            //Copyrights = al.Copyrights != null ? JsonConvert.SerializeObject(Helpers.GetCopyrights(al.Copyrights)) : null;
            //Genres = al.Genres != null ? JsonConvert.SerializeObject(al.Genres.ToList()) : null;
            //Images = al.Images != null ? Helpers.GetImages(al.Images) : null;
            //Label = al.Label;
            //Name = al.Name;
            //Popularity = 0;
            //ReleaseDate = al.ReleaseDate;
            //TotalTracks = al.TotalTracks;
            //Songs = al.Tracks != null ? Helpers.GetSongs(al.Tracks.Songs) : null;
            //LastActiveTime = DateTime.Now;
        }
    }
}
