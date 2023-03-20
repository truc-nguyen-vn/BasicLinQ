using System.Collections;

namespace BasicLinQ.Operators
{
    public static class FilteringData
    {
        public static IEnumerable<T> WhereWithTerm<T>(this IQueryable<T> source, Func<T, bool> predicate)
        {
            #region Log Where()
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Filter by Where():");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion

            //return from item in source
            //       where predicate
            //       select item;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> CheckOfType<T>(this IEnumerable source)
        {
            //var product = 
            return source.OfType<T>();
        }
    }
}
