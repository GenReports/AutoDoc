using AutoDoc.Clients;
using AutoDoc.Validators;
using Microsoft.Extensions.Logging;

namespace AutoDoc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (!InputValidator.TryGetDates(args, out DateTime start, out DateTime end))
                throw new ArgumentException(Constants.InputError);

            var logger = GetLogger();

            var myCommits = GitClient.GetCommits(start, end);

            await LMClient.GenerateReportsAsync(myCommits, logger);
        }

        static ILogger<Program> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            return loggerFactory.CreateLogger<Program>();
        }
    }
}