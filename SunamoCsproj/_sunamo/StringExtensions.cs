namespace SunamoCsproj._sunamo;

/// <summary>
/// Extension methods for string.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Checks if string contains specified substring, returning false if string is null.
    /// </summary>
    /// <param name="text">The text to check.</param>
    /// <param name="searchValue">The value to search for.</param>
    /// <returns>True if text contains searchValue, false if text is null or doesn't contain searchValue.</returns>
    internal static bool ContainsNullAllow(this string text, string searchValue)
    {
        if (text == null)
        {
            return false;
        }
        return text.Contains(searchValue);
    }
}