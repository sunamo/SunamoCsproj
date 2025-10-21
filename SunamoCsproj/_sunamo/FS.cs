// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoCsproj._sunamo;

internal class FS
{
    internal static string WithEndBs(string s)
    {
        return s.TrimEnd('\\') + "\\";
    }
}