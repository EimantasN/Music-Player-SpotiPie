using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.BackEnd;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly CancellationTokenSource cts;
        private CancellationToken ct;
        private readonly ISongService _songs;

        public SongsController(ISongService ctd)
        {
            _songs = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        [HttpGet("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var data = _songs.GetAsync(id);
                

                return Ok(song);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Search for songs with specified name
        [HttpPost("search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Bad search query");

                return Ok(await _songs.Search(query));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Songs")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetSongs()
        {
            try
            {
                var songs = await _songs.GetAllAsync();
                return Ok(songs);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                if (await _songs.RemoveAudio(id))
                    return Ok();
                else
                    return StatusCode(404);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("PopularRecent")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetMostPopularRecent()
        {
            try
            {

                var popular = await _ctx.Items.AsNoTracking()
                    .Select(x => new
                    {
                        x.Name,
                        x.ImageUrl,
                        x.Id,
                        Artist = JsonConvert.DeserializeObject<Artists[]>(x.Artists)[0].Name,
                        AlbumName = "",
                        x.Popularity
                    })
                    .OrderByDescending(x => x.Popularity).
                    FirstOrDefaultAsync();

                var recent = await _ctx.Items.AsNoTracking()
                    .Select(x => new
                    {
                        x.Name,
                        x.ImageUrl,
                        x.Id,
                        Artist = JsonConvert.DeserializeObject<Artists[]>(x.Artists)[0].Name,
                        AlbumName = "",
                        x.LastActiveTime
                    })
                    .OrderByDescending(x => x.LastActiveTime).
                    FirstOrDefaultAsync();

                if (popular != null && recent != null)
                {

                    var album = await _ctx.Albums.AsNoTracking()
                        .Include(x => x.Songs)
                        .Select(x => new { x.Name, x.Songs })
                        .FirstOrDefaultAsync(x => x.Songs.Exists(y => y.Id == popular.Id));

                    var albumRec = await _ctx.Albums.AsNoTracking()
                        .Include(x => x.Songs)
                        .Select(x => new { x.Name, x.Songs })
                        .FirstOrDefaultAsync(x => x.Songs.Exists(y => y.Id == recent.Id));

                    return Ok(new
                    {
                        Popular = new
                        {
                            popular.Name,
                            popular.ImageUrl,
                            popular.Id,
                            popular.Artist,
                            AlbumName = album.Name != null ? album.Name : ""
                        },
                        Recent = new
                        {
                            recent.Name,
                            recent.ImageUrl,
                            recent.Id,
                            recent.Artist,
                            AlbumName = albumRec.Name != null ? albumRec.Name : ""
                        }
                    });
                }

                return Ok(new
                {
                    Popular = new
                    {
                        Name = "",
                        ImageUrl = "",
                        Id = 0,
                        Artist = "",
                        AlbumName = ""
                    },
                    Recent = new
                    {
                        Name = "",
                        ImageUrl = "",
                        Id = 0,
                        Artist = "",
                        AlbumName = ""
                    }
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetSongAlbum/{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetSongAlbum(int id)
        {
            try
            {

                string query = "Select AlbumId as Id from Item where id = " + id;
                var albumid = await _ctx.Items.FromSql(query).Select(x => new { x.Id }).ToListAsync();

                Album al = await _ctx.Albums.AsNoTracking().Include(x => x.Images).FirstAsync(x => x.Id == albumid.First().Id);
                _ctx.Update(al);
                al.Popularity++;
                al.LastActiveTime = DateTime.Now;
                _ctx.SaveChanges();

                return Ok(al);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest(e.Message);
            }
        }

        [Route("{id}/Update")]
        [HttpGet]
        public async Task Update(int id)
        {
            try
            {
                var song = _ctx.Items  ///.Select(x => new { x.Id, x.Artists, x.DurationMs, x.IsPlayable, x.Name })
                    .First(x => x.Id == id);

                song.LastActiveTime = DateTime.Now;
                _ctx.Entry(song).State = EntityState.Modified;
                _ctx.SaveChanges();
            }
            catch (System.Exception ex)
            {
            }
        }
    }
}
