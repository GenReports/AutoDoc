using CsvHelper;
using System.Text;
using AutoDoc.Models;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace AutoDoc.Clients
{
    internal static class CsvClient
    {
        private static readonly CultureInfo _culture = new("pt-BR");
        public const string DirectoryName = "Datas";

        public static async Task<bool> CreateAsync(
            IEnumerable<Report[]> chunkReports,
            ILogger<Program>? logger,
            CancellationToken ct = default)
        {
            if (chunkReports is null || !chunkReports.Any())
                return false;

            foreach (var reports in chunkReports)
            {
                await CreateAsync(reports, logger, ct);
            }

            return true;
        }

        public static async Task<bool> CreateAsync(
            IEnumerable<Report> reports,
            ILogger<Program>? logger,
            CancellationToken ct = default)
        {
            if (reports is null || !reports.Any())
                return false;

            try
            {
                var path = CreatePath(reports.First());

                using var writer = new StreamWriter(path);
                using var csv = new CsvWriter(writer, _culture);

                await csv.WriteRecordsAsync(reports, ct);
            }
            catch (Exception ex)
            {
                logger?.LogError("{Error}", ex.Message);
                throw;
            }

            return true;
        }

        private static string CreatePath(Report report)
        {
            ArgumentNullException.ThrowIfNull(report);

            if (!Directory.Exists(DirectoryName))
                Directory.CreateDirectory(DirectoryName);

            var fileName = new StringBuilder()
                .Append(report.Date.ToString("dd-MM-yyyy-"))
                .Append(Guid.CreateVersion7().ToString("N"))
                .Append(".csv")
                .ToString();

            return Path.Combine(DirectoryName, fileName);
        }
    }
}
