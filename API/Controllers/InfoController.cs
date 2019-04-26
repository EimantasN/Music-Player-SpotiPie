using Config.Net;
using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
        private CancellationToken ct;
        private readonly IDb _ctd;
        private ISettings settings;

        public InfoController(IDb ctd)
        {
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
            settings = new ConfigurationBuilder<ISettings>()
                .UseJsonFile(Environment.CurrentDirectory + @"/settings.json")
                .Build();
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
