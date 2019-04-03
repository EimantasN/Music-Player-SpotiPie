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
    public class ArtistController : ControllerBase
    {

        private readonly SpotyPieIDbContext _ctx;
        private readonly CancellationTokenSource cts;
        private CancellationToken ct;
        private readonly IDb _ctd;

        public ArtistController(SpotyPieIDbContext ctx, IDb ctd)
        {
            _ctx = ctx;
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        //Search for artists with specified name
        [HttpPost("search")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest("Bad search query");

                var artists = await Task.Factory.StartNew(() =>
                {
                    return _ctx.Artists
                    .AsNoTracking()
                    .Include(x => x.Images)
                    .Include(x => x.Songs)
                    .Where(x => x.Name.Contains(query));
                });

                return Ok(artists);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // Get artist list
        [HttpGet("Artists")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtists()
        {
            try
            {
                var artists = await _ctx.Artists.Include(x => x.Images).ToListAsync();

                return Ok(artists);
            }
            catch (System.Exception ex)
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
                var artist = await _ctx.Artists.Include(x => x.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return Ok(artist);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        //Return artist top 15 tracks
        [Route("{id}/top-tracks")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtistTopTracks(int id)
        {
            try
            {
                //TODO add popularity option and order by that
                var artist = await _ctx.Artists.Include(x => x.Images).Include(x => x.Songs)
                    .Select(x => new { x.Id, x.Songs, x.Popularity })
                    .OrderByDescending(x => x.Popularity)
                    .FirstOrDefaultAsync(x => x.Id == id);

                return Ok(artist.Songs);
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
                var artist = await _ctx.Artists.FirstAsync(x => x.Id == id);
                var GenresList = JsonConvert.DeserializeObject<List<string>>(artist.Genres);
                List<Artist> RelatedArtist = new List<Artist>();
                foreach (var a in GenresList)
                {
                    var artists = await _ctx.Artists.AsNoTracking().Include(x => x.Images).Where(x => x.Id != id && x.Genres.Contains(a)).ToListAsync();
                    RelatedArtist.AddRange(artists);

                    if (RelatedArtist.Count >= 6)
                        break;
                }

                return Ok(RelatedArtist);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetAllArtists()
        {
            try
            {
                var artists = await _ctd.GetArtistList();
                return Ok(artists);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("Popular/")]
        [HttpGet]
        public async Task<IActionResult> GetPopularArtists()
        {
            try
            {
                var data = await _ctx.Artists.Include(x => x.Images).Select(x => new
                {
                    x.Id,
                    x.Popularity,
                    x.Name,
                    x.Genres,
                    x.Images
                }).
                OrderByDescending(x => x.Popularity).Take(6).ToListAsync();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("{id}/Albums")]
        [HttpGet]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetArtistAlbums(int id)
        {
            try
            {
                var data = await _ctx.Artists.Include(x => x.Images).Include(x => x.Albums).ThenInclude(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
