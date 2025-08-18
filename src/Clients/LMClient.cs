using System.Text;
using AutoDoc.Models;
using System.Text.Json;
using AutoDoc.Extensions;
using System.Net.Http.Headers;
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
            ILogger<Program>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any())
                return [];

            var modelContext = await GetModelContextAsync(ct);

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
            if (commits is null || !commits.Any())
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
            if (commits is null || !commits.Any())
                return [];

            var retriesCount = 1;
            string json = string.Empty;
            IEnumerable<Report>? result = null;

            do
            {
                try
                {
                    var response = await SendRequestAsync(commits, modelContext, appSettings, ct);
                    var responseString = await response.Content.ReadAsStringAsync(ct);

                    using var doc = JsonDocument.Parse(responseString);
                    json = doc.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;

                    result = JsonSerializer.Deserialize<IEnumerable<Report>>(json, _jsonOptions);
                }
                catch (Exception ex)
                {
                    logger?.LogError("Attempt: {Count}º", retriesCount);
                    logger?.LogError("{Error}", ex.Message);
                    logger?.LogError("{Json}", json);

                    retriesCount++;

                    if (retriesCount > appSettings.MaxRetries)
                        throw;

                    await Task.Delay(appSettings.DelayMilliseconds, ct);
                }

            } while (retriesCount <= appSettings.MaxRetries && result is null);

            return result!;
        }

        private static async Task<HttpResponseMessage> SendRequestAsync(
            IEnumerable<MyCommit> commits,
            string modelContext,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any())
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var content = BuildRequestBody(commits, modelContext, appSettings);

            var request = new HttpRequestMessage(HttpMethod.Post, appSettings.CompletionsUri)
            {
                Content = content
            };

            if (!string.IsNullOrWhiteSpace(appSettings.ApiKey))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", appSettings.ApiKey);

            var response = await _httpClient.SendAsync(request, ct);
            return response.EnsureSuccessStatusCode();
        }

        private static StringContent BuildRequestBody(
            IEnumerable<MyCommit> commits,
            string modelContext,
            AppSettings appSettings)
        {
            if (commits is null || !commits.Any())
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

        private static async Task<string> GetModelContextAsync(CancellationToken ct)
        {
            const string FileName = "Context.txt";

            var path = Path.Combine(AppContext.BaseDirectory, FileName);

            var userContext = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);
            ArgumentException.ThrowIfNullOrWhiteSpace(userContext);

            return string.Concat(Constants.ModelContext, Environment.NewLine, userContext).Trim();
        }
    }
}
