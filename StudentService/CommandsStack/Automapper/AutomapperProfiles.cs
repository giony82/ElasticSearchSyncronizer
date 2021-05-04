using AutoMapper;
using StudentService.Business.Models;
using StudentService.Data.Models;

namespace StudentService.Business.Automapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<StudentModel, Student>().ReverseMap();
        }
    }
}
