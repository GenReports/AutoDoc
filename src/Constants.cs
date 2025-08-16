namespace AutoDoc
{
    internal static class Constants
    {
        public const string InputError =
           @"Please enter the correct arguments.
           Option 1: [StartDate] [EndDate] | Example: 2025-01-01 2025-02-01 | Explanation: Will generate 1 month of reports.
           Option 2: [Days] | Example: 7 | Explanation: Will generate 1 week of reports from the current day.";

        public const string ModelMessage =
            @"Now, using the instructions above, generate the list of technical reports based on the following JSON commits:
            {0}
            Please just return only the complete, valid JSON array, without any other text. The results must be in the language: {1}.";
    }
}
