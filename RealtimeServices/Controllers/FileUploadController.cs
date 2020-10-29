using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealtimeServices.Hubs;
using RealtimeServices.Models;

namespace RealtimeServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IHubContext<ChatHub> hubContext;

        public FileUploadController(IHubContext<ChatHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost]
        [Route("UploadFileByJS")]
        public async Task<IActionResult> UploadByJS([FromBody] ImageJsonDTO jsonDTO)
        {
            if (ModelState.IsValid)
            {
                if (jsonDTO.formFile == null || jsonDTO.formFile.Length == 0)
                    return Content("file not selected");
                //hier optioneel de file bewaren via een repo
                await hubContext.Clients.All.SendAsync("UserImage", jsonDTO.formFile);
            }
            return Accepted(new { body = jsonDTO });
        }




    }
}
