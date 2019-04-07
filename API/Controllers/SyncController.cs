using API.SpotifyAPI;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly SpotyPieIDbContext _ctx;

        public SyncController(SpotyPieIDbContext ctx)
        {

            _ctx = ctx;
        }


        [HttpGet("GetQuote/{id}")]
        public Quote AddedQuote(int id)
        {
            try
            {
                return _ctx.Quotes.First(x => x.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpPost("AddQuote")]
        public void AddedQuote([FromForm] string quote)
        {
            try
            {
                var Quotes = _ctx.Artists.Include(x => x.Quotes).First(x => x.Id == 2);
                if (Quotes.Quotes == null)
                    Quotes.Quotes = new List<Quote>();
                if (!Quotes.Quotes.Any(x => x.Text == quote))
                {
                    var q = new Quote(quote);
                    Quotes.Quotes.Add(q);
                    _ctx.Entry(Quotes).State = EntityState.Modified;
                    _ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
        }


        [HttpGet("GetDataFromSpotify")]
        public string GetData()
        {
            MainParser parser = new MainParser();
            var artist = parser.Bind();
            var Artist = _ctx.Artists.Include(x => x.Albums).ThenInclude(x => x.Songs).FirstOrDefault(x => x.SpotifyId == artist.SpotifyId);
            if (Artist == null)
            {
                _ctx.Artists.Add(artist);
                _ctx.SaveChanges();
            }
            else
            {
                foreach (var spotAlbum in artist.Albums)
                {
                    var tempAlbum = _ctx.Albums.FirstOrDefault(x => x.SpotifyId == spotAlbum.SpotifyId);
                    if (tempAlbum == null)
                    {
                        Artist.Albums.Add(tempAlbum);
                        _ctx.Entry(tempAlbum).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                    else
                    {
                        var AlbumWithSongs = _ctx.Albums.Include(x => x.Songs).FirstOrDefault(x => x.SpotifyId == spotAlbum.SpotifyId);
                        foreach (var spotSong in spotAlbum.Songs)
                        {
                            if (!AlbumWithSongs.Songs.Any(x => x.SpotifyId == spotSong.SpotifyId))
                            {
                                AlbumWithSongs.Songs.Add(spotSong);
                                _ctx.Entry(AlbumWithSongs).State = EntityState.Modified;
                                _ctx.SaveChanges();
                            }
                        }
                    }
                }
            }
            return null;
        }

        [HttpGet]
        public void Get()
        {
            try
            {
                var albums = _ctx.Albums.Include(x => x.Songs).ToList();
                foreach (var album in albums)
                {
                }
                //    var image = album.Images.OrderBy(x => x.Width).First().Url;
                //    foreach (var song in album.Songs)
                //    {
                //        if (song.Image == null)
                //        {
                //            var artist = JsonConvert.DeserializeObject<List<Artist>>(song.Artists);
                //            song.Artists = JsonConvert.SerializeObject(artist.Select(x => x.Name).ToList());
                //            song.Image = image;
                //            List<string> genres = new List<string>();

                //            string tempGendres = "";
                //            foreach (var art in artist.Select(x => x.Name).ToList())
                //            {
                //                List<string> g = new List<string>();
                //                foreach (var z in _ctx.Artists.Where(x => x.Name.Contains(art)).Select(x => x.Genres))
                //                {
                //                    if (!string.IsNullOrEmpty(z))
                //                        foreach (var y in JsonConvert.DeserializeObject<List<string>>(z))
                //                        {
                //                            if (!g.Any(x => x == y))
                //                            {
                //                                g.Add(y);
                //                            }
                //                        }
                //                }
                //                song.Genres = JsonConvert.SerializeObject(g);
                //            }

                //            _ctx.Entry(song).State = EntityState.Modified;
                //            _ctx.SaveChanges();
                //        }
                //    }
                //}
            }
            catch (Exception e)
            {

            }
        }
    }
}