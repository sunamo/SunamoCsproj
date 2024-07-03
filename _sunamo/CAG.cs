namespace SunamoCsproj._sunamo;

internal class CAG
{
    internal static List<T> GetDuplicities<T>(List<T> clipboardL)
    {
        List<T> alreadyProcessed;
        return GetDuplicities<T>(clipboardL, out alreadyProcessed);
    }

    internal static List<T> GetDuplicities<T>(List<T> clipboardL, out List<T> alreadyProcessed)
    {
        alreadyProcessed = new List<T>(clipboardL.Count);
        List<T> duplicated = new List<T>();
        foreach (var item in clipboardL)
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
