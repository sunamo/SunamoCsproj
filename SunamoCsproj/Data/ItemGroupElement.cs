namespace SunamoCsproj.Data;

/// <summary>
/// Represents a single ItemGroup element from csproj file (PackageReference, ProjectReference, Compile, etc.).
/// </summary>
public class ItemGroupElement
{
    /// <summary>
    /// Gets or sets the Include attribute value (e.g., package name or file path).
    /// </summary>
    public string? Include { get; set; }

    /// <summary>
    /// Gets or sets the Version attribute value (for PackageReference).
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Gets or sets the Link attribute value (for linked files).
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// Gets or sets the underlying XmlNode.
    /// </summary>
    public XmlNode? XmlNode { get; set; }

    /// <summary>
    /// Gets or sets the type of ItemGroup tag (PackageReference, ProjectReference, etc.).
    /// </summary>
    public ItemGroupTagName ItemGroupTagName { get; set; }

    /// <summary>
    /// Converts this element to XML string representation.
    /// </summary>
    /// <returns>XML string representation.</returns>
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
    /// Parses XmlNode to ItemGroupElement.
    /// </summary>
    /// <param name="item">The XmlNode to parse.</param>
    /// <returns>Parsed ItemGroupElement or null if tag name is not recognized.</returns>
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
    /// Adds this element to specified ItemGroup XML element.
    /// </summary>
    /// <param name="itemGroup">The ItemGroup element to add to.</param>
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