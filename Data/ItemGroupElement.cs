namespace SunamoCsproj.Data;

public class ItemGroupElement
{
    public string Include { get; set; }
    public string Version { get; set; }
    public ItemGroupTagName ItemGroupTagName { get; set; }

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
        ige.ItemGroupTagName = itemGroupTagName;

        return ige;
    }
}
