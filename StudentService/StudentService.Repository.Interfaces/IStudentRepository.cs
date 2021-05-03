using System;
using System.Threading.Tasks;
using Common.Data;
using SchoolUtils;

namespace StudentService.Repository.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetByIdAsync(Guid id);
    }
}
