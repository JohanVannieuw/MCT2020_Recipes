using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantServices.Services;

namespace RestaurantServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageNoSQLController : ControllerBase
    {
        private readonly IFileHandlers fileHandlers;

        public ImageNoSQLController(IFileHandlers fileHandlers)
        {
            this.fileHandlers = fileHandlers;
        }

        [HttpPost]
        [Route("UploadImg")]
        public async Task<IActionResult> UploadImage(IFormFile formFile)
        {
            ObjectResult result = await fileHandlers.UploadImageAsync(formFile) as  ObjectResult;
            return Ok(new { file = formFile.FileName + " -> " + result.Value });
        }

        [HttpGet]
        [Route("DownloadImg")]
        public async Task<IActionResult> DownloadImageAsync(string id)
        {
            var stream = await fileHandlers.DownloadImageAsync(id);
            return File(stream, stream.FileInfo.Metadata["contentType"].AsString);
        }

    }
}
