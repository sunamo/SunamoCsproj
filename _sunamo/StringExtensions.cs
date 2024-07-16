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
