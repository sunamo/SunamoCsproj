namespace SunamoCsproj._sunamo;

internal class XmlHelper
{
    internal static XmlAttribute? FoundedNode = null;

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

    // EN: Converts XmlNodeList to List of XmlNode. XmlNodeList only inherits from IEnumerable and IDisposable.
    internal static List<XmlNode> ChildNodes(XmlNode node)
    {
        var result = new List<XmlNode>();

        foreach (XmlNode item in node.ChildNodes)
        {
            result.Add(item);
        }

        return result;
    }

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

    internal static string? Attr(XmlNode node, string attributeName)
    {
        var attribute = GetAttributeWithName(node, attributeName);
        if (attribute != null)
        {
            return attribute.Value;
        }
        return null;
    }

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
