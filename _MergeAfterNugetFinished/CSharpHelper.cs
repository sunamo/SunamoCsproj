using System.Text.RegularExpressions;

namespace SunamoCsproj._MergeAfterNugetFinished;
public class CSharpHelper
{
    public static string StripComments(string code)
    {
        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        return Regex.Replace(code, re, "$1");
    }
}
