namespace System.Linq
{
    public static class IOrderedQueryableExtensions
    {
        public static IQueryable<T> PageBy<T>(this IOrderedQueryable<T> items, int pageNumber, int pageSize)
        {
            return items
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
    }
}