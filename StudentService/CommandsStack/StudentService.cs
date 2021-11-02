using System;
using System.Threading.Tasks;
using AutoMapper;
using CommandsStack.Infrastructure;
using Redis.Interfaces;
using StudentService.Business.Events;
using StudentService.Business.Interfaces;
using StudentService.Business.Models;
using StudentService.Data.Models;
using StudentService.Repository.Interfaces;

namespace StudentService.Business
{
    public class StudentService: IStudentService
    {
        private readonly IStudentRepository studentRepository;        
        private readonly IBus eventSource;
        private readonly IMapper mapper;
        private readonly IRedisService _redisService;
        private readonly IMapper mapper1;

        public StudentService(IStudentRepository studentRepository, IBus eventSource, IMapper mapper, IRedisService redisService)
        {
            this.studentRepository = studentRepository;            
            this.eventSource = eventSource;
            this.mapper = mapper;
            _redisService = redisService;
        }

        public async Task<Guid> CreateAsync(CreateStudentModel studentModel)
        {
            var studentEntity = new Student()
            {
                Name = studentModel.Name
            };

            await this.studentRepository.AddAsync(studentEntity);
                        
            this.eventSource.Handle(new StudentCreated(studentEntity.StudentId.ToString()));

            return studentEntity.StudentId;
        }

        public async Task<StudentModel> GetAsync(Guid id)
        {
            Student studentEntity = await this.studentRepository.GetByIdAsync(id);
            if(studentEntity==null)
            {
                return null;
            }

            return this.mapper.Map<StudentModel>(studentEntity);            
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Student studentEntity = await this.studentRepository.GetByIdAsync(id);
            if (studentEntity == null)
            {
                return false;
            }

            await this.studentRepository.DeleteAsync(studentEntity);

            this.eventSource.Handle(new StudentUpdated(studentEntity.StudentId.ToString()));

            return true;
        }

        public async Task<bool> UpdateAsync(StudentModel student)
        {
            Student studentEntity = await this.studentRepository.GetByIdAsync(student.StudentId);
            if (studentEntity == null)
            {
                return false;
            }

            studentEntity.Name = student.Name;

            await this.studentRepository.UpdateAsync(studentEntity);

            this.eventSource.Handle(new StudentUpdated(studentEntity.StudentId.ToString()));

            return true;
        }

        public async Task<bool> IncrementScoreAsync(Guid id, int value)
        {
            var key = $"student-increment{id}";
            try
            {
                _redisService.AcquireLock(key);

                var updated = await this.studentRepository.IncrementScoreAsync(id, value);
                
                return updated;
            }
            finally
            {
                _redisService.ReleaseLock(key);
            }
        }
    }
}
