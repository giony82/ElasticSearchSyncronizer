using System.Threading.Tasks;
using ElasticSearch.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElasticSearchWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {      
        private readonly ILogger<StudentController> logger;
        private readonly IStudentSyncronizer studentSyncronizer;

        public StudentController(ILogger<StudentController> logger, IStudentSyncronizer studentSyncronizer)
        {
            this.logger = logger;
            this.studentSyncronizer = studentSyncronizer;
        }

        [HttpPost]
        [Route("syncronize")]
        public async Task<IActionResult> Post()
        {
            this.logger.LogDebug("Post called");

            await this.studentSyncronizer.ExecuteAsync();

            return Ok();
        }
    }
}
