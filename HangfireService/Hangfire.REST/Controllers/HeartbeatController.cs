using Microsoft.AspNetCore.Mvc;

namespace Hangfire.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController : ControllerBase
    {                    
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
