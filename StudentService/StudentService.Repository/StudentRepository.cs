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
        private SchoolContext _context;

        public StudentRepository(SchoolContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Student> GetByIdAsync(Guid id)
        {
            return await GetAll().Include(x => x.StudentProfile).FirstOrDefaultAsync(x => x.StudentId == id);
        }

        public async Task<bool> IncrementScoreAsync(Guid id, int value)
        {
            Student student = await GetByIdAsync(id);

            if (student != null)
            {
                if (student.StudentProfile == null)
                    student.StudentProfile = new StudentProfile
                    {
                        StudentId = student.StudentId,
                        Score = value
                    };
                else
                    student.StudentProfile.Score += value;


                await UpdateAsync(student);
                return true;
            }

            return false;
        }
    }
}