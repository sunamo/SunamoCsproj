using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj._sunamo;
internal class SH
{
    /// <summary>
    /// Přesunout poté do SH
    /// Tam jsem teď nemohl udělat ani unit testy
    /// </summary>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static List<int> GetIndexesOfLinesStartingWith(List<string> list, Func<string, bool> predicate)
    {
        List<int> allIndices = list.Select((s, i) => new { Str = s, Index = i })
            .Where(x => predicate(x.Str))
            .Select(x => x.Index).ToList();
        return allIndices;
    }

    public static List<int> GetIndexesOfLinesWhichContainsAnyOfStrings(List<string> list, List<string> whichMustContains)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (whichMustContains.Contains(list[i]))
            {
                result.Add(i);
            }
        }

        return result;
    }
}
