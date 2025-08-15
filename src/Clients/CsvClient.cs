using CsvHelper;
using System.Text;
using AutoDoc.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace AutoDoc.Clients
{
    internal static class CsvClient
    {
        public static async Task<bool> CreateAsync(
            IEnumerable<IEnumerable<Report>> chunkReports,
            ILogger<Program>? logger,
            AppSettings appSettings,
            CultureInfo culture,
            CancellationToken ct = default)
        {
            if (chunkReports is null || !chunkReports.Any())
                return false;

            foreach (var reports in chunkReports)
            {
                await CreateAsync(reports, logger, appSettings, culture, ct);
            }

            return true;
        }

        public static async Task<bool> CreateAsync(
            IEnumerable<Report> reports,
            ILogger<Program>? logger,
            AppSettings appSettings,
            CultureInfo culture,
            CancellationToken ct = default)
        {
            if (reports is null || !reports.Any())
                return false;

            try
            {
                var path = CreatePath(reports.First(), appSettings);

                using var writer = new StreamWriter(path);
                using var csv = new CsvWriter(writer, culture);

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
                .Append(report.Date.ToString())
                .Append('-')
                .Append(Guid.CreateVersion7().ToString("N"))
                .Append(".csv")
                .ToString();

            return Path.Combine(appSettings.OutputPath, fileName);
        }
    }
}
