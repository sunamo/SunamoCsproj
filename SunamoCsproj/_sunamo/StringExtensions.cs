// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoCsproj._sunamo;

internal static class StringExtensions
{
    internal static bool ContainsNullAllow(this string d, string contains)
    {
        if (d == null)
        {
            return false;
        }
        return d.Contains(contains);
    }
}
