
using BasicLinQ.Entities;

namespace BasicLinQ.Operators
{
    public static class SortingData
    {
        public static IQueryable<Product> AscendingSort(this IQueryable<Product>? source)
        {
            return source.OrderBy(x => x.Id);
        }
        public static IQueryable<Product> DescendingSort(this IQueryable<Product>? source)
        {
            
            return from src in source
                   orderby src.Id descending
                   select src;
        }

        public static IQueryable<Product> AscendingThenByDescendingSort(this IQueryable<Product>? source)
        {
            return source.OrderBy(x => x.CategoryId).ThenByDescending(x => x.Name);
        }
        public static IQueryable<Product> DescendingThenByAscendingSort(this IQueryable<Product>? source)
        {
            return from src in source
                   orderby src.CategoryId descending, src.Name
                   select src;
        }
    }
}
