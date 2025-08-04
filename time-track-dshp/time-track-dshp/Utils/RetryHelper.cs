using System;

namespace time_track_dshp.Utils
{
    public class RetryHelper
    {
        public const string RetryPolicyName = "HttpRetry";

        public static double CalculateRetryTimeMs(int retryCount, double maximumTime)
        {
            // Raise the number of retries to the power of 2 and convert to milliseconds 
            var calculatedTime = Math.Pow(retryCount, 2) * 1000;

            return calculatedTime < maximumTime ? calculatedTime : maximumTime;
        }
    }
}