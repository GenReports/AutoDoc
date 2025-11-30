using AutoDoc.Core.Models;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AutoDoc.Core.Clients
{
    public static class CsvClient
    {
        public static async Task<bool> CreateAsync(
            IEnumerable<IEnumerable<Report>> chunkReports,
            ILogger<Log>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (chunkReports is null || !chunkReports.Any())
                return false;

            foreach (var reports in chunkReports)
            {
                if (!await CreateAsync(reports, logger, appSettings, ct))
                    return false;
            }

            return true;
        }

        public static async Task<bool> CreateAsync(
            IEnumerable<Report> reports,
            ILogger<Log>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (reports is null || !reports.Any())
                return false;

            try
            {
                var path = CreatePath(reports.First(), appSettings);

                using var writer = new StreamWriter(path);
                using var csv = new CsvWriter(writer, appSettings.CultureInfo);

                await csv.WriteRecordsAsync(reports, ct);
            }
            catch (Exception ex)
            {
                logger?.LogError("{Error}", ex.Message);
                throw;
            }

            return true;
        }

        private static string CreatePath(Report report, AppSettings appSettings)
        {
            ArgumentNullException.ThrowIfNull(report);

            if (!Directory.Exists(appSettings.OutputPath))
                Directory.CreateDirectory(appSettings.OutputPath);

            var fileName = new StringBuilder()
                .Append(appSettings.ProjectName)
                .Append('-')
                .Append(report.Date.ToShortDateString().Replace('/', '-'))
                .Append('-')
                .Append(Guid.CreateVersion7().ToString("N"))
                .Append(".csv")
                .ToString();

            return Path.Combine(appSettings.OutputPath, fileName);
        }
    }
}
