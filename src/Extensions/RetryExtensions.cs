using AutoDoc.Models;
using Microsoft.Extensions.Logging;

namespace AutoDoc.src.Extensions
{
    public static class RetryExtensions
    {
        public static async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> action,
            AppSettings appSettings,
            ILogger<Program>? logger,
            CancellationToken cancellationToken = default)
        {
            var retriesCount = 1;
            T? result = default;

            do
            {
                try
                {
                    result = await action(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger?.LogError("Attempt: {Count}º", retriesCount);
                    logger?.LogError("{Error}", ex.Message);

                    retriesCount++;

                    if (retriesCount > appSettings.MaxRetries)
                        throw;

                    await Task.Delay(appSettings.DelayMilliseconds, cancellationToken);
                }
            }
            while (retriesCount <= appSettings.MaxRetries && result is null);

            return result!;
        }
    }
}
