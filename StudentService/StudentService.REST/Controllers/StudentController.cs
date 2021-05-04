using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StudentService.Business.Interfaces;
using StudentService.Business.Models;

namespace StudentService.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static readonly ILogger Logger = Log.ForContext<StudentController>();
        private readonly IMapper mapper;
        private readonly IStudentService studentService;

        public StudentController(IStudentService studentService, IMapper mapper)
        {
            this.studentService = studentService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateStudentViewModel student)
        {
            Logger.Debug($"Post called with {student.Name}.");

            Guid id = await studentService.CreateAsync(mapper.Map<CreateStudentModel>(student));

            return StatusCode(StatusCodes.Status201Created, id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(StudentViewModel student)
        {
            Logger.Debug($"Put called with {student.StudentId}");

            var updated = await studentService.UpdateAsync(mapper.Map<StudentModel>(student));

            if (updated)
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Logger.Debug($"Delete called with {id}.");

            var deleted = await studentService.DeleteAsync(id);

            if (deleted)
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            Logger.Debug("Get called with {id}.");

            StudentModel studentModel = await studentService.GetAsync(id);

            if (studentService == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<StudentViewModel>(studentModel));
        }
    }
}