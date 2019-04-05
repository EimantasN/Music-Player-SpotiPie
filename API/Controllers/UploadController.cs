using Config.Net;
using Database;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Service.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly CancellationTokenSource cts;
        private CancellationToken ct;
        private readonly IDb _ctd;
        private ISettings settings;

        public UploadController(IDb ctd)
        {
            _ctd = ctd;
            cts = new CancellationTokenSource();
            ct = cts.Token;
            settings = new ConfigurationBuilder<ISettings>()
                .UseJsonFile(Environment.CurrentDirectory + @"/settings.json")
                .Build();
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
                        string filePath =
                            Path.DirectorySeparatorChar + "root" +
                            Path.DirectorySeparatorChar + "Content" +
                            Path.DirectorySeparatorChar + "Flac" +
                            Path.DirectorySeparatorChar + formFile.FileName;

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(fileStream);
                        }

                        if (System.IO.File.Exists(filePath))
                        {
                            if (await _ctd.AddAudioToLibrary(filePath, formFile.FileName, null))
                                results.Add(formFile.FileName, "Success");
                            else
                                results.Add(formFile.FileName, "Failed");
                        }
                        else
                            results.Add(formFile.FileName, "Failed");
                    }
                }
                return new JsonResult(results);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
        {
            public void OnResourceExecuting(ResourceExecutingContext context)
            {
                var factories = context.ValueProviderFactories;
                factories.RemoveType<FormValueProviderFactory>();
                factories.RemoveType<JQueryFormValueProviderFactory>();
            }

            public void OnResourceExecuted(ResourceExecutedContext context)
            {
            }
        }
    }
}
