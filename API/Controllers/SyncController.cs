using API.SpotifyAPI;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Newtonsoft.Json;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("CorectDatabase")]
        public async Task<ActionResult> CorectDatabaseAsync()
        {
            try
            {
                var songs = _ctx.Songs.ToList();
                foreach (var x in songs)
                {
                    if (x.LocalUrl != null && x.LocalUrl.Contains("C:"))
                        x.LocalUrl = null;

                    if (string.IsNullOrEmpty(x.LocalUrl))
                    {
                        x.IsLocal = false;
                        x.IsPlayable = false;
                    }
                    else
                    {
                        x.IsPlayable = true;
                    }
                    _ctx.Entry(x).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("FFpeg_test")]
        public ActionResult FFpeg()
        {
            try
            {
                string path =
                    System.IO.Path.DirectorySeparatorChar + "root" +
                    System.IO.Path.DirectorySeparatorChar + "Content" +
                    System.IO.Path.DirectorySeparatorChar + "Flac";

                Ffmpeg.ConvertFile(path + Path.DirectorySeparatorChar, "unintended.flac", "unintended2.flac");

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("GetQuote/{id}")]
        public async Task<IActionResult> AddedQuoteAsync(int id)
        {
            try
            {
                return Ok(await _ctx.Quotes.FirstOrDefaultAsync(x => x.Id == id));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("AddQuote")]
        public async Task<IActionResult> AddedQuote([FromForm] string quote)
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
                    await _ctx.SaveChangesAsync();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost("GetDataFromSpotify")]
        public IActionResult GetData([FromForm] string artistId, [FromForm] string token)
        {
            try
            {
                MainParser parser = new MainParser(artistId, token);
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
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}