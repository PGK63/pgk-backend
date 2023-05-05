namespace PGK.Application.Common.Extentions
{
    internal static class EnumerableExtentions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
       => self.Select((item, index) => (item, index));
    }
}
