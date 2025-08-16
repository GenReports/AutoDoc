using System.Text;
using AutoDoc.Models;
using System.Text.Json;
using AutoDoc.Extensions;
using Microsoft.Extensions.Logging;

namespace AutoDoc.Clients
{
    internal static class LMClient
    {
        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10_000)
        };

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        public static async Task<IEnumerable<IEnumerable<Report>>> GenerateReportsAsync(
            IEnumerable<MyCommit> commits,
            string modelContext,
            ILogger<Program>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any() || string.IsNullOrWhiteSpace(modelContext))
                return [];

            var chunkReports = new List<IEnumerable<Report>>();
            var dates = commits.Select(c => c.CreatedAt.Date).Distinct();

            foreach (var date in dates)
            {
                var reportsByDate = await GetReportsByDateAsync(commits, modelContext, date, logger, appSettings, ct);
                await CsvClient.CreateAsync(reportsByDate, logger, appSettings, ct);

                chunkReports.Add(reportsByDate);
            }

            return chunkReports;
        }

        private static async Task<IEnumerable<Report>> GetReportsByDateAsync(
            IEnumerable<MyCommit> commits,
            string modelContext,
            DateTime date,
            ILogger<Program>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any() || string.IsNullOrWhiteSpace(modelContext))
                return [];

            IEnumerable<Report> reportsByDate = [];

            var totalCommitsByDate = commits.Where(c => c.CreatedAt.Date == date);
            var totalCount = totalCommitsByDate.Count();

            logger?.LogInformation("Start for date: {Date} | Total commits: {Count}",
                    date.ToShortDateString(), totalCount);

            foreach (var processCommits in totalCommitsByDate.Chunk())
            {
                logger?.LogInformation("Process: {Count}º commits", processCommits.Count());

                var reports = await CallModelAsync(processCommits, modelContext, logger, appSettings, ct);
                reportsByDate = reportsByDate.Concat(reports);

                await Task.Delay(appSettings.DelayMilliseconds, ct);
            }

            return reportsByDate.Set(c => c.Participants, appSettings.OwnerName);
        }

        private static async Task<IEnumerable<Report>> CallModelAsync(
            IEnumerable<MyCommit> commits,
            string modelContext,
            ILogger<Program>? logger,
            AppSettings appSettings,
            CancellationToken ct = default)
        {
            if (commits is null || !commits.Any() || string.IsNullOrWhiteSpace(modelContext))
                return [];

            var retriesCount = 1;
            string message = string.Empty;
            IEnumerable<Report>? result = null;

            do
            {
                try
                {
                    var content = BuildRequestBody(commits, modelContext, appSettings);

                    var response = await _httpClient.PostAsync(appSettings.CompletionsUri, content, ct);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync(ct);

                    using var doc = JsonDocument.Parse(responseString);
                    message = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;

                    result = JsonSerializer.Deserialize<IEnumerable<Report>>(message, _jsonOptions);
                }
                catch (Exception ex)
                {
                    logger?.LogError("Attempt: {Count}º", retriesCount);
                    logger?.LogError("{Error}", ex.Message);
                    logger?.LogError("{Json}", message);

                    retriesCount++;

                    if (retriesCount > appSettings.MaxRetries)
                        throw;

                    await Task.Delay(appSettings.DelayMilliseconds, ct);
                }

            } while (retriesCount <= appSettings.MaxRetries && result is null);

            return result!;
        }

        private static StringContent BuildRequestBody(
            IEnumerable<MyCommit> commits,
            string modelContext,
            AppSettings appSettings)
        {
            if (commits is null || !commits.Any() || string.IsNullOrWhiteSpace(modelContext))
                return new StringContent(string.Empty);

            var inputJson = JsonSerializer.Serialize(commits);

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = modelContext },
                    new { role = "user", content = string.Format(Constants.ModelMessage, inputJson, appSettings.Culture) }
                },
                temperature = appSettings.ModelTemperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
