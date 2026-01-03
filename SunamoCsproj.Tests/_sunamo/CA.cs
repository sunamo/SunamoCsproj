// variables names: ok
namespace SunamoCsproj.Tests._sunamo;
internal class CA
{
    public static List<string> Trim(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].Trim();
        }

        return list;
    }
}
