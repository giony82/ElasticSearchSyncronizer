using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StudentService.Business.Interfaces;
using StudentService.Business.Models;

namespace Common.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class StudentController : ControllerBase
	{				
        private readonly IStudentService studentService;
        private readonly IMapper mapper;
        private static readonly ILogger Logger = Log.ForContext<StudentController>();

		public StudentController(IStudentService studentService, IMapper mapper)
		{			
            this.studentService = studentService;
            this.mapper = mapper;
        }

		[HttpPost]
		public async Task<IActionResult> Post(CreateStudentViewModel student)
		{
			Logger.Debug($"Post called with {student.Name}");

			Guid id = await this.studentService.CreateAsync(this.mapper.Map<CreateStudentModel>(student));			

			return StatusCode(StatusCodes.Status201Created, id);
		}

		[HttpPut]
		public async Task<IActionResult> Put(StudentViewModel student)
		{			
			Logger.Debug($"Put called with {student.StudentId}");

			bool result = await this.studentService.UpdateAsync(this.mapper.Map<StudentModel>(student));			

			if (result)
            {
				return StatusCode(StatusCodes.Status200OK);
			}
            else
            {
				return NotFound();			
            }			
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			Logger.Debug($"Delete called with {id}");

			bool result = await this.studentService.DeleteAsync(id);
			
			if (result)
			{
				return StatusCode(StatusCodes.Status200OK);
			}
			else
			{
				return NotFound();
			}
		}

		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> Get(Guid id)
		{
			Logger.Debug("Get called with {id}");

            StudentModel studentModel = await this.studentService.GetAsync(id);

			if(studentService==null)
            {
				return NotFound();
            }
			return Ok(this.mapper.Map<StudentViewModel>(studentModel));
		}

	}
}
