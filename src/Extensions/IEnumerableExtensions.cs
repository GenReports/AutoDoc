namespace AutoDoc.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> values)
        {
            if (values is null || !values.Any())
                return [];

            var totalCount = values.Count();

            if (totalCount < 3)
                return [[.. values]];

            return totalCount < 10
                ? values.Chunk(totalCount / 3)
                : values.Chunk(totalCount / 5);
        }
    }
}
