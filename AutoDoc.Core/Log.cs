using Microsoft.Extensions.Logging;

namespace AutoDoc.Core
{
    public class Log { }

    public static class LogUtils
    {
        public static ILogger<Log> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            return loggerFactory.CreateLogger<Log>();
        }
    }
}
