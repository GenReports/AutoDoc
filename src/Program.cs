using AutoDoc.Clients;
using AutoDoc.Models;
using AutoDoc.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoDoc
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (!InputValidator.TryGetDates(args, out DateTime start, out DateTime end))
                throw new ArgumentException(Constants.InputError);

            CancellationToken ct = default;

            var logger = GetLogger();
            var appSettings = GetAppSettings();

            var myCommits = GitClient.GetCommits(start, end, appSettings);

            await LMClient.GenerateReportsAsync(myCommits, logger, appSettings, ct);

            logger?.LogInformation("Finished :) Please look at {OutputPath}", appSettings.OutputPath);
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