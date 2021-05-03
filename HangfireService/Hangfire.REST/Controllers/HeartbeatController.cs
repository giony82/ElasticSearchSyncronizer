using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchWebService.Controllers
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
