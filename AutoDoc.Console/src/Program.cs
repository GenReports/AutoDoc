using AutoDoc.Core;
using AutoDoc.Core.Clients;
using AutoDoc.Core.Models;
using AutoDoc.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AutoDoc
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            if (!InputValidator.TryGetDates(args, out DateTime start, out DateTime end))
                throw new ArgumentException(Constants.InputError);

            CancellationToken ct = default;

            var logger = LogUtils.GetLogger();
            var appSettings = GetAppSettings();

            var myCommits = GitClient.GetCommits(start, end, appSettings);

            var modelData = await GetModelDataAsync(ct);

            await LMClient.GenerateReportsAsync(myCommits, modelData, logger, appSettings, ct);

            logger?.LogInformation("Finished :) Please look at {OutputPath}", appSettings.OutputPath);
        }

        static async Task<string> GetModelContextAsync(CancellationToken ct)
        {
            const string FileName = "Context.txt";

            var path = Path.Combine(AppContext.BaseDirectory, FileName);

            var userContext = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);
            ArgumentException.ThrowIfNullOrWhiteSpace(userContext);

            return string.Concat(Constants.ModelContext, Environment.NewLine, userContext).Trim();
        }

        static async Task<ModelData> GetModelDataAsync(CancellationToken ct = default)
        {
            var modelTemplateMsg = Constants.ModelMessage;
            var modelContext = await GetModelContextAsync(ct);

            return new ModelData
            {
                Context = modelContext,
                TemplateMessage = modelTemplateMsg,
            };
        }

        static AppSettings GetAppSettings()
        {
            const string AppSettingsName = "appsettings.json";

            return new ConfigurationBuilder()
                .AddJsonFile(AppSettingsName)
                .Build()
                .Get<AppSettings>()!;
        }
    }
}