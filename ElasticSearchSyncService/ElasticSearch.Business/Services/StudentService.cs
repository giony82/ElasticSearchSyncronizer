using AutoMapper;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Business.Models;
using ElasticSearch.Repository.Entities;
using ElasticSearch.Repository.Interfaces;

namespace ElasticSearch.Syncronizer
{
    public class StudentService:IStudentService
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
        }

        public StudentModel Get(string id)
        {
            StudentEntity studentEntity = this.studentRepository.Get(id);

            return this.mapper.Map<StudentModel>(studentEntity);
        }
    }
}
