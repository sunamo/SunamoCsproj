namespace SunamoCsproj._sunamo;

internal class SH
{
    internal static string RemoveAfterFirst(string input, string after)
    {
        int index = input.IndexOf(after);
        if (index >= 0)
            input = input.Substring(0, index);

        return input;
    }



    /// <summary>
    /// Přesunout poté do SH
    /// Tam jsem teď nemohl udělat ani unit testy
    /// </summary>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    internal static List<int> GetIndexesOfLinesStartingWith(List<string> list, Func<string, bool> predicate)
    {
        List<int> allIndices = list.Select((str, i) => new { Str = str, Index = i })
            .Where(x => predicate(x.Str))
            .Select(x => x.Index).ToList();
        return allIndices;
    }

    internal static List<int> GetIndexesOfLinesWhichContainsAnyOfStrings(List<string> list, List<string> whichMustContains)
    {
        List<int> result = [];
        for (int i = 0; i < list.Count; i++)
        {
            if (whichMustContains.Contains(list[i]))
            {
                result.Add(i);
            }
        }

        return result;
    }
}