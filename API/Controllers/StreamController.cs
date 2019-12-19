using Config.Net;
using Database;
using Microsoft.AspNetCore.Mvc;
using Service.Helpers;
using Service.Settings;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        private readonly CancellationTokenSource cts;
        private CancellationToken ct;
        private readonly IDb _ctd;
        private ISettings settings;

        public StreamController(IDb ctd)
        {
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
            //settings = new ConfigurationBuilder<ISettings>()
            //    .UseJsonFile(Environment.CurrentDirectory + @"/settings.json")
            //    .Build();
        }

        [HttpGet("Test")]
        public IActionResult Test(CancellationToken t)
        {
            return Ok(settings.StreamQuality);
        }

        [HttpPost("play/{id}")]
        public async Task<IActionResult> GetMusic([FromBody] MusicPlayPost data, CancellationToken t, int id)
        {
            try
            {
                t.ThrowIfCancellationRequested();
                var path = await _ctd.GetAudioPathById(id);

                if (!string.IsNullOrWhiteSpace(path))
                {
                    //if (settings != null && settings.StreamQuality < 1000)
                    //{
                    //    if (await _ctd.SetAudioPlaying(id, data.ArtistId, data.AlbumId, data.PlaylistId))
                    //    {
                    //        var qualityPath = _ctd.ConvertAudio(path, settings.StreamQuality);
                    //        if (string.IsNullOrWhiteSpace(qualityPath))
                    //            return _ctd.OpenFile(path, out FileStream fs)
                    //                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                    //                : (IActionResult)BadRequest();
                    //        else
                    //            return _ctd.OpenFile(qualityPath, out FileStream fs)
                    //                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                    //                : (IActionResult)BadRequest();
                    //    }
                    //}
                    //else
                    //{
                        if (await _ctd.SetAudioPlaying(id, data.ArtistId, data.AlbumId, data.PlaylistId))
                        {
                            return _ctd.OpenFile(path, out FileStream fs)
                                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                                : (IActionResult)BadRequest();
                        }
                    //}

                    return BadRequest("Cannot find path specified/File not playable");
                }

                return BadRequest("Cannot find path specified/File not playable");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }

        [HttpGet("play/{id}")]
        public async Task<IActionResult> GetMusic(CancellationToken t, int id)
        {
            try
            {
                t.ThrowIfCancellationRequested();
                var path = await _ctd.GetAudioPathById(id);

                if (!string.IsNullOrWhiteSpace(path))
                {
                    //if (settings != null && settings.StreamQuality < 1000)
                    //{
                    //    if (await _ctd.SetAudioPlaying(id, 0, 0, 0))
                    //    {
                    //        var qualityPath = _ctd.ConvertAudio(path, settings.StreamQuality);
                    //        if (string.IsNullOrWhiteSpace(qualityPath))
                    //            return _ctd.OpenFile(path, out FileStream fs)
                    //                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                    //                : (IActionResult)BadRequest();
                    //        else
                    //            return _ctd.OpenFile(qualityPath, out FileStream fs)
                    //                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                    //                : (IActionResult)BadRequest();
                    //    }
                    //}
                    //else
                    //{
                        if (await _ctd.SetAudioPlaying(id, 0, 0, 0))
                        {
                            return _ctd.OpenFile(path, out FileStream fs)
                                ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                                : (IActionResult)BadRequest();
                        }
                    //}

                    return BadRequest("Cannot find path specified/File not playable");
                }

                return BadRequest("Cannot find path specified/File not playable");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }

        [HttpGet("{file}")]
        public IActionResult GetMusic(CancellationToken t, string file)
        {
            try
            {
                t.ThrowIfCancellationRequested();
                string aPath = settings != null ? settings.AudioStoragePath : "/root/Music/";
                return _ctd.OpenFile(aPath + file, out FileStream fs)
                    ? File(fs, new MediaTypeHeaderValue("audio/mpeg").MediaType, true)
                    : (IActionResult)BadRequest();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }
    }
}
