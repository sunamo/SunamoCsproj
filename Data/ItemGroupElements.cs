namespace SunamoCsproj.Data;
public class ItemGroupElements
{
    public List<ItemGroupElement> list = new List<ItemGroupElement>();

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
