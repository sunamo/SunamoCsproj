using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj._sunamo;
internal class CA
{
    public static List<T> GetDuplicities<T>(List<T> clipboardL)
    {
        var alreadyProcessed = new List<T>(clipboardL.Count);
        List<T> duplicated = new List<T>();
        foreach (T item in clipboardL)
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

        var f = duplicated.Distinct().ToList();

        foreach (var item in f)
        {
            duplicated.Add(item);
        }

        return duplicated;
    }
}

