using System.Globalization;

namespace AutoDoc.Models
{
    public class AppSettings
    {
        public AppSettings()
        {
            RepositoryPath = string.Empty;
            OwnerEmail = string.Empty;
            OwnerName = string.Empty;
            Culture = string.Empty;
            OutputPath = string.Empty;
            CompletionsUri = string.Empty;
            ApiKey = string.Empty;
        }

        public string RepositoryPath { get; set; }

        public string ProjectName { get { return Path.GetFileNameWithoutExtension(RepositoryPath); } }

        public string OwnerEmail { get; set; }

        public string OwnerName { get; set; }

        public string Culture { get; set; }

        public CultureInfo CultureInfo { get { return new CultureInfo(Culture); } }

        public string OutputPath { get; set; }

        public string CompletionsUri { get; set; }

        public string ApiKey { get; set; }

        public int DelayMilliseconds { get; set; }

        public double ModelTemperature { get; set; }

        public int MaxRetries { get; set; }
    }
}
