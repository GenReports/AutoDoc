using AutoDoc.Models;
using AutoDoc.Clients;
using AutoDoc.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AutoDoc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (!InputValidator.TryGetDates(args, out DateTime start, out DateTime end))
                throw new ArgumentException(Constants.InputError);

            var logger = GetLogger();
            var appSettings = GetAppSettings();

            var myCommits = GitClient.GetCommits(start, end, appSettings);

            await LMClient.GenerateReportsAsync(myCommits, logger, appSettings);
        }

        static AppSettings GetAppSettings()
        {
            const string AppSettingsName = "appsettings.json";

            return new ConfigurationBuilder()
                .AddJsonFile(AppSettingsName)
                .Build()
                .Get<AppSettings>()!;
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