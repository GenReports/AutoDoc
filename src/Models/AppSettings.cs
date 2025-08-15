namespace AutoDoc.Models
{
    internal class AppSettings
    {
        public AppSettings()
        {
            RepositoryPath = string.Empty;
            OwnerEmail = string.Empty;
            OwnerName = string.Empty;
            Culture = string.Empty;
            OutputPath = string.Empty;
            CompletionsUri = string.Empty;
        }

        public string RepositoryPath { get; set; }

        public string OwnerEmail { get; set; }

        public string OwnerName { get; set; }

        public string Culture { get; set; }

        public string OutputPath { get; set; }

        public string CompletionsUri { get; set; }

        public int DelayMillisecondsMultiplier { get; set; }

        public double ModelTemperature { get; set; }
    }
}
