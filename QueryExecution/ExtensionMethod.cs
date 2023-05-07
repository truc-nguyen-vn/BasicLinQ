
namespace QueryExecution
{
    public static class ExtensionMethod
    {
        public static IEnumerable<T> WhereExecution<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            #region Log Where()
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Filter by WhereExecution():");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion
            int index = -1;
            foreach (T element in source)
            {
                index++;
                Console.WriteLine("Element " + index);

                if (predicate(element))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<T> WhereExecutionWithoutLog<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            #region Log Where()
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Filter by WhereExecutionWithoutLog():");
            Console.ForegroundColor = ConsoleColor.Gray;
            #endregion
            foreach (T element in source)
            {
                if (predicate(element))
                {
                    yield return element;
                }
            }
        }
    }
}
