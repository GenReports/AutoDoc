using System.Text;
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

            CancellationToken ct = default;

            var logger = GetLogger();
            var appSettings = GetAppSettings();
            var modelContext = await GetModelContextAsync(ct);

            var myCommits = GitClient.GetCommits(start, end, appSettings);

            await LMClient.GenerateReportsAsync(myCommits, modelContext, logger, appSettings, ct);

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

        static async Task<string> GetModelContextAsync(CancellationToken ct)
        {
            const string FileName = "Context.txt";

            var userContext = await File.ReadAllTextAsync(FileName, Encoding.UTF8, ct);
            ArgumentException.ThrowIfNullOrWhiteSpace(userContext);

            return string.Concat(Constants.ModelContext, Environment.NewLine, userContext).Trim();
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