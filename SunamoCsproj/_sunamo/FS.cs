namespace SunamoCsproj._sunamo;

internal class FS
{
    internal static string WithEndBs(string str)
    {
        return str.TrimEnd('\\') + "\\";
    }
}