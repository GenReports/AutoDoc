using System.Reflection;
using System.Linq.Expressions;

namespace AutoDoc.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> values)
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

        public static IEnumerable<T> Set<T, TProperty>(
            this IEnumerable<T> contents,
            Expression<Func<T, TProperty>> expression,
            TProperty value)
        {
            if (contents is null || !contents.Any())
                return [];

            if (expression.Body is not MemberExpression memberExpr)
                return [];

            if (memberExpr.Member is not PropertyInfo propInfo)
                return [];

            foreach (var content in contents)
                propInfo.SetValue(content, value);

            return contents;
        }
    }
}
