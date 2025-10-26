namespace SunamoCsproj.Results;

// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
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
    //public bool IsGeneric;
}