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

        [HttpGet]
        public void Get()
        {
            try
            {
                var albums = _ctx.Albums.Include(x => x.Images).Include(x => x.Songs).ToList();
                foreach (var album in albums)
                {
                    var image = album.Images.OrderBy(x => x.Width).First().Url;
                    foreach (var song in album.Songs)
                    {
                        if (song.Image == null)
                        {
                            var artist = JsonConvert.DeserializeObject<List<Artist>>(song.Artists);
                            song.Artists = JsonConvert.SerializeObject(artist.Select(x => x.Name).ToList());
                            song.Image = image;
                            List<string> genres = new List<string>();

                            string tempGendres = "";
                            foreach (var art in artist.Select(x => x.Name).ToList())
                            {
                                List<string> g = new List<string>();
                                foreach (var z in _ctx.Artists.Where(x => x.Name.Contains(art)).Select(x => x.Genres))
                                {
                                    if (!string.IsNullOrEmpty(z))
                                        foreach (var y in JsonConvert.DeserializeObject<List<string>>(z))
                                        {
                                            if (!g.Any(x => x == y))
                                            {
                                                g.Add(y);
                                            }
                                        }
                                }
                                song.Genres = JsonConvert.SerializeObject(g);
                            }

                            _ctx.Entry(song).State = EntityState.Modified;
                            _ctx.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}