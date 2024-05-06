using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj._sunamo;
internal class FS
{
    internal static string WithEndBs(string s)
    {
        return s.TrimEnd('\\') + "\\";
    }
}
