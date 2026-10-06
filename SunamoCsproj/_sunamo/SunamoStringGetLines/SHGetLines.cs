namespace SunamoCsproj._sunamo.SunamoStringGetLines;

internal class SHGetLines
{
    internal static List<string> GetLines(string text)
    {
        var parts = text.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    private static void SplitByUnixNewline(List<string> lines)
    {
        SplitBy(lines, "\r");
        SplitBy(lines, "\n");
    }

    private static void SplitBy(List<string> lines, string delimiter)
    {
        for (var index = lines.Count - 1; index >= 0; index--)
        {
            if (delimiter == "\r")
            {
                var carriageReturnNewline = lines[index].Split(new[] { "\r\n" }, StringSplitOptions.None);
                var newlineCarriageReturn = lines[index].Split(new[] { "\n\r" }, StringSplitOptions.None);

                if (carriageReturnNewline.Length > 1)
                    ThrowEx.Custom("cannot contain any \r\n, pass already split by this pattern");
                else if (newlineCarriageReturn.Length > 1)
                    ThrowEx.Custom("cannot contain any \n\r, pass already split by this pattern");
            }

            var splitResult = lines[index].Split(new[] { delimiter }, StringSplitOptions.None);

            if (splitResult.Length > 1)
                InsertOnIndex(lines, splitResult.ToList(), index);
        }
    }

    private static void InsertOnIndex(List<string> lines, List<string> insertItems, int index)
    {
        insertItems.Reverse();

        lines.RemoveAt(index);

        foreach (var item in insertItems)
            lines.Insert(index, item);
    }
}
