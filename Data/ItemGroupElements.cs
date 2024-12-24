namespace SunamoCsproj.Data;

public class ItemGroupElements
{
    /// <summary>
    /// V�echny package references
    /// </summary>
    public List<ItemGroupElement> list = [];

    /// <summary>
    /// A1 = Seznam kter� m� m�t v�echno v 1. lize
    /// </summary>
    /// <param name="deps"></param>
    /// <returns></returns>
    public List<string> HaveOnlyDepsFromList(List<string> deps)
    {
        List<string> result = [];
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
