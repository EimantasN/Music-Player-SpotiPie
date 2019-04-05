using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {

        private readonly SpotyPieIDbContext _ctx;
        private readonly CancellationTokenSource cts;
        private CancellationToken ct;
        private readonly IDb _ctd;

        public PlaylistController(SpotyPieIDbContext ctx, IDb ctd)
        {
            _ctx = ctx;
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        //Search for playlists with specified name
        [HttpPost("/search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Bad search query");

                var albums = await Task.Factory.StartNew(() =>
                {
                    return _ctx.Playlist
                    .AsNoTracking()
                    .Include(x => x.Songs)
                    .Where(x => x.Name.Contains(query));
                });

                return Ok(albums);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Returns full playlist info without songs
        [HttpGet("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetPlaylist(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id can't be " + id);

                //Need includes
                var playlist = await _ctx.Playlist
                    .AsNoTracking()
                    .Include(x => x.Songs)
                    .FirstOrDefaultAsync(x => x.Id == id);

                _ctx.Update(playlist);
                playlist.Image = playlist.Songs[new Random((int)DateTime.Now.Ticks).Next(0, playlist.Songs.Count)].LargeImage;
                playlist.LastActiveTime = DateTime.Now;
                playlist.Popularity++;
                _ctx.SaveChanges();

                playlist.Songs = null;

                return Ok(playlist);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Returns full playlist info with songs
        [HttpGet("{id}/tracks")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetPlaylistSongs(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id can't be " + id);

                //Need includes
                var playlist = await _ctx.Playlist
                    .AsNoTracking()
                    .Include(x => x.Songs)
                    .FirstOrDefaultAsync(x => x.Id == id);

                _ctx.Update(playlist);
                playlist.Image = playlist.Songs[new Random((int)DateTime.Now.Ticks).Next(0, playlist.Songs.Count)].LargeImage;
                playlist.LastActiveTime = DateTime.Now;
                playlist.Popularity++;
                _ctx.SaveChanges();

                return Ok(playlist);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Returns playlist list
        [HttpGet("playlists")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetPlaylists()
        {
            try
            {
                var playlists = await _ctx.Playlist
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(playlists);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Create playlist without songs
        //        {
        //          "name": "Testas"
        //        }
        //Create playlist with songs
        //        {
        //          "name": "Testas"
        //          "Songs": [{"id": "<Song ID>"}]
        //        }
        [HttpPost()]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> CreatePlaylist([FromBody] Playlist playlist)
        {
            try
            {
                var exPlaylist = await _ctx.Playlist.FirstOrDefaultAsync(x => x.Name == playlist.Name);
                if (exPlaylist != null)
                    return BadRequest("Playlist with this name already exists");

                playlist.Created = DateTime.Now;
                playlist.LastActiveTime = DateTime.Now;

                if (playlist.Songs.Count > 0)
                {
                    for (int i = 0; i < playlist.Songs.Count; i++)
                    {
                        var sg = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == playlist.Songs[i].Id);
                        playlist.Songs[i] = sg;
                        playlist.Total++;
                    }

                    playlist.Image = playlist.Songs[0].LargeImage;
                }

                _ctx.Playlist.Add(playlist);
                _ctx.SaveChanges();

                return Ok();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Add track to the playlist
        [HttpPost("{id}/tracks")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> AddToPlaylist(int id, [FromBody] int trackID)
        {
            if (id < 0 || trackID < 0)
                return BadRequest("Bad track/playlist ID");

            try
            {
                var playlist = await _ctx.Playlist.Include(x => x.Songs).FirstOrDefaultAsync(x => x.Id == id);
                if (playlist != null)
                {
                    var track = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == trackID);
                    if (track != null)
                    {
                        if (!playlist.Songs.Exists(x => x.Id == trackID))
                        {
                            _ctx.Update(playlist);

                            if (playlist.Image == null)
                                playlist.Image = track.LargeImage;

                            playlist.Songs.Add(track);
                            playlist.Total++;
                            _ctx.SaveChanges();

                            return Ok();
                        }

                        return BadRequest("Item already exists");
                    }
                    return StatusCode(500, "Can't find specified track");
                }
                return StatusCode(500, "Can't find specified playlist");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Delete playlist
        [HttpDelete("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            if (id < 0)
                return BadRequest("Bad playlist ID");

            try
            {
                var playlist = await _ctx.Playlist.FirstOrDefaultAsync(x => x.Id == id);
                if (playlist != null)
                {
                    _ctx.Playlist.Remove(playlist);
                    _ctx.SaveChanges();
                    return Ok();
                }
                return StatusCode(500, "Can't find specified playlist");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Delete track from the playlist
        [HttpDelete("{id}/tracks")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> DeleteFromPlaylist(int id, [FromBody] int trackID)
        {
            if (id < 0 || trackID < 0)
                return BadRequest("Bad track/playlist ID");

            try
            {
                var playlist = await _ctx.Playlist.Include(x => x.Songs).FirstOrDefaultAsync(x => x.Id == id);
                if (playlist != null)
                {
                    var track = await _ctx.Songs.FirstOrDefaultAsync(x => x.Id == id);
                    if (track != null)
                    {
                        _ctx.Update(playlist);
                        playlist.Songs.Remove(track);
                        playlist.Total--;
                        _ctx.SaveChanges();

                        return Ok();
                    }
                    return StatusCode(500, "Can't find specified track");
                }
                return StatusCode(500, "Can't find specified playlist");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
