using AutoMapper;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Business.Models;
using ElasticSearch.Repository.Entities;
using ElasticSearch.Repository.Interfaces;

namespace ElasticSearch.Business.Services
{
    public class StudentService:IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        public StudentModel Get(string id)
        {
            StudentEntity studentEntity = _studentRepository.Get(id);

            return _mapper.Map<StudentModel>(studentEntity);
        }
    }
}
