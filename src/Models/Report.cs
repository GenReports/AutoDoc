namespace AutoDoc.Models
{
    internal class Report
    {
        public Report()
        {
            Step = string.Empty;
            Activity = string.Empty;
            Description = string.Empty;
            Motivation = string.Empty;
            Process = string.Empty;
            Result = string.Empty;

            Participants = "Eduardo Rezende";
        }

        public DateTime Date { get; set; }

        public string Step { get; set; }

        public string Activity { get; set; }

        public string Description { get; set; }

        public string Motivation { get; set; }

        public string Process { get; set; }

        public string Result { get; set; }

        public string Participants { get; }
    }
}
