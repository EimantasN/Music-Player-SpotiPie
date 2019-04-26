using Config.Net;
using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Models.BackEnd;
using Service.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly CancellationTokenSource cts;
        private readonly SpotyPieIDbContext _ctx;
        private CancellationToken ct;
        private readonly IDb _ctd;
        private ISettings settings;

        public InfoController(IDb ctd, SpotyPieIDbContext ctx)
        {
            _ctx = ctx;
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
            settings = new ConfigurationBuilder<ISettings>()
                .UseJsonFile(Environment.CurrentDirectory + @"/settings.json")
                .Build();
        }

        [HttpGet("GetState")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetState()
        {
            try
            {
                return Ok(await _ctx.CurrentSong.FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("SetState")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> SetState(
            [FromForm] int songId, 
            [FromForm] int artistId, 
            [FromForm] int albumId, 
            [FromForm] int playlistId)
        {
            try
            {
                var currentSong = await _ctx.CurrentSong.FirstOrDefaultAsync();
                if (currentSong != null)
                {
                    currentSong.SongId = songId != 0 ? songId : currentSong.SongId;
                    currentSong.ArtistId = artistId != 0 ? artistId : currentSong.ArtistId;
                    currentSong.AlbumId = albumId != 0 ? albumId : currentSong.AlbumId;
                    currentSong.PlaylistId = playlistId != 0 ? playlistId : currentSong.PlaylistId;
                    _ctx.Entry(currentSong).State = EntityState.Modified;
                }
                else
                {
                    var model = new CurrentSong
                    {
                        SongId = songId,
                        AlbumId = albumId,
                        ArtistId = artistId,
                        PlaylistId = playlistId
                    };
                    await _ctx.CurrentSong.AddAsync(model);
                }
                await _ctx.SaveChangesAsync();
                return Ok(currentSong);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("Library")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetLibraryInformation(CancellationToken t)
        {
            try
            {
                return Ok(await _ctd.GetLibraryInfo());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("FileList")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetFileList(CancellationToken t)
        {
            try
            {
                return new JsonResult(await _ctd.GetAudioList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("SystemInformation")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetSysInformation(CancellationToken t)
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                {
                    var cpuUsage = _ctd.GetCPUUsage();
                    var cpuTemp = _ctd.GetCPUTemperature();
                    var ramUsage = _ctd.GetRAMUsage();
                    var dUsed = _ctd.GetUsedStorage();

                    return new JsonResult(new { cU = cpuUsage, cT = cpuTemp, rU = ramUsage, dU = dUsed });
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
