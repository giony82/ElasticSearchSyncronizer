namespace ElasticSearch.Business
{
    public class BusinessConstants
    {
        public static string ElasticSearchBulkSyncNoOfRetries = nameof(ElasticSearchBulkSyncNoOfRetries);

        public static string ElasticSearchSyncBatchSize = nameof(ElasticSearchSyncBatchSize);

        public static string ElasticSearchDocumentItemNoOfRetries = nameof(ElasticSearchDocumentItemNoOfRetries);
    }

    public class DefaultValues
    {
        public const int ElasticSearchDocumentItemNoOfRetries = 3;
    }
}
