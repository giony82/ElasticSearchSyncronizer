using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolUtils;
using StudentService.Data;
using StudentService.Data.Models;
using StudentService.Repository.Interfaces;

namespace StudentService.Repository
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {       
        public StudentRepository(SchoolContext context) : base(context)
        {
        }

        public async Task<Student> GetByIdAsync(Guid id)
        {
            return await this.GetAll().FirstOrDefaultAsync(x => x.StudentId == id);
        }
    }
}
