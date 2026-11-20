namespace SunamoCsproj._sunamo;

// Variable names have been checked and replaced with self-descriptive names
internal static class XmlNodeExtensions
{
    internal static XmlElement AddElement(this XmlNode element, string name)
    {
        XmlElement xmlElement = element.OwnerDocument!.CreateElement(name);
        element.AppendChild(xmlElement);
        return xmlElement;
    }
}