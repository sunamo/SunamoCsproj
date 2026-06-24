namespace SunamoCsproj._sunamo;

internal class XHelper
{
    internal static string FormatXmlInMemory(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }
}
