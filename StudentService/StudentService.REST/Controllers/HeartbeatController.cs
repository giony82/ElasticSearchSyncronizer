using Microsoft.AspNetCore.Mvc;

namespace StudentService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeartbeatController:ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
