// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// Extension methods for List of string.
/// </summary>
internal static class ListStringExtensions
{
    /// <summary>
    /// Inserts a multiline string into a list at specified index, splitting it into individual lines.
    /// </summary>
    /// <param name="list">The target list to insert into.</param>
    /// <param name="insertIndex">The index where to insert the lines.</param>
    /// <param name="toInsert">The multiline string to insert.</param>
    internal static void InsertMultilineString(this List<string> list, int insertIndex, string toInsert)
    {
        var lines = SHGetLines.GetLines(toInsert);
        for (int index = lines.Count - 1; index >= 0; index--)
        {
            list.Insert(insertIndex, lines[index]);
        }
    }
}