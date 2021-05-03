using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Models;
using StudentService.Business.Models;

namespace StudentService.REST.Automapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<StudentModel, StudentViewModel>().ReverseMap();
            CreateMap<CreateStudentModel, CreateStudentViewModel>().ReverseMap();
        }
    }
}
