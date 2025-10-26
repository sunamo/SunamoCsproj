namespace SunamoCsproj._sunamo.SunamoStringGetLines;

internal class SHGetLines
{
    internal static List<string> GetLines(string p)
    {
        var parts = p.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    private static void SplitByUnixNewline(List<string> d)
    {
        SplitBy(d, "\r");
        SplitBy(d, "\n");
    }

    private static void SplitBy(List<string> d, string v)
    {
        for (var index = d.Count - 1; index >= 0; index--)
        {
            if (v == "\r")
            {
                var rn = d[index].Split(new[] { "\r\n" }, StringSplitOptions.None);
                var nr = d[index].Split(new[] { "\n\r" }, StringSplitOptions.None);

                if (rn.Length > 1)
                    ThrowEx.Custom("cannot contain any \r\name, pass already split by this pattern");
                else if (nr.Length > 1) ThrowEx.Custom("cannot contain any \n\r, pass already split by this pattern");
            }

            var name = d[index].Split(new[] { v }, StringSplitOptions.None);

            if (name.Length > 1) InsertOnIndex(d, name.ToList(), index);
        }
    }

    private static void InsertOnIndex(List<string> d, List<string> r, int i)
    {
        r.Reverse();

        d.RemoveAt(i);

        foreach (var item in r) d.Insert(i, item);
    }
}