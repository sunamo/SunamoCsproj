namespace SunamoCsproj;

/// <summary>
/// Zde jsou ty co používají xd
/// => snadno
/// </summary>
public class CsprojInstance : CsprojConsts
{
    public XmlDocument xd { get; set; }

    public CsprojInstance(string path)
    {
        this.xd = new XmlDocument();
        xd.LoadXml(path);
    }

    public CsprojInstance()
    {

    }

    public void RemoveSingleItemGroup(string attrValue, ItemGroupTagName tagName)
    {
        var t = xd.SelectSingleNode($"/Project/ItemGroup/{tagName}[@{Include} = '{attrValue}']");
        t.ParentNode?.RemoveChild(t);
    }

    public void CreateElementInPropertyGroupWhichDoesNotExists()
    {
        // Používat na to MSBuild jako mám v MsBuildTarget

    }

    public XmlElement CreateNewPackageReference(string include, string version)
    {
        return CreateNewItemGroupElement(ItemGroupTagName.PackageReference, include, version);
    }

    /// <summary>
    /// Vrací ale jinak bude i v xml
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="include"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public XmlElement CreateNewItemGroupElement(ItemGroupTagName tagName, string include, string version)
    {
        var newEl = xd.CreateElement(ItemGroupTagName.PackageReference.ToString());
        if (include != null)
        {
            var attr = CreateAttr(newEl, Include, include);
            newEl.Attributes.Append(attr);
        }

        if (version != null)
        {
            var attrVersion = CreateAttr(newEl, Version, "*");
            newEl.Attributes.Append(attrVersion);
        }

        return newEl;
    }

    private XmlAttribute CreateAttr(XmlElement newEl, string attrName, string attrValue)
    {
        var attr = xd.CreateAttribute(attrName);
        attr.Value = attrValue;

        var newAttr = newEl.Attributes.Append(attr);
        return newAttr;
    }
}
