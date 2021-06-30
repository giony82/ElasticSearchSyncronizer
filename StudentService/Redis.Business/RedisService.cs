using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Services;
using Redis.Interfaces;
using SchoolUtils;
using Serilog;
using StackExchange.Redis;

namespace Redis.Business
{
    public class RedisService : IRedisService
    {
        private readonly ServiceRetry _serviceRetry;
        private readonly int _batchSize;
        private readonly TimeSpan _timeoutSpan = new TimeSpan(0, 0, 10);
        private static readonly ILogger Logger = Log.ForContext<RedisService>();
        private readonly ConnectionMultiplexer _muxer;

        public RedisService(ServiceRetry serviceRetry, IAppSettings appSettings)
        {
            _muxer = ConnectionMultiplexer.Connect(appSettings.Get<string>(RedisEnvConstants.RedisHost));
            _serviceRetry = serviceRetry;
            _batchSize = appSettings.Get(RedisEnvConstants.RedisSetRange, 10);
        }

        public void AddToSet(string identifier, string setId, double score = 0)
        {
            _serviceRetry.RetryPolicy.Execute(() =>
            {
                _muxer.GetDatabase().SortedSetAdd(setId, identifier, score: score);
            });
        }

        public T WithLock<T>(Func<T> action, string identifier)
        {
            var locked = false;
            try
            {
                if (AcquireLock(identifier))
                {
                    Logger.Debug($"Lock acquired for {identifier}");
                    locked = true;
                    return action();
                }
            }
            finally
            {
                if (locked)
                {
                    ReleaseLock(identifier);
                    Logger.Debug($"Lock removed for {identifier}");
                }
            }
            return default;
        }

        /// <summary>
        /// Extracts a range of values from the given sorted set Id. This is done transactional by creating a Redis transaction, fetching the configured range of values
        /// and then removing the range.
        /// </summary>
        public async Task<List<(string id, double score)>> ExtractIdsFromSetWithScore(string setId)
        {
            return await _serviceRetry.RetryPolicy.Execute(async () =>
            {
                //The object returned from GetDatabase is a cheap pass-thru object, and does not need to be stored.
                IDatabase db = _muxer.GetDatabase();

                ITransaction transaction = db.CreateTransaction();

                var results = await transaction.SortedSetRangeByRankWithScoresAsync(setId, 0, _batchSize - 1);

                var idsWithScore = results.Select(result => (result.Element, result.Score)).Select(dummy => ((string, double)) dummy).ToList();

                await transaction.SortedSetRemoveRangeByRankAsync(setId, 0, _batchSize - 1);

                var committed = await transaction.ExecuteAsync();

                if(!committed)
                {
                    throw new Exception("Cannot commit redis transaction");
                }

                return idsWithScore;
            });
        }

        /// <summary>  
        /// Acquires the lock.  
        /// </summary>  
        /// <returns><c>true</c>, if lock was acquired, <c>false</c> otherwise.</returns>          
        public bool AcquireLock(string key)
        {
            return _serviceRetry.RetryPolicy.Execute(() =>
            {
                var acquired = _muxer.GetDatabase().StringSet(key, "1", _timeoutSpan, When.NotExists);
                if (!acquired)
                {
                    throw new Exception($"A lock already exists for {key}");
                }
                return true;
            });
        }

        /// <summary>  
        /// Releases the lock.  
        /// </summary>          
        public void ReleaseLock(string key)
        {
            var lua_script = @"  
    if (redis.call('GET', KEYS[1]) == ARGV[1]) then  
        redis.call('DEL', KEYS[1])  
        return true  
    else  
        return false  
    end  
    ";
            _serviceRetry.RetryPolicy.Execute(() =>
            {
                IDatabase db = _muxer.GetDatabase();

                db.ScriptEvaluate(lua_script, new RedisKey[] { key }, new RedisValue[] { "1" });
            });
        }
    }
}
