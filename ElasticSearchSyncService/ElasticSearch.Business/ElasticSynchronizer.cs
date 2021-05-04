using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using Common.Elastic.Types;
using Elasticsearch.Net;
using Nest;
using SchoolUtils;
using Serilog;

namespace ElasticSearch.Business
{
    public class ElasticSynchronizer
    {
        private readonly ElasticClient _esClient;
        private static readonly ILogger Logger = Log.ForContext<ElasticSynchronizer>();
        private readonly IAppSettings _appSettings;

        public ElasticSynchronizer(IAppSettings appSettings)
        {
            SingleNodeConnectionPool connectionPool = new SingleNodeConnectionPool(new Uri("http://elk:9200"));

            ConnectionSettings connectionSettings = new ConnectionSettings(connectionPool)
            .EnableDebugMode()
            .DefaultMappingFor<StudentDocument>(m => m.IndexName(nameof(StudentDocument).ToLower()).IdProperty(p=>p.Id))
            .DefaultMappingFor<StudentAddressDocument>(m => m.IndexName(nameof(StudentAddressDocument).ToLower()).IdProperty(p => p.Id))
            .PrettyJson()
            .RequestTimeout(TimeSpan.FromMinutes(2));

            this._esClient = new ElasticClient(connectionSettings);            
            this._appSettings = appSettings;
        }

        /// <summary>
        /// Bulk sync to ES.
        /// </summary>
        /// <typeparam name="T">Document</typeparam>
        /// <param name="addedOrUpdatedDocuments">The documents to synchronize</param>
        /// <returns>the list of documents that could NOT be synced</returns>
        internal List<string> Execute<T>(List<T> addedOrUpdatedDocuments) where T : DocumentBase
        {
            var sw = new Stopwatch();
            sw.Start();
            Logger.Debug($"Got {addedOrUpdatedDocuments.Count} new/updated items!");

            var failedIds = new List<string>();

            var bulkAllObservable = _esClient.BulkAll(addedOrUpdatedDocuments, b => b.BufferToBulk((descriptor, buffer) =>
                {
                    foreach (T document in buffer)
                    {
                        if (document.Deleted)
                        {
                            descriptor.Delete<T>(doc => doc.Index(document.GetType().Name.ToLower()).Document(document));
                            Logger.Debug($"Item {document.Id} marked to be deleted!");
                        }
                        else
                        {
                            descriptor.Index<T>(doc => doc.Index(document.GetType().Name.ToLower()).Document(document));
                            Logger.Debug($"Item {document.Id} marked to be upserted!");
                        }
                    }
                })
            .DroppedDocumentCallback((bulkResponseItem, document) =>
            {
                Logger.Error($"Unable to index: {bulkResponseItem} {document}");
                failedIds.Add(document.Id);
            })
            .BackOffTime("1s") //how long to wait between retries
            .BackOffRetries(_appSettings.Get(BusinessConstants.ElasticSearchBulkSyncNoOfRetries, DefaultValues.ElasticSearchBulkSyncNoOfRetries)) //how many retries are attempted if a failure occurs
            .RefreshOnCompleted() //refresh the index after bulk insert
            .MaxDegreeOfParallelism(Environment.ProcessorCount)
            .ContinueAfterDroppedDocuments(true)
            .Size(_appSettings.Get(BusinessConstants.ElasticSearchSyncBatchSize, DefaultValues.ElasticSearchSyncBatchSize))); ;            

            var waitHandle = new ManualResetEvent(false);
            ExceptionDispatchInfo exceptionDispatchInfo = null;

            var observer = new BulkAllObserver(
                onNext: response =>
                {                    
                    Logger.Debug($"Written {response.Items.Count} in ES");                    
                },
                onError: exception =>
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
                    waitHandle.Set();
                },
                onCompleted: () => waitHandle.Set());

            bulkAllObservable.Subscribe(observer); //Subscribe to the observable, which will initiate the bulk indexing process

            waitHandle.WaitOne(TimeSpan.FromMinutes(1)); //Block the current thread until a signal is received

            exceptionDispatchInfo?.Throw(); //If an exception was captured during the bulk indexing process, throw it

            sw.Stop();

            Logger.Debug("Finished in {ElapsedMilliseconds} ms", sw.ElapsedMilliseconds);

            return failedIds;
        }
    }
}
