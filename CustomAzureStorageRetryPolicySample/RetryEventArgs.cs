using System;
using Microsoft.WindowsAzure.Storage;

namespace CustomAzureStorageRetryPolicySample
{
    /// <summary>
    /// Data that will be passed on to the retry event handler.
    /// </summary>
    public class RetryEventArgs : EventArgs
    {
        public int CurrentRetryCount { get; set; }
        public int StatusCode { get; set; }
        public Exception LastException { get; set; }
        public TimeSpan RetryInterval { get; set; }
        public OperationContext OperationContext { get; set; }

        public RetryEventArgs(int currentRetryCount, int statusCode, Exception lastException, TimeSpan retryInterval, OperationContext operationContext)
        {
            this.CurrentRetryCount = currentRetryCount;
            this.StatusCode = statusCode;
            this.LastException = lastException;
            this.RetryInterval = retryInterval;
            this.OperationContext = operationContext;
        }
    }
}
