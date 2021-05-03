using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Elastic.Types;
using ElasticSearch.Business.Interfaces;
using ElasticSearch.Business.Models;
using Redis.Interfaces;
using School.ElasticSearchSyncronizer;
using SchoolUtils;
using Serilog;

namespace Common.Services
{
    public class StudentSyncronizer : IStudentSyncronizer
    {
        private readonly IStudentService _studentService;
        private readonly IRedisService _redisService;
        private readonly ElasticSyncronizer _elasticSyncronizer;
        private readonly int _noOfRetries;
        private static readonly ILogger Logger = Log.ForContext<StudentSyncronizer>();

        public StudentSyncronizer(IStudentService studentService, IRedisService redisService, ElasticSyncronizer elasticSyncronizer, IAppSettings appSettings)
        {
            _studentService = studentService;
            _redisService = redisService;
            _elasticSyncronizer = elasticSyncronizer;

            _noOfRetries = appSettings.Get(EnvVarNameConstants.ElasticSearchDocumentItemNoOfRetries, 3);
        }

        public async Task ExecuteAsync()
        {
            while (await ExecuteInternal())
            {
                await Task.Delay(50);
            }
        }

        private async Task<bool> ExecuteInternal()
        {
            List<string> failedIds = new List<string>();
            List<StudentDocument> students = new List<StudentDocument>();

            try
            {
                students = await Get();

                if (students != null)
                {
                    failedIds = _elasticSyncronizer.Execute(students);

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"failed to run the student syncronizer job:{e}");

                HandleException(failedIds, students);
            }

            return false;
        }

        private void HandleException(List<string> failedIds, List<StudentDocument> students)
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
            List<(string id, double score)> idsWithNoOfRetry = await _redisService.ExtractIdsFromSetWithScore(nameof(StudentDocument)).ConfigureAwait(false);

            if (idsWithNoOfRetry == null || idsWithNoOfRetry.Count == 0)
            {
                return null;
            }

            List<StudentDocument> students = new List<StudentDocument>();

            foreach ((string id, double score) in idsWithNoOfRetry)
            {
                int currentRetryAttempt = (int)score;

                StudentModel studentModel = _studentService.Get(id);

                if (studentModel != null)
                {
                    Logger.Debug($"Student id {id} found. Will be sync in ES");

                    students.Add(BuildStudent(studentModel, currentRetryAttempt));
                }
                else
                {
                    Logger.Debug($"Student id {id} not found. Will be deleted from ES");

                    students.Add(new StudentDocument()
                    {
                        Id = id,
                        Deleted = true,
                        NoOfRetry = currentRetryAttempt
                    });
                }
            };

            return students;
        }

        private StudentDocument BuildStudent(StudentModel studentModel, int currentRetryAttempt)
        {
            // enrich object with info from other places by case
            return new StudentDocument()
            {
                Id = studentModel.StudentId.ToString(),
                Name = studentModel.Name,
                Address = new StudentAddressDocument() { City = "Ohio" },
                NoOfRetry = currentRetryAttempt,
                LastUpdate = DateTime.UtcNow
            };
        }
    }
}
