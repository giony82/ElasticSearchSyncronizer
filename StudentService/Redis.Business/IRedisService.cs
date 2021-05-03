using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Redis.Interfaces
{
    public interface IRedisService
    {
        bool AcquireLock(string key);
        void AddToSet(string identifier, string setID, double score = 0);
        Task<List<(string id, double score)>> ExtractIdsFromSetWithScore(string setID);
        void ReleaseLock(string key);
        T WithLock<T>(Func<T> action, string identifier);
    }
}
