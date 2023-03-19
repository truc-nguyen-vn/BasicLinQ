using BasicLinQ.Entities;
using ConsoleTables;

namespace BasicLinQ
{
    public static class Helper
    {
        public static void LogListData<T>(IEnumerable<T> list)
        {
            ConsoleTable.From(list).Write();
        }
    }
}
