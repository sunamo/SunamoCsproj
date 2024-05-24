using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj.Extensions;
internal static class StringExtensions
{
    public static bool ContainsNullAllow(this string d, string contains)
    {
        if (d == null)
        {
            return false;
        }

        return d.Contains(contains);
    }
}
