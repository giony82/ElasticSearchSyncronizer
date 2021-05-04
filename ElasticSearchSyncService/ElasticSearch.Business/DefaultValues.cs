// ------------------------------------------------------------------------------
//     <copyright file="DefaultValues.cs" company="BlackLine">
//         Copyright (C) BlackLine. All rights reserved.
//     </copyright>
// ------------------------------------------------------------------------------

namespace ElasticSearch.Business
{
    public class DefaultValues
    {
        public const int ElasticSearchDocumentItemNoOfRetries = 3;

        /// <summary>
        /// Indicates the default value for how many retries are attempted if a bulk update failure occurs.
        /// </summary>
        public const int ElasticSearchBulkSyncNoOfRetries = 3;

        /// <summary>
        ///  Indicates the default value for how many documents are updated in ES at once.
        /// </summary>
        public const int ElasticSearchSyncBatchSize = 3;
    }
}