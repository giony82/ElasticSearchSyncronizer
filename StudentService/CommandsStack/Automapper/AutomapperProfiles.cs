using AutoMapper;
using Common.Data;
using StudentService.Business.Models;

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
