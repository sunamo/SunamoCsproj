namespace SunamoCsproj._sunamo;


internal class DictionaryHelper
{
    public static List<string> GetListStringFromDictionary(Dictionary<string, string> p)
    {
        List<string> vr = new List<string>();

        foreach (var item in p)
        {
            vr.Add(item.Key);
            vr.Add(item.Value);
        }

        return vr;
    }
}
