namespace SunamoCsproj.Results;

/// <summary>
/// Represents the parse sharp if to first code element result type.
/// </summary>
public class ParseSharpIfToFirstCodeElementResult
{
    /// <summary>
    /// Gets or sets the founded namespaces.
    /// </summary>
    public List<string>? FoundedNamespaces { get; set; }

    /// <summary>
    /// Gets or sets the all lines before.
    /// </summary>
    public List<string>? AllLinesBefore { get; set; }
}