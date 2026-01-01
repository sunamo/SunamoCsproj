namespace SunamoCsproj.Data;

public class ItemGroupElement
{
    public string Include { get; set; }
    public string Version { get; set; }
    public string Link { get; set; }
    public XmlNode XmlNode { get; set; }
    public ItemGroupTagName ItemGroupTagName { get; set; }

    public override string ToString()
    {
        List<string> attributes = [];

        if (Include != null)
        {
            attributes.Add(CsprojConsts.Include);
            attributes.Add(Include);
        }

        if (Version != null)
        {
            attributes.Add(CsprojConsts.Version);
            attributes.Add(Version);
        }

        if (Link != null)
        {
            attributes.Add(CsprojConsts.Link);
            attributes.Add(Link);
        }

        XmlGenerator xmlGenerator = new XmlGenerator();
        xmlGenerator.WriteTagWithAttrs(ItemGroupTagName.ToString(), attributes);
        return xmlGenerator.ToString();
    }

    public static ItemGroupElement? Parse(XmlNode item)
    {
        var tagName = item.Name;
        if (!Enum.TryParse<ItemGroupTagName>(tagName, false, out var itemGroupTagName))
        {
            return null;
        }

        ItemGroupElement ige = new ItemGroupElement();
        ige.Include = XmlHelper.Attr(item, CsprojInstance.Include);
        ige.Version = XmlHelper.Attr(item, CsprojInstance.Version);
        ige.Link = XmlHelper.Attr(item, CsprojInstance.Link);
        ige.XmlNode = item;
        ige.ItemGroupTagName = itemGroupTagName;

        return ige;
    }

    /// <summary>
    /// Přidám this do A1
    /// </summary>
    /// <param name="itemGroup"></param>
    public void AddToItemGroup(XmlElement itemGroup)
    {
        var document = itemGroup.OwnerDocument;
        var element = document.CreateElement(ItemGroupTagName.ToString());

        if (Include != null)
        {
            var includeAttr = document.CreateAttribute(CsprojConsts.Include);
            includeAttr.Value = Include;
            element.Attributes.Append(includeAttr);
        }

        if (Version != null)
        {
            var versionAttr = document.CreateAttribute(CsprojConsts.Version);
            versionAttr.Value = Version;
            element.Attributes.Append(versionAttr);
        }

        if (Link != null)
        {
            var linkAttr = document.CreateAttribute(CsprojConsts.Link);
            linkAttr.Value = Link;
            element.Attributes.Append(linkAttr);
        }

        itemGroup.AppendChild(element);
    }
}