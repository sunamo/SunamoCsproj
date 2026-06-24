namespace SunamoCsproj.Data;

public class ItemGroupElements
{
    public List<ItemGroupElement> List { get; set; } = [];

    // Returns items that are NOT in the specified allowed list.
    // Used to find references that should not be present.
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
