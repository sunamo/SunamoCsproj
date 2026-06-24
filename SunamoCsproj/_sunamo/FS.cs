namespace SunamoCsproj._sunamo;

internal class FS
{
    internal static string WithEndBs(string path) => path.TrimEnd('\\') + "\\";
}