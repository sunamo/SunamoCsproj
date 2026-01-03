// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// EN: File system helper methods.
/// </summary>
internal class FS
{
    /// <summary>
    /// EN: Ensures path ends with backslash.
    /// </summary>
    /// <param name="path">Path to process.</param>
    /// <returns>Path with trailing backslash.</returns>
    internal static string WithEndBs(string path)
    {
        return path.TrimEnd('\\') + "\\";
    }
}
