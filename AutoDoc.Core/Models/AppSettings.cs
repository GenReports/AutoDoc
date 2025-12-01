using System.Globalization;
using System.Text.Json.Serialization;

namespace AutoDoc.Core.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            Name = string.Empty;
            ApiKey = string.Empty;
            Culture = string.Empty;
            OwnerName = string.Empty;
            OwnerEmail = string.Empty;
            OutputPath = string.Empty;
            CompletionsUri = string.Empty;
            RepositoryPath = string.Empty;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public string RepositoryPath { get; set; }

        [JsonIgnore]
        public string ProjectName { get { return Path.GetFileNameWithoutExtension(RepositoryPath); } }

        [JsonIgnore]
        public string OwnerEmail { get; set; }

        [JsonIgnore]
        public string OwnerName { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }

        [JsonIgnore]
        public CultureInfo CultureInfo { get { return new CultureInfo(Culture); } }

        public string OutputPath { get; set; }

        public string CompletionsUri { get; set; }

        public string ApiKey { get; set; }

        public int DelayMilliseconds { get; set; }

        public double ModelTemperature { get; set; }

        public int MaxRetries { get; set; }
    }
}
