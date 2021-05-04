using System.Threading.Tasks;
using ElasticSearch.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElasticSearchSync.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {      
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentSynchronizer _studentSynchronizer;

        public StudentController(ILogger<StudentController> logger, IStudentSynchronizer studentSynchronizer)
        {
            this._logger = logger;
            this._studentSynchronizer = studentSynchronizer;
        }

        [HttpPost]
        [Route("synchronize")]
        public async Task<IActionResult> Post()
        {
            _logger.LogDebug("Synchronizing students.");

            await this._studentSynchronizer.ExecuteAsync();

            return Ok();
        }
    }
}
