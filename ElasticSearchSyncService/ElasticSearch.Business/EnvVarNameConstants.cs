using System;
using System.Collections.Generic;
using System.Text;

namespace School.ElasticSearchSyncronizer
{
    public class EnvVarNameConstants
    {
        public static string ElasticSearchBulkSyncNoOfRetries = nameof(ElasticSearchBulkSyncNoOfRetries);

        public static string ElasticSearchSyncBatchSize = nameof(ElasticSearchSyncBatchSize);

        public static string ElasticSearchDocumentItemNoOfRetries = nameof(ElasticSearchDocumentItemNoOfRetries);

        public static string SyncJobCronExpression = nameof(SyncJobCronExpression);
    }
}
