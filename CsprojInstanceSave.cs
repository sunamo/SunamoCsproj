namespace SunamoCsproj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class CsprojInstance
{
    public void Save()
    {
        JoinMultiPropertyGroupToOne();

        RemoveDuplicatedInNoWarnAndDefineConstant();

        xd.Save(PathFs);
    }

    private void RemoveDuplicatedInNoWarnAndDefineConstant()
    {
        var defineConstants = xd.SelectNodes("//DefineConstants");
        var noWarn = xd.SelectNodes("//NoWarn");

        RemoveDuplicated(defineConstants);
        RemoveDuplicated(noWarn);

    }

    private void RemoveDuplicated(XmlNodeList? noWarn)
    {
        foreach (XmlNode item in noWarn)
        {
            var inner = item.InnerXml;
            var parts = inner.Split(';').Distinct().ToList();

            item.InnerXml = string.Join(';', parts);
        }
    }

    private void JoinMultiPropertyGroupToOne()
    {
        var nodes = xd.SelectNodes("/Project/PropertyGroup");

        List<XmlNode> debug = new();
        List<XmlNode> release = new();

        foreach (XmlNode item in nodes)
        {
            var d = XmlHelper.GetAttributeWithNameValue(item, "Condition");
            if (d == Release)
                release.Add(item);
            else if (d == Debug)
                debug.Add(item);
        }

        JoinMultiPropertyGroupToOneWorker(debug);
        JoinMultiPropertyGroupToOneWorker(release);
    }

    private void JoinMultiPropertyGroupToOneWorker(List<XmlNode> debug)
    {
        Dictionary<string, string> elements = new();

        foreach (var item in debug)
        {
            foreach (XmlNode item2 in item.ChildNodes)
            {
                elements.TryAdd(item2.Name, item2.InnerXml);
            }
        }

        foreach (var item in debug)
        {
            if (item.ParentNode != null)
            {
                item.ParentNode.RemoveChild(item);
            }
        }

        foreach (var item in elements)
        {
            AddRemovePropertyGroupItem(true, item.Key, item.Value);
        }
    }
}