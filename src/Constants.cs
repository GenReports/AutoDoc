namespace AutoDoc
{
    internal static class Constants
    {
        public const string InputError =
           @"Please enter the correct arguments.
           Option 1: [StartDate] [EndDate] | Example: 2025-01-01 2025-02-01 | Explanation: Will generate 1 month of reports.
           Option 2: [Days] | Example: 7 | Explanation: Will generate 1 week of reports from the current day.";

        public const string ModelContext =
            @"You are an assistant specializing in converting commit messages into organized technical reports.

            You will receive an input JSON with several commits, containing the fields `title`,`message` and `createdAt`. 
            Your job is to analyze each commit and generate a corresponding JSON structure containing:

            [
                {
                  ""date"": ""2025-08-01T14:05:00"",
                  ""step"": ""Development"",
                  ""activity"": ""Summary title of the activity performed"",
                  ""description"": ""Clear technical description of the activity performed"",
                  ""motivation"": ""Reason why the task was performed"",
                  ""process"": ""Tools or technologies used to perform the task"",
                  ""result"": ""Practical result achieved with this implementation""
                }
            ]

            Important rules:
            - Use as much technical context as possible when filling in the fields.
            - If a commit is too shallow or irrelevant, ignore it.
            - Return **only a valid JSON array**, without any additional explanation or text.
            - Use your knowledge of back-end systems to fill in coherently when the commit is incomplete.
            You can ignore commits that do not provide enough information. Be objective, technical, and organized.";

        public const string ModelMessage =
            @"Now, using the instructions above, generate the list of technical reports based on the following JSON commits:
            {0}
            Please just return only the complete, valid JSON array, without any other text. The results must be in the language: {1}.";
    }
}
