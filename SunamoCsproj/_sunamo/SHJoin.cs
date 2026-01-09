// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// Helper class for joining strings.
/// </summary>
internal class SHJoin
{
    /// <summary>
    /// Joins list of strings with newline character.
    /// </summary>
    /// <param name="lines">The lines to join.</param>
    /// <returns>Joined string with newline separators.</returns>
    internal static string JoinNL(List<string> lines)
    {
        return string.Join('\n', lines);
    }
}