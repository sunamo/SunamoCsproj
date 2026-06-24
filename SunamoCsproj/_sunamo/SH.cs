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

    internal static List<int> GetIndexesOfLinesStartingWith(List<string> list, Func<string, bool> predicate)
    {
        var allIndices = list.Select((str, i) => new { Str = str, Index = i })
            .Where(x => predicate(x.Str))
            .Select(x => x.Index).ToList();
        return allIndices;
    }

    internal static List<int> GetIndexesOfLinesWhichContainsAnyOfStrings(List<string> list, List<string> searchStrings)
    {
        List<int> result = [];
        for (int i = 0; i < list.Count; i++)
        {
            if (searchStrings.Contains(list[i]))
            {
                result.Add(i);
            }
        }

        return result;
    }
}