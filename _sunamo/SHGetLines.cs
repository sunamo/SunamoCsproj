namespace SunamoCsproj._sunamo;
internal class SHGetLines
{
    internal static List<string> GetLines(string v)
    {
        return v.Split(new String[] { v.Contains("\r\n") ? "\r\n" : "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
