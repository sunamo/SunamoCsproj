namespace SunamoCsproj._sunamo;
internal static class ListStringExtensions
{
    internal static void InsertMultilineString(this List<string> l, int dx, string toInsert)
    {
        var lines = SHGetLines.GetLines(toInsert);

        for (int i = lines.Count - 1; i >= 0; i--)
        {
            l.Insert(dx, lines[i]);
        }
    }
}
