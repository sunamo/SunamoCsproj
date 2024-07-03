namespace SunamoCsproj._sunamo;
public static class XmlNodeExtensions
{
    public static XmlElement AddElement(this XmlNode element, string name)
    {
        XmlElement xmlElement = element.OwnerDocument!.CreateElement(name);
        element.AppendChild(xmlElement);
        return xmlElement;
    }
}
