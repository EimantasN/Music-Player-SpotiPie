using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IErrorService _error;

        public ReportsController(IErrorService error)
        {
            _error = error;
        }

        [HttpPost]
        public async Task<IActionResult> ReportAsync([FromForm] string message, [FromForm] string method)
        {
            try
            {
                return Ok(await _error.ReportAsync(message, method));
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}