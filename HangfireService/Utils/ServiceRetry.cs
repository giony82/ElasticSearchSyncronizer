using System;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace Common
{
    public class ServiceRetry
    {
        public ServiceRetry()
        {
            //see more details here https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#wait-and-retry-with-jittered-back-off
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(3), retryCount: 5);

            this.RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(delay);
        }

        public RetryPolicy RetryPolicy { get; }
    }
}
