
namespace SunamoCsproj._sunamo;
using System.Collections.Generic;
using System.Linq;


internal class XmlHelper
{
    internal static XmlAttribute foundedNode = null;

    internal static string GetAttributeWithNameValue(XmlNode item, string p)
    {
        foreach (XmlAttribute item2 in item.Attributes)
        {
            if (item2.Name == p)
            {
                foundedNode = item2;
                return item2.InnerXml;
            }
        }

        return null;
    }

    /// <summary>
    /// because return type is Object and can't use item.ChildNodes.First(d => d.) etc.
    /// XmlNodeList dědí jen z IEnumerable, IDisposable
    /// </summary>
    /// <returns></returns>
    internal static List<XmlNode> ChildNodes(XmlNode xml)
    {
        // TODO: až přilinkuji SunamoExtensions tak .COunt
        List<XmlNode> result = new List<XmlNode>();

        foreach (XmlNode item in xml.ChildNodes)
        {
            result.Add(item);
        }

        return result;
    }

    internal static string GetAttrValueOrInnerElement(XmlNode item, string v)
    {
        var attr = item.Attributes[v];

        if (attr != null)
        {
            return attr.Value;
        }

        var childNodes = ChildNodes(item);
        if (childNodes.Count != 0)
        {
            var el = childNodes.First(d => d.Name == v);
            return el?.Value;
        }
        System.Diagnostics.Debugger.Break();
        return null;
    }

    internal static string Attr(XmlNode d, string v)
    {
        var a = GetAttributeWithName(d, v);
        if (a != null)
        {
            return a.Value;
        }
        return null;
    }

    internal static XmlNode GetAttributeWithName(XmlNode item, string p)
    {
        foreach (XmlAttribute item2 in item.Attributes)
        {
            if (item2.Name == p)
            {
                foundedNode = item2;
                return item2;
            }
        }

        return null;
    }
}

