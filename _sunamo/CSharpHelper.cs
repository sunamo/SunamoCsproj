namespace SunamoCsproj._sunamo;
internal class CSharpHelper
{
    internal static string StripComments(string code)
    {
        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        return Regex.Replace(code, re, "$1");
    }
}
