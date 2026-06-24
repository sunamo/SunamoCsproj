namespace SunamoCsproj._sunamo;

internal static class StringExtensions
{
    internal static bool ContainsNullAllow(this string? text, string searchValue)
    {
        if (text is null)
            return false;
        return text.Contains(searchValue);
    }
}