// variables names: ok
namespace SunamoCsproj.Data;

/// <summary>
/// Collection of ItemGroup elements from csproj file.
/// </summary>
public class ItemGroupElements
{
    /// <summary>
    /// All package or project references.
    /// </summary>
    public List<ItemGroupElement> List { get; set; } = [];

    /// <summary>
    /// Returns items that are NOT in the specified allowed list.
    /// Used to find references that should not be present.
    /// </summary>
    /// <param name="allowedDependencies">List of allowed dependency names.</param>
    /// <returns>List of dependency names that are not in the allowed list.</returns>
    public List<string> HaveOnlyDepsFromList(List<string> allowedDependencies)
    {
        List<string> result = [];
        foreach (var item in List)
        {
            if (!allowedDependencies.Contains(item.Include))
            {
                result.Add(item.Include);
            }
        }

        return result;
    }
}