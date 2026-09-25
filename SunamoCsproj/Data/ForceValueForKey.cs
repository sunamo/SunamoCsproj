// variables names: ok
namespace SunamoCsproj.Data;

/// <summary>
/// Dictionary for forcing specific values for csproj properties.
/// Each entry maps a property name to a dictionary of key-value pairs.
/// Must be passed to each method individually to maintain clarity about what affects method behavior.
/// Passing directly to constructor would save work but would reduce visibility of method dependencies.
/// </summary>
public class ForceValueForKey : Dictionary<string, Dictionary<string, string>>
{
}