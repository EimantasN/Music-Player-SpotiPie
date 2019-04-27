using Config.Net;
using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.BackEnd;
using Service.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly CancellationTokenSource cts;
        private readonly ISongService _song;
        private readonly SpotyPieIDbContext _ctx;
        private CancellationToken ct;
        private readonly IDb _ctd;
        private ISettings settings;

        public InfoController(IDb ctd, SpotyPieIDbContext ctx, ISongService song)
        {
            _ctx = ctx;
            _ctd = ctd;
            _song = song;
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
                return Ok(await _song.GetState());
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
                await _song.SetStateAsync(songId, artistId, albumId, playlistId);
                return Ok();
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
