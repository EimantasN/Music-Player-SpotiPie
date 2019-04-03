using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _album;

        public AlbumController(IAlbumService ctx)
        {
            _album = ctx;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //_ctd.Start();
            return Ok();
        }

        //Search for albums with specified name
        [HttpPost("search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Bad search query");

                return Ok(await _album.Search(query));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Returns full Album info without songs
        [HttpGet("{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetAlbum(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Id can't be " + id);

                return Ok(await _album.GetAlbumAsync(id));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Returns album list
        [HttpGet("Albums")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetAlbums([FromBody] int count = 10)
        {
            try
            {
                return Ok(await _album.GetAlbumsAsync(count));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Return 6 most recent albums
        [Route("Recent")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetRecentAlbums()
        {
            try
            {
                return Ok(await _album.GetRecentAlbumsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Return 6 most popular albums
        [Route("Popular")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetPopularAlbums()
        {
            try
            {
                return Ok(await _album.GetPopularAlbumsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Return 6 oldes albums
        [Route("Old")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetOldAlbums()
        {
            try
            {
                return Ok(await _album.GetOldAlbumsAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("artist/{artistId}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetAlbumsByArtist(int artistId)
        {
            try
            {
                return Ok(await _album.GetAlbumsByArtistAsync(artistId));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
