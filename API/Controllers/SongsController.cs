using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songs;

        public SongsController(ISongService ctd)
        {
            _songs = ctd;
        }

        [HttpGet("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await _songs.GetAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Bad search query");

                return Ok(await _songs.SearchAsync(query));
            }
            catch (Exception e)
            {
                return BadRequest(e);
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
            catch (Exception ex)
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
                await _songs.RemoveAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Recent")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetRecent()
        {
            try
            {
                return Ok(await _songs.GetRecentAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpGet("Popular")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetPopular()
        {
            try
            {
                return Ok(await _songs.GetPopularAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetSongAlbum/{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetSongAlbum(int id)
        {
            try
            {
                return Ok(await _songs.GetSongsByAlbumAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
