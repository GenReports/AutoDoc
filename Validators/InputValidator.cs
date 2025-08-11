namespace AutoDoc.Validators
{
    internal static class InputValidator
    {
        public static bool IsValid(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args is null)
                return false;

            if (args.Length is 2)
                return ValidateDates(args, out start, out end);

            if (args.Length is 1)
                return ValidateDays(args, out start, out end);

            return true;
        }

        private static bool ValidateDates(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args == null || args.Length != 2)
                return false;

            if (!DateTime.TryParse(args[0], out start) ||
                !DateTime.TryParse(args[1], out end) ||
                start > end)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateDays(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args == null || args.Length != 1)
                return false;

            if (!int.TryParse(args[0], out var days) || days < 1)
            {
                return false;
            }

            end = DateTime.Now.Date;
            start = end.AddDays(-days);
            return true;
        }
    }
}
