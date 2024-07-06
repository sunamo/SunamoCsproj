namespace SunamoCsproj._sunamo;


internal class ThrowEx
{
    internal static void Custom(string v)
    {
        throw new Exception(v);
    }

    internal static void NotImplementedMethod()
    {
        Custom("Method not implemented: " + Environment.NewLine + Environment.StackTrace);
    }
}
