using System;
using System.Collections.Generic;
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
        private readonly string _setLock = nameof(_setLock);
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

        public void AddToSet(string identifier, string setID, double score = 0)
        {
            _serviceRetry.RetryPolicy.Execute(() =>
            {
                _muxer.GetDatabase().SortedSetAdd(setID, identifier, score: score);
            });
        }

        public T WithLock<T>(Func<T> action, string identifier)
        {
            bool locked = false;
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

        public async Task<List<(string id, double score)>> ExtractIdsFromSetWithScore(string setID)
        {
            return await _serviceRetry.RetryPolicy.Execute(async () =>
            {
                //The object returned from GetDatabase is a cheap pass-thru object, and does not need to be stored.
                IDatabase db = _muxer.GetDatabase();

                var transaction = db.CreateTransaction();

                SortedSetEntry[] results = await transaction.SortedSetRangeByRankWithScoresAsync(setID, 0, _batchSize - 1);

                List<(string, double)> idsWithScore = new List<(string, double)>();

                foreach (var result in results)
                {
                    idsWithScore.Add((result.Element, result.Score));
                }

                await transaction.SortedSetRemoveRangeByRankAsync(setID, 0, _batchSize - 1);

                bool commited = await transaction.ExecuteAsync();

                if(!commited)
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
                bool acquired = _muxer.GetDatabase().StringSet(key, "1", _timeoutSpan, When.NotExists);
                if (!acquired)
                {
                    throw new Exception($"A lock already exists for {key}");
                }
                return acquired;
            });
        }

        /// <summary>  
        /// Releases the lock.  
        /// </summary>          
        public void ReleaseLock(string key)
        {
            string lua_script = @"  
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

                var res = db.ScriptEvaluate(lua_script, new RedisKey[] { key }, new RedisValue[] { "1" });
            });
        }
    }
}
