namespace SunamoCsproj._sunamo;

internal class CAG
{
    internal static List<T> GetDuplicities<T>(List<T> list)
    {
        return GetDuplicities<T>(list, out _);
    }

    internal static List<T> GetDuplicities<T>(List<T> list, out List<T> alreadyProcessed)
    {
        alreadyProcessed = new List<T>(list.Count);
        List<T> duplicated = [];
        foreach (var item in list)
        {
            if (alreadyProcessed.Contains(item))
            {
                duplicated.Add(item);
            }
            else
            {
                alreadyProcessed.Add(item);
            }
        }
        return duplicated.Distinct().ToList();
    }
}