namespace SunamoCsproj._sunamo;

internal static class ListStringExtensions
{
    internal static void InsertMultilineString(this List<string> list, int insertIndex, string toInsert)
    {
        var lines = SHGetLines.GetLines(toInsert);
        for (int index = lines.Count - 1; index >= 0; index--)
        {
            list.Insert(insertIndex, lines[index]);
        }
    }
}