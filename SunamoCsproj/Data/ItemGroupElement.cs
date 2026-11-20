namespace SunamoCsproj.Data;

/// <summary>
/// Represents the item group element type.
/// </summary>
public class ItemGroupElement
{
    /// <summary>
    /// Gets or sets the include.
    /// </summary>
    public string? Include { get; set; }

    /// <summary>
    /// Gets or sets the version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the link.
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets the xml node.
    /// </summary>
    public XmlNode? XmlNode { get; set; }

    /// <summary>
    /// Gets or sets the item group tag name.
    /// </summary>
    public ItemGroupTagName ItemGroupTagName { get; set; }

    /// <summary>
    /// To string.
    /// </summary>
    /// <returns>The to string result.</returns>
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

    /// <summary>
    /// Initializes a new instance of the parse class.
    /// </summary>
    /// <param name="CsprojConsts.Version">The csproj consts version.</param>
    public static ItemGroupElement? Parse(XmlNode item)
    {
        var tagName = item.Name;
        if (!Enum.TryParse<ItemGroupTagName>(tagName, false, out var itemGroupTagName))
        {
            return null;
        }

        ItemGroupElement element = new ItemGroupElement();
        element.Include = XmlHelper.Attr(item, CsprojInstance.Include);
        element.Version = XmlHelper.Attr(item, CsprojInstance.Version);
        element.Link = XmlHelper.Attr(item, CsprojInstance.Link);
        element.XmlNode = item;
        element.ItemGroupTagName = itemGroupTagName;

        return element;
    }

    /// <summary>
    /// Initializes a new instance of the add to item group class.
    /// </summary>
    /// <param name="itemGroupTagName)">The item group tag name.</param>
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
