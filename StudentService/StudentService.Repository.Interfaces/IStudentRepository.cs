using System;
using System.Threading.Tasks;
using SchoolUtils;
using StudentService.Data.Models;

namespace StudentService.Repository.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetByIdAsync(Guid id);
        Task<bool> IncrementScoreAsync(Guid id, int value);
    }
}
