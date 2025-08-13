namespace AutoDoc.Extensions
{
    internal static class TaskExtensions
    {
        public static async Task DelayAsync(
            this int count,
            int multiplier = 1000,
            CancellationToken ct = default)
        {
            await Task.Delay(count * multiplier, ct);
        }
    }
}
