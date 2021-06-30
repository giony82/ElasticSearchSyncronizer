using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Elastic.Types;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Business.Models;
using ElasticSearch.Business.Types;
using Redis.Interfaces;
using SchoolUtils;
using Serilog;

namespace ElasticSearch.Business
{
    public class StudentSynchronizer : IStudentSynchronizer
    {
        private static readonly ILogger Logger = Log.ForContext<StudentSynchronizer>();
        private readonly ElasticSynchronizer _elasticSynchronizer;
        private readonly int _noOfRetries;
        private readonly IRedisService _redisService;
        private readonly IStudentService _studentService;

        public StudentSynchronizer(IStudentService studentService, IRedisService redisService,
            ElasticSynchronizer elasticSynchronizer, IAppSettings appSettings)
        {
            _studentService = studentService;
            _redisService = redisService;
            _elasticSynchronizer = elasticSynchronizer;

            _noOfRetries = appSettings.Get(BusinessConstants.ElasticSearchDocumentItemNoOfRetries,
                DefaultValues.ElasticSearchDocumentItemNoOfRetries);
        }

        public async Task ExecuteAsync()
        {
            while (await ExecuteInternal()) await Task.Delay(50);
        }

        private async Task<bool> ExecuteInternal()
        {
            var failedIds = new List<string>();
            var students = new List<StudentDocument>();

            try
            {
                students = await Get();

                if (students != null)
                {
                    failedIds = _elasticSynchronizer.Execute(students);

                    Logger.Debug($"Finish to synchronize {students.Count} students.");

                    return true;
                }

                Logger.Information("No students available!.");
            }
            catch (Exception e)
            {
                Logger.Error($"failed to run the student synchronizer job:{e}");

                HandleException(failedIds, students);
            }

            return false;
        }

        private void HandleException(ICollection<string> failedIds, List<StudentDocument> students)
        {
            try
            {
                students?.ForEach(x =>
                {
                    if (failedIds.Contains(x.Id))
                    {
                        if (x.NoOfRetry < _noOfRetries)
                        {
                            Logger.Warning($"Adding {x.Name} again to list. Current attempt {x.NoOfRetry}");

                            _redisService.AddToSet(x.Id, nameof(StudentDocument), x.NoOfRetry + 1);
                        }
                        else
                        {
                            Logger.Error($"Could not process {x.Id}. Sending it to DLQ");

                            _redisService.AddToSet(x.Id, nameof(StudentDocument) + "-failed");
                        }
                    }
                });
            }
            catch (Exception e)
            {
                // worst case scenario
                Logger.Error(e, "Could not add again students to redis set!");
            }
        }

        private async Task<List<StudentDocument>> Get()
        {
            var idsWithNoOfRetry = await _redisService.ExtractIdsFromSetWithScore(nameof(StudentDocument))
                .ConfigureAwait(false);

            if (idsWithNoOfRetry == null || idsWithNoOfRetry.Count == 0) return null;

            var students = new List<StudentDocument>();

            foreach (var (id, noOfRetries) in idsWithNoOfRetry)
            {
                var currentRetryAttempt = (int) noOfRetries;

                StudentModel studentModel = _studentService.Get(id);

                if (studentModel != null)
                {
                    Logger.Debug($"Student id {id} found. Will be synced in ES");

                    students.Add(BuildStudent(studentModel, currentRetryAttempt));
                }
                else
                {
                    Logger.Debug($"Student id {id} not found. Will be deleted from ES");

                    students.Add(new StudentDocument
                    {
                        Id = id,
                        Deleted = true,
                        NoOfRetry = currentRetryAttempt
                    });
                }
            }

            return students;
        }

        private StudentDocument BuildStudent(StudentModel studentModel, int currentRetryAttempt)
        {
            dynamic dynamicContent = new
            {
                someProperty = new Random(DateTime.Now.Second).Next(1,2000),
                someSecondProperty = "some string property"
            };

            // enrich object with info from other places by case
            return new StudentDocument
            {
                Id = studentModel.StudentId.ToString(),
                Name = studentModel.Name,
                Address = new StudentAddressDocument {City = "Ohio"},
                NoOfRetry = currentRetryAttempt,
                LastUpdate = DateTime.UtcNow,
                DynamicContent=dynamicContent
            };
        }
    }
}