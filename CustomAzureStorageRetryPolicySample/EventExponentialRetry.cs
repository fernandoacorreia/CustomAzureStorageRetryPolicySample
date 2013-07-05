using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace CustomAzureStorageRetryPolicySample
{
    /// <summary>
    /// A custom event policy based on ExponentialRetry that triggers an event when there is a retry attempt.
    /// </summary>
    public class EventExponentialRetry : IRetryPolicy
    {
        private static readonly TimeSpan DefaultClientBackoff = TimeSpan.FromSeconds(4.0);
        private const int DefaultClientRetryCount = 3;
        private TimeSpan deltaBackoff;
        private int maximumAttempts;
        private ExponentialRetry retry;

        public event EventHandler<RetryEventArgs> RaiseRetryEvent;

        public EventExponentialRetry()
        {
            Initialize(DefaultClientBackoff, DefaultClientRetryCount);
        }

        public EventExponentialRetry(TimeSpan deltaBackoff, int maxAttempts)
        {
            Initialize(deltaBackoff, maxAttempts);
        }

        private void Initialize(TimeSpan deltaBackoff, int maxAttempts)
        {
            this.deltaBackoff = deltaBackoff;
            this.maximumAttempts = maxAttempts;
            retry = new ExponentialRetry(this.deltaBackoff, this.maximumAttempts);
        }

        public IRetryPolicy CreateInstance()
        {
            EventExponentialRetry newInstance = new EventExponentialRetry(this.deltaBackoff, this.maximumAttempts);
            newInstance.RaiseRetryEvent = this.RaiseRetryEvent;
            return newInstance;
        }

        public bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, OperationContext operationContext)
        {
            bool shouldRetry = retry.ShouldRetry(currentRetryCount, statusCode, lastException, out retryInterval, operationContext);
            if (shouldRetry)
            {
                OnRaiseRetryEvent(new RetryEventArgs(currentRetryCount, statusCode, lastException, retryInterval, operationContext));
            }
            return shouldRetry;
        }

        protected virtual void OnRaiseRetryEvent(RetryEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler<RetryEventArgs> handler = RaiseRetryEvent;

            // Event will be null if there are no subscribers.
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }
    }
}
