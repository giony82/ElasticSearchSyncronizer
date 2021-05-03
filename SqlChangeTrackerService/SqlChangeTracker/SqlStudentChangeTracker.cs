using System;
using Redis.Interfaces;
using Serilog;
using SqlChangeTrackerService.Business.Models;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;

namespace SqlChangeTrackerService.Business
{
    /// <summary>
    /// https://github.com/christiandelbianco/monitor-table-change-with-sqltabledependency
    /// </summary>
    public class SqlEntityChangeTracker : IDisposable
    {
        private bool disposedValue;
        private readonly SqlTableDependency<StudentDocument> sqlStudentTableDependency;
        private readonly IRedisService redisService;
        private static readonly ILogger Logger = Log.ForContext<SqlEntityChangeTracker>();

        public SqlEntityChangeTracker(string conn, IRedisService redisService)
        {
            sqlStudentTableDependency = new SqlTableDependency<StudentDocument>(conn, "Students");
            sqlStudentTableDependency.OnChanged += OnStudentChanged;

            this.redisService = redisService;
        }

        public void Start()
        {
            sqlStudentTableDependency.Start();
        }

        private void OnStudentChanged(object sender, RecordChangedEventArgs<StudentDocument> e)
        {
            StudentDocument changedEntity = e.Entity;

            Logger.Debug($"Change detected:{changedEntity.StudentId}");

            try
            {
                redisService.AddToSet(changedEntity.StudentId, "StudentDocument");
            }
            catch (Exception ex)
            {
                Logger.Error(ex,"Can't handle changes for students");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    sqlStudentTableDependency.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
