using System;
using System.Threading.Tasks;
using Commands.Infrastructure.Interfaces;
using Common.Events;
using Redis.Interfaces;
using SchoolUtils;
using Serilog;
using StudentService.Business;

namespace School.EventHandlers
{
    public class StudentChangeEventHandler :IEventHandler<StudentUpdated>, IEventHandler<StudentCreated>
    {
        private readonly IRedisService _redisService;
        private readonly IAppSettings _appSettings;
        private static readonly ILogger Logger = Log.ForContext<StudentChangeEventHandler>();

        public StudentChangeEventHandler(IRedisService redisService, IAppSettings appSettings)
        {
            _redisService = redisService;
            _appSettings = appSettings;
        }

        public void Handle(StudentUpdated @event)
        {
            Logger.Debug($"StudentUpdated event triggered for {@event.StudentId}");

            HandleInternal(@event.StudentId);
        }

        public void Handle(StudentCreated @event)
        {
            Logger.Debug($"StudentCreated event triggered for {@event.StudentId}");

            HandleInternal(@event.StudentId);
        }

        private void HandleInternal(string id)
        {
            bool isElasticSearchSyncEnabled = _appSettings.Get<bool>(Constants.EnableElasticSearchSync);

            if (!isElasticSearchSyncEnabled)
            {
                return;
            }

            Task.Run(() =>
              {
                  try
                  {
                      _redisService.AddToSet(id, Constants.StudentDocument);

                      Logger.Debug($"StudentChange event triggered for {id}");
                  }
                  catch (Exception e)
                  {
                      Logger.Error(e, $"Could not handle the student changes for {id}");
                  }
              });
        }     
    }
}