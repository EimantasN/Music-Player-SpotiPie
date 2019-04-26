using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.BackEnd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static API.Controllers.UploadController;

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

        [HttpPost("Update/{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                return Ok(await _songs.UpdateAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Corrupted/{id}")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Corrupted(int id)
        {
            try
            {
                await _songs.SetCorruptedAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //[HttpGet("CurrentSong")]
        //public async Task<IActionResult> GetCurrentSong()
        //{
        //    try
        //    {
        //        Song song = await _songs.GetCurrent();
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e);
        //    }
        //}

        [HttpPost("SetSongLenght")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> SetSongLenght([FromForm]int id, [FromForm] long lenght)
        {
            try
            {
                return Ok(await _songs.SetLenght(id, lenght));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromForm] string query)
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

        [HttpGet("All")]
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

        [RequestSizeLimit(500000000)]
        [DisableFormValueModelBinding]
        [HttpPost]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Post()
        {
            try
            {
                Dictionary<string, string> results = new Dictionary<string, string>();

                if (Request.HasFormContentType)
                {
                    var form = Request.Form;

                    foreach (var formFile in form.Files)
                    {
                        return Ok(await _songs.AddAudioToLibrary(formFile));
                    }
                }
                return new JsonResult(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("BindFiles")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> BindFiles()
        {
            try
            {
                return Ok(await _songs.BindData());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
