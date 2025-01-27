using Microsoft.AspNetCore.Mvc;

namespace StrikerBot.API.Misc.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiscController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Hello, World");
        }

        [HttpGet("myip")]
        public IActionResult MyIP()
        {
            string ipAddress = Request?.Headers["CF-CONNECTING-IP"];

            if (ipAddress == null)
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            if (ipAddress == null)
            {
                return BadRequest("400: Bad Request (cannot read ip)");
            }

            return Ok(ipAddress);
        }
    }
}
