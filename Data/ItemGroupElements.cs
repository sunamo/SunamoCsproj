namespace SunamoCsproj.Data;
public class ItemGroupElements
{
    public List<ItemGroupElement> list = new List<ItemGroupElement>();

    public bool HaveOnlyDepsFromList(List<string> deps)
    {
        foreach (var item in list)
        {
            if (!deps.Contains(item.Include))
            {
                return false;
            }
        }

        return true;
    }
}
