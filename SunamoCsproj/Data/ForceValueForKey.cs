// variables names: ok
namespace SunamoCsproj.Data;

// Must be passed to each method individually to maintain clarity about what affects method behavior.
// Passing directly to constructor would save work but would reduce visibility of method dependencies.
public class ForceValueForKey : Dictionary<string, Dictionary<string, string>>
{
}
