namespace AutoDoc.Validators
{
    internal static class InputValidator
    {
        public static bool TryGetDates(string[] args, out DateTime start, out DateTime end)
        {
            return IsDefaults(args, out start, out end) ||
                   IsValidDates(args, out start, out end) ||
                   IsValidDays(args, out start, out end);
        }

        private static bool IsValidDates(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args is { Length: not 2 })
                return false;

            if (!DateTime.TryParse(args[0], out start) ||
                !DateTime.TryParse(args[1], out end) ||
                start > end)
            {
                return false;
            }

            return true;
        }

        private static bool IsValidDays(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args is { Length: not 1 })
                return false;

            if (!int.TryParse(args[0], out var days) || days < 1)
            {
                return false;
            }

            end = DateTime.Now;
            start = end.AddDays(-days).Date;
            return true;
        }

        private static bool IsDefaults(string[] args, out DateTime start, out DateTime end)
        {
            start = DateTime.MinValue;
            end = DateTime.MaxValue;

            if (args is { Length: > 0 })
                return false;

            start = DateTime.Now.Date;
            end = start.AddDays(1).AddSeconds(-1);
            return true;
        }
    }
}
