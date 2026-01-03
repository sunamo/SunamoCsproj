// variables names: ok
namespace SunamoCsproj.Results;

/// <summary>
/// Result of parsing preprocessor directives (#if, #elif, etc.) before first code element.
/// </summary>
public class ParseSharpIfToFirstCodeElementResult
{
    /// <summary>
    /// Trimmed lines that contain only text that can be found in AllNamespaces.
    /// All lines that contained only namespace-related text.
    /// </summary>
    public List<string>? FoundedNamespaces { get; set; }

    /// <summary>
    /// Trimmed, but includes ALL lines before first code element.
    /// Needed to determine indexes of #if, #elif, etc.
    /// </summary>
    public List<string>? AllLinesBefore { get; set; }
}