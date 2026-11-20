namespace SunamoCsproj.Data;

/// <summary>
/// Represents the item group elements type.
/// </summary>
public class ItemGroupElements
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    public List<ItemGroupElement> List { get; set; } = [];

    // Returns items that are NOT in the specified allowed list.
    // Used to find references that should not be present.
    /// <summary>
    /// Have only deps from list.
    /// </summary>
    /// <param name="allowedDependencies">The allowed dependencies.</param>
    /// <returns>The have only deps from list result.</returns>
    public List<string> HaveOnlyDepsFromList(List<string> allowedDependencies)
    {
        List<string> result = [];
        foreach (var item in List)
        {
            if (!allowedDependencies.Contains(item.Include!))
            {
                result.Add(item.Include!);
            }
        }

        return result;
    }
}
