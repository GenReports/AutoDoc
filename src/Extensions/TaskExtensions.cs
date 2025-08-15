namespace AutoDoc.Extensions
{
    internal static class TaskExtensions
    {
        public static async Task DelayAsync(
            this int count,
            int multiplier,
            CancellationToken ct)
        {
            await Task.Delay(count * multiplier, ct);
        }
    }
}
