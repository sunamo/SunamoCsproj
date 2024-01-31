namespace SunamoCsproj.Data;
public class ItemGroupElements
{
    /// <summary>
    /// Všechny package references
    /// </summary>
    public List<ItemGroupElement> list = new List<ItemGroupElement>();

    /// <summary>
    /// A1 = Seznam který má mít všechno v 1. lize
    /// </summary>
    /// <param name="deps"></param>
    /// <returns></returns>
    public List<string> HaveOnlyDepsFromList(List<string> deps)
    {
        List<string> result = new List<string>();
        foreach (var item in list)
        {
            if (!deps.Contains(item.Include))
            {
                result.Add(item.Include);
            }
        }

        return result;
    }
}
