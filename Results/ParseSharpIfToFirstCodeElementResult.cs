using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj.Results;
public class ParseSharpIfToFirstCodeElementResult
{
    /// <summary>
    /// trimmed
    /// 
    /// všechny řádky které obsahovali pouze text který lze najít i ve AllNamespacess
    /// </summary>
    public List<string> foundedNamespaces;
    /// <summary>
    /// trimmed
    /// 
    /// opravdu všechny řádky
    /// je to třeba k určení indexů #if, #elif atd.
    /// </summary>
    public List<string> allLinesBefore;
    public bool IsGeneric;
}
