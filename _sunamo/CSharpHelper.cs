namespace SunamoCsproj._sunamo;
internal class CSharpHelper
{
    public static string StripComments(string code)
    {
        var re = @"(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|//.*|/\*(?s:.*?)\*/";
        return Regex.Replace(code, re, "$1");
    }
}
