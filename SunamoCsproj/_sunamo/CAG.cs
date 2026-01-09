// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// EN: Collection analysis generic helper.
/// </summary>
internal class CAG
{
    /// <summary>
    /// EN: Gets duplicate items from a list.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="list">List to analyze.</param>
    /// <returns>List of duplicate items.</returns>
    internal static List<T> GetDuplicities<T>(List<T> list)
    {
        List<T> alreadyProcessed;
        return GetDuplicities<T>(list, out alreadyProcessed);
    }

    /// <summary>
    /// EN: Gets duplicate items from a list and outputs already processed items.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    /// <param name="list">List to analyze.</param>
    /// <param name="alreadyProcessed">Output list of already processed unique items.</param>
    /// <returns>List of duplicate items.</returns>
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