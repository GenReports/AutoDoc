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

        public const string CompletionsUri = "http://localhost:1234/v1/chat/completions";

        public static async Task<IEnumerable<Report[]>> GenerateReportsAsync(
            IEnumerable<MyCommit> commits,
            ILogger<Program>? logger,
            CancellationToken ct = default)
        {
            if (commits is null || !commits.Any())
                return [];

            var chunkReports = new List<Report[]>();
            var dates = commits.Select(c => c.CreatedAt.Date).Distinct();

            foreach (var date in dates)
            {
                var reportsByDate = await GetReportsByDateAsync(commits, date, logger, ct);
                await CsvClient.CreateAsync(reportsByDate, logger, ct);

                chunkReports.Add([.. reportsByDate]);
            }

            return chunkReports;
        }

        private static async Task<IEnumerable<Report>> GetReportsByDateAsync(
            IEnumerable<MyCommit> commits,
            DateTime date,
            ILogger<Program>? logger,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any())
                return [];

            IEnumerable<Report> reportsByDate = [];

            var totalCommitsByDate = commits.Where(c => c.CreatedAt.Date == date);
            var totalCount = totalCommitsByDate.Count();

            logger?.LogInformation("Start for date: {Date} | Total commits: {Count}",
                    date.ToShortDateString(), totalCount);

            foreach (var processCommits in totalCommitsByDate.Chunk())
            {
                logger?.LogInformation("Process: {Count}º commits", processCommits.Length);

                var reports = await CallModelAsync(processCommits, logger, ct);
                reportsByDate = reportsByDate.Concat(reports);

                await totalCount.DelayAsync(multiplier: 3000, ct);
            }

            return reportsByDate;
        }

        private static async Task<Report[]> CallModelAsync(
            IEnumerable<MyCommit> commits,
            ILogger<Program>? logger,
            CancellationToken ct = default)
        {
            if (commits is null || !commits.Any())
                return [];

            var content = BuildRequestBody(commits);

            var response = await _httpClient.PostAsync(CompletionsUri, content, ct);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(responseString);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;

            try
            {
                return JsonSerializer.Deserialize<Report[]>(message, _jsonOptions)!;
            }
            catch (Exception ex)
            {
                logger?.LogError("{Error}", ex.Message);
                logger?.LogError("{Json}", message);
                throw;
            }
        }

        private static StringContent BuildRequestBody(IEnumerable<MyCommit> commits)
        {
            if (commits is null || !commits.Any())
                return new StringContent(string.Empty);

            var inputJson = JsonSerializer.Serialize(commits);

            var requestBody = new
            {
                messages = new[]
                {
                    //new { role = "system", content = Constants.Context },
                    new { role = "user", content = string.Format(Constants.Message, inputJson) }
                },
                temperature = 0.5
            };

            var json = JsonSerializer.Serialize(requestBody);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
