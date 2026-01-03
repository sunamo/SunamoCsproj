// variables names: ok
namespace SunamoCsproj;

partial class CsprojInstance
{
    public void Save()
    {
        JoinMultiPropertyGroupToOne();
        RemoveDuplicatedInNoWarnAndDefineConstant();
        XmlDocument.Save(PathFs);
    }
    private void RemoveDuplicatedInNoWarnAndDefineConstant()
    {
        var defineConstants = XmlDocument.SelectNodes("//DefineConstants");
        var noWarn = XmlDocument.SelectNodes("//NoWarn");
        RemoveDuplicated(defineConstants);
        RemoveDuplicated(noWarn);
    }
    private void RemoveDuplicated(XmlNodeList? nodeList)
    {
        foreach (XmlNode node in nodeList)
        {
            var innerXml = node.InnerXml;
            var parts = innerXml.Split(';').Distinct().ToList();
            node.InnerXml = string.Join(';', parts);
        }
    }
    private void JoinMultiPropertyGroupToOne()
    {
        var propertyGroupNodes = XmlDocument.SelectNodes("/Project/PropertyGroup");
        List<XmlNode> debugNodes = [];
        List<XmlNode> releaseNodes = [];
        foreach (XmlNode propertyGroup in propertyGroupNodes)
        {
            var condition = XmlHelper.GetAttributeWithNameValue(propertyGroup, "Condition");
            if (condition == null)
            {
                continue;
            }
            var conditionValue = condition.Replace(" ", "");
            if (conditionValue == Release)
                releaseNodes.Add(propertyGroup);
            else if (conditionValue == Debug)
                debugNodes.Add(propertyGroup);
        }
        JoinMultiPropertyGroupToOneWorker(debugNodes);
        JoinMultiPropertyGroupToOneWorker(releaseNodes);
    }
    private void JoinMultiPropertyGroupToOneWorker(List<XmlNode> propertyGroups)
    {
        Dictionary<string, string> elements = [];
        foreach (var propertyGroup in propertyGroups)
        {
            foreach (XmlNode childNode in propertyGroup.ChildNodes)
            {
                elements.TryAdd(childNode.Name, childNode.InnerXml);
            }
        }
        foreach (var propertyGroup in propertyGroups)
        {
            propertyGroup.ParentNode?.RemoveChild(propertyGroup);
        }
        foreach (var element in elements)
        {
            AddRemovePropertyGroupItem(true, element.Key, element.Value);
        }
    }
}
