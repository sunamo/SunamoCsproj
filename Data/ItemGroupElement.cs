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
        List<string> ls = [];
        if (Include != null)
        {
            ls.Add(CsprojConsts.Include);
            ls.Add(Include);
        }

        if (Version != null)
        {
            ls.Add(CsprojConsts.Version);
            ls.Add(Version);
        }

        if (Link != null)
        {
            ls.Add(CsprojConsts.Link);
            ls.Add(Link);
        }

        XmlGenerator xd = new XmlGenerator();
        xd.WriteTagWithAttrs(ItemGroupTagName.ToString(), ls);
        return xd.ToString();
    }

    public static ItemGroupElement Parse(XmlNode item)
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
        var xd = itemGroup.OwnerDocument;
        var d = xd.CreateElement(ItemGroupTagName.ToString());

        if (Include != null)
        {
            var includeAttr = xd.CreateAttribute(CsprojConsts.Include);
            includeAttr.Value = Include;
            d.Attributes.Append(includeAttr);
        }

        if (Version != null)
        {
            var versionAttr = xd.CreateAttribute(CsprojConsts.Version);
            versionAttr.Value = Version;
            d.Attributes.Append(versionAttr);
        }

        if (Link != null)
        {
            var linkAttr = xd.CreateAttribute(CsprojConsts.Link);
            linkAttr.Value = Link;
            d.Attributes.Append(linkAttr);
        }

        itemGroup.AppendChild(d);
    }
}
