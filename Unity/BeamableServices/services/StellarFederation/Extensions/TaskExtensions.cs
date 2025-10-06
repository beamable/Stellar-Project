using System;
using System.Threading.Tasks;
using Beamable.Common;

namespace Beamable.StellarFederation.Extensions;

public static class TaskExtensions
{
    public static async Task RunAsyncBlock(this Func<Task> task)
    {
        try
        {
            await task();
        }
        catch (Exception ex)
        {
            var error = $"Error running asyncBlock in method {task.Method.Name}. Error: {ex.ToLogFormat()}";
            BeamableLogger.LogError(error);
            throw;
        }
    }

    public static async Task<T> WithRetry<T>(this Func<Task<T>> taskFactory, int maxRetry, int timeoutMs = 100, bool useExponentialBackoff = false)
    {
        if (maxRetry < 1)
            throw new ArgumentOutOfRangeException(nameof(maxRetry), "Max retry must be at least 1.");
        if (timeoutMs < 0)
            throw new ArgumentOutOfRangeException(nameof(timeoutMs), "Timeout must be non-negative.");

        var retryCount = 0;
        while (true)
        {
            try
            {
                return await taskFactory();
            }
            catch (Exception)
            {
                retryCount++;
                if (retryCount > maxRetry)
                {
                    throw;
                }
                var delay = useExponentialBackoff ? timeoutMs * (int)Math.Pow(2, retryCount - 1) : timeoutMs;
                await Task.Delay(delay);
            }
        }
    }
}