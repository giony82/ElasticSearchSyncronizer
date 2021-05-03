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
        private readonly string setLock = nameof(setLock);
        private readonly ServiceRetry serviceRetry;
        private readonly int range;
        private readonly TimeSpan timeoutSpan = new TimeSpan(0, 0, 10);
        private static readonly ILogger Logger = Log.ForContext<RedisService>();
        private readonly ConnectionMultiplexer muxer;

        public RedisService(ServiceRetry serviceRetry, IAppSettings appSettings)
        {
            muxer = ConnectionMultiplexer.Connect(appSettings.Get<string>(RedisEnvConstants.RedisHost));
            this.serviceRetry = serviceRetry;
            range = appSettings.Get(RedisEnvConstants.RedisSetRange, 10);
        }

        public void AddToSet(string identifier, string setID, double score = 0)
        {
            serviceRetry.RetryPolicy.Execute(() =>
            {
                muxer.GetDatabase().SortedSetAdd(setID, identifier, score: score);
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
            return await serviceRetry.RetryPolicy.Execute(async () =>
            {
                //The object returned from GetDatabase is a cheap pass-thru object, and does not need to be stored.
                IDatabase db = muxer.GetDatabase();

                var transaction = db.CreateTransaction();

                var task = transaction.SortedSetRangeByRankWithScoresAsync(setID, 0, range - 1);



                var re= transaction.SortedSetRemoveRangeByRankAsync(setID, 0, range - 1);

                bool commited = await transaction.ExecuteAsync();

                if(!commited)
                {
                    throw new Exception("Cannot commit redis transaction");
                }

                List<(string, double)> idsWithScore = new List<(string, double)>();

                foreach (var result in await task)
                {
                    idsWithScore.Add((result.Element, result.Score));
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
            return serviceRetry.RetryPolicy.Execute(() =>
            {
                bool acquired = muxer.GetDatabase().StringSet(key, "1", timeoutSpan, When.NotExists);
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
            serviceRetry.RetryPolicy.Execute(() =>
            {
                IDatabase db = muxer.GetDatabase();

                var res = db.ScriptEvaluate(lua_script, new RedisKey[] { key }, new RedisValue[] { "1" });
            });
        }
    }
}
