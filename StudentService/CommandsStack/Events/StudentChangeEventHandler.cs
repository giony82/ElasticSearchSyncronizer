using System;
using System.Threading.Tasks;
using Commands.Infrastructure.Interfaces;
using Redis.Interfaces;
using SchoolUtils;
using Serilog;

namespace StudentService.Business.Events
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

        /// <summary>
        /// Handles student updated event. In our case it does not matter if it's updated or created, but this is done here just to show
        /// how to differentiate these events.
        /// </summary>
        public void Handle(StudentUpdated @event)
        {
            Logger.Debug($"StudentUpdated event triggered for {@event.StudentId}.");

            HandleInternal(@event.StudentId);
        }

        /// <summary>
        /// Handles student created event. In our case it does not matter if it's updated or created, but this is done here just to show
        /// how to differentiate these events.
        /// </summary>
        public void Handle(StudentCreated @event)
        {
            Logger.Debug($"StudentCreated event triggered for {@event.StudentId}.");

            HandleInternal(@event.StudentId);
        }

        private void HandleInternal(string id)
        {
            var isElasticSearchSyncEnabled = _appSettings.Get<bool>(Constants.EnableElasticSearchSync);

            if (!isElasticSearchSyncEnabled)
            {
                Logger.Debug("Sync event is disabled via env settings!");

                return;
            }

            Task.Run(() =>
              {
                  try
                  {
                      _redisService.AddToSet(id, Constants.StudentDocument);

                      Logger.Debug($"StudentChange event triggered for {id}.");
                  }
                  catch (Exception e)
                  {
                      Logger.Error(e, $"Could not handle the student changes for {id}.");
                  }
              });
        }     
    }
}