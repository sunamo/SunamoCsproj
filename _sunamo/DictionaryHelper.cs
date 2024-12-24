namespace SunamoCsproj._sunamo;
internal class DictionaryHelper
{
    internal static List<string> GetListStringFromDictionary(Dictionary<string, string> p)
    {
        List<string> vr = [];
        foreach (var item in p)
        {
            vr.Add(item.Key);
            vr.Add(item.Value);
        }
        return vr;
    }
}
