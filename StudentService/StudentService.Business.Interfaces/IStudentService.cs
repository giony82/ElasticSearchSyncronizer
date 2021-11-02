using System;
using System.Threading.Tasks;
using StudentService.Business.Models;

namespace StudentService.Business.Interfaces
{
    public interface IStudentService
    {
        Task<Guid> CreateAsync(CreateStudentModel studentModel);
        Task<bool> DeleteAsync(Guid id);
        Task<StudentModel> GetAsync(Guid id);
        Task<bool> UpdateAsync(StudentModel student);
        Task<bool> IncrementScoreAsync(Guid id, int value);
    }
}
