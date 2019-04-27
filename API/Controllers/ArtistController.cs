using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {

        private readonly IArtistService _artists;

        public ArtistController(IArtistService ctx)
        {
            _artists = ctx;
        }

        //Search for artists with specified name
        [HttpPost("Search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromForm] string query)
        {
            try
            {
                return Ok(await _artists.SearchAsync(query));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // Get artist list
        [HttpGet("All")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtists()
        {
            try
            {
                return Ok(await _artists.GetArtistsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get artist list
        [HttpGet("Albums/{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtistAlbums(int id)
        {
            try
            {
                return Ok(await _artists.GetArtistAlbum(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get only artist info and images
        [HttpGet("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtist(int id)
        {
            try
            {
                return Ok(await _artists.GetArtistAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Return artist top 15 tracks
        [Route("{id}/Top-tracks")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtistTopTracks(int id)
        {
            try
            {
                return Ok(await _artists.GetArtistTopTracksAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Route("Related/{id}")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetRelatedArtists(int id)
        {
            try
            {
                return Ok(await _artists.GetRelatedArtistsAsync(id));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("Popular")]
        [HttpGet]
        public async Task<IActionResult> GetPopularArtists()
        {
            try
            {
                return Ok(await _artists.GetPopularArtistsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
