// variables names: ok
namespace SunamoCsproj._sunamo.SunamoStringGetLines;

/// <summary>
/// Helper class for splitting strings into lines by various newline delimiters.
/// </summary>
internal class SHGetLines
{
    /// <summary>
    /// Splits text into lines by all types of newline characters (CRLF, LFCR, CR, LF).
    /// </summary>
    /// <param name="text">The text to split into lines.</param>
    /// <returns>List of lines from the text.</returns>
    internal static List<string> GetLines(string text)
    {
        var parts = text.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    /// <summary>
    /// Splits lines by Unix-style newline characters (CR and LF).
    /// </summary>
    /// <param name="lines">The list of lines to further split.</param>
    private static void SplitByUnixNewline(List<string> lines)
    {
        SplitBy(lines, "\r");
        SplitBy(lines, "\n");
    }

    /// <summary>
    /// Splits lines by specified delimiter, expanding the list in-place.
    /// </summary>
    /// <param name="lines">The list of lines to split.</param>
    /// <param name="delimiter">The delimiter to split by.</param>
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

    /// <summary>
    /// Inserts items at specified index, replacing the original item.
    /// </summary>
    /// <param name="lines">The target list to insert into.</param>
    /// <param name="insertItems">The items to insert.</param>
    /// <param name="index">The index where to insert items.</param>
    private static void InsertOnIndex(List<string> lines, List<string> insertItems, int index)
    {
        insertItems.Reverse();

        lines.RemoveAt(index);

        foreach (var item in insertItems)
            lines.Insert(index, item);
    }
}