// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// Helper methods for XML manipulation.
/// </summary>
internal class XHelper
{
    /// <summary>
    /// Formats XML string with proper indentation. Returns original string if parsing fails.
    /// </summary>
    /// <param name="xml">The XML string to format.</param>
    /// <returns>Formatted XML string, or original string if parsing failed.</returns>
    internal static string FormatXmlInMemory(string xml)
    {
        try
        {
            XDocument doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }
}