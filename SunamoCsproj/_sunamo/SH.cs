namespace SunamoCsproj._sunamo;

/// <summary>
/// String helper methods.
/// </summary>
internal class SH
{
    /// <summary>
    /// Removes all characters after first occurrence of specified string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="after">The string to search for.</param>
    /// <returns>Substring before first occurrence of 'after', or original string if not found.</returns>
    internal static string RemoveAfterFirst(string input, string after)
    {
        int index = input.IndexOf(after);
        if (index >= 0)
            input = input.Substring(0, index);

        return input;
    }

    /// <summary>
    /// Gets indexes of all lines that match specified predicate.
    /// </summary>
    /// <param name="list">The list of lines to search.</param>
    /// <param name="predicate">The predicate to match lines.</param>
    /// <returns>List of indexes where predicate returns true.</returns>
    internal static List<int> GetIndexesOfLinesStartingWith(List<string> list, Func<string, bool> predicate)
    {
        List<int> allIndices = list.Select((str, i) => new { Str = str, Index = i })
            .Where(x => predicate(x.Str))
            .Select(x => x.Index).ToList();
        return allIndices;
    }

    /// <summary>
    /// Gets indexes of lines that contain any of specified search strings.
    /// </summary>
    /// <param name="list">The list of lines to search.</param>
    /// <param name="searchStrings">The strings to search for.</param>
    /// <returns>List of indexes where lines contain any of the search strings.</returns>
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