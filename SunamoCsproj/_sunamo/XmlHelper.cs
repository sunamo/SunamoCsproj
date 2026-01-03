// variables names: ok
namespace SunamoCsproj._sunamo;

/// <summary>
/// EN: XML helper methods using XmlDocument/XmlNode.
/// </summary>
internal class XmlHelper
{
    internal static XmlAttribute? FoundedNode = null;

    /// <summary>
    /// EN: Gets attribute value by name.
    /// </summary>
    /// <param name="node">XML node to search.</param>
    /// <param name="attributeName">Attribute name to find.</param>
    /// <returns>Attribute value or null.</returns>
    internal static string? GetAttributeWithNameValue(XmlNode node, string attributeName)
    {
        foreach (XmlAttribute attribute in node.Attributes!)
        {
            if (attribute.Name == attributeName)
            {
                FoundedNode = attribute;
                return attribute.InnerXml;
            }
        }

        return null;
    }

    /// <summary>
    /// EN: Converts XmlNodeList to List of XmlNode. XmlNodeList only inherits from IEnumerable and IDisposable.
    /// </summary>
    /// <param name="node">Parent XML node.</param>
    /// <returns>List of child nodes.</returns>
    internal static List<XmlNode> ChildNodes(XmlNode node)
    {
        List<XmlNode> result = new List<XmlNode>();

        foreach (XmlNode item in node.ChildNodes)
        {
            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// EN: Gets attribute value or inner element value by name.
    /// </summary>
    /// <param name="node">XML node to search.</param>
    /// <param name="name">Attribute or element name.</param>
    /// <returns>Value or null.</returns>
    internal static string? GetAttrValueOrInnerElement(XmlNode node, string name)
    {
        var attr = node.Attributes![name];

        if (attr != null)
        {
            return attr.Value;
        }

        var childNodes = ChildNodes(node);
        if (childNodes.Count != 0)
        {
            var el = childNodes.First(child => child.Name == name);
            return el?.Value;
        }
        System.Diagnostics.Debugger.Break();
        return null;
    }

    /// <summary>
    /// EN: Gets attribute value by name.
    /// </summary>
    /// <param name="node">XML node to search.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <returns>Attribute value or null.</returns>
    internal static string? Attr(XmlNode node, string attributeName)
    {
        var attribute = GetAttributeWithName(node, attributeName);
        if (attribute != null)
        {
            return attribute.Value;
        }
        return null;
    }

    /// <summary>
    /// EN: Gets attribute node by name.
    /// </summary>
    /// <param name="node">XML node to search.</param>
    /// <param name="attributeName">Attribute name.</param>
    /// <returns>Attribute node or null.</returns>
    internal static XmlNode? GetAttributeWithName(XmlNode node, string attributeName)
    {
        foreach (XmlAttribute attribute in node.Attributes!)
        {
            if (attribute.Name == attributeName)
            {
                FoundedNode = attribute;
                return attribute;
            }
        }

        return null;
    }
}
