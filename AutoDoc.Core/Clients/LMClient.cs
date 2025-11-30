using AutoDoc.Core.Extensions;
using AutoDoc.Core.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ToonNetSerializer;

namespace AutoDoc.Core.Clients
{
    public static class LMClient
    {
        const int TimeoutSeconds = 10_000;

        private static readonly HttpClient _httpClient = new()
        {
            Timeout = TimeSpan.FromSeconds(TimeoutSeconds)
        };

        private static readonly ToonDecodeOptions _toonOptions = new()
        {
            Indent = 1,
            Strict = true,
            ExpandPaths = PathExpansionMode.Safe
        };

        public static async Task<IEnumerable<IEnumerable<Report>>> GenerateReportsAsync(
            IEnumerable<Commit> commits,
            ModelData modelData,
            ILogger<Log>? logger,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any() || modelData is null)
                return [];

            var chunkReports = new List<IEnumerable<Report>>();
            var dates = commits.Select(c => c.CreatedAt.Date).Distinct();

            foreach (var date in dates)
            {
                var reportsByDate = await GetReportsByDateAsync(commits, modelData, date, logger, appSettings, ct);
                await CsvClient.CreateAsync(reportsByDate, logger, appSettings, ct);

                chunkReports.Add(reportsByDate);
            }

            return chunkReports;
        }

        private static async Task<IEnumerable<Report>> GetReportsByDateAsync(
            IEnumerable<Commit> commits,
            ModelData modelData,
            DateTime date,
            ILogger<Log>? logger,
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

                var reports = await CallModelAsync(processCommits, modelData, logger, appSettings, ct);
                reportsByDate = reportsByDate.Concat(reports);

                await Task.Delay(appSettings.DelayMilliseconds, ct);
            }

            return reportsByDate.Set(c => c.Participants, appSettings.OwnerName);
        }

        private static async Task<IEnumerable<Report>> CallModelAsync(
            IEnumerable<Commit> commits,
            ModelData modelData,
            ILogger<Log>? logger,
            AppSettings appSettings,
            CancellationToken ct = default)
        {
            if (commits is null || !commits.Any())
                return [];

            return await RetryExtensions.ExecuteAsync(async ct =>
            {
                var response = await SendRequestAsync(commits, modelData, appSettings, ct);
                var responseString = await response.Content.ReadAsStringAsync(ct);

                using var doc = JsonDocument.Parse(responseString);
                var json = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()!;

                return ToonNet.Decode<IEnumerable<Report>>(json, _toonOptions)!;
            }, appSettings, logger, ct);
        }

        private static async Task<HttpResponseMessage> SendRequestAsync(
            IEnumerable<Commit> commits,
            ModelData modelData,
            AppSettings appSettings,
            CancellationToken ct)
        {
            if (commits is null || !commits.Any())
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            var content = BuildRequestBody(commits, modelData, appSettings);

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
            IEnumerable<Commit> commits,
            ModelData modelData,
            AppSettings appSettings)
        {
            if (commits is null || !commits.Any())
                return new StringContent(string.Empty);

            var inputJson = ToonNet.Encode(commits);

            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = modelData.Context },
                    new { role = "user", content = string.Format(modelData.TemplateMessage, inputJson, appSettings.Culture) }
                },
                temperature = appSettings.ModelTemperature
            };

            var json = JsonSerializer.Serialize(requestBody);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
