using System;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebApi.Controllers
{
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Route("api")]
        public ActionResult Get()
        {
            return Ok(string.Format("ChatWebApi is running. Now: {0}.", DateTime.Now));
        }
    }
}
