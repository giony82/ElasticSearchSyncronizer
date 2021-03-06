
using AutoMapper;
using Commands.Infrastructure.Interfaces;
using CommandsStack.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using StudentService.Business.Automapper;
using StudentService.Business.Events;
using StudentService.Repository;
using StudentService.Repository.Interfaces;

namespace StudentService.Business
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IStudentRepository, StudentRepository>();

            services.AddTransient<IEventHandler<StudentUpdated>, StudentChangeEventHandler>();
            services.AddTransient<IEventHandler<StudentCreated>, StudentChangeEventHandler>();

            services.AddTransient<IBus,Bus>();

            services.AddAutoMapper(typeof(AutoMapperProfiles));
        }
    }
}
