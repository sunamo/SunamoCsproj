// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoCsproj._sunamo;
internal static class XmlNodeExtensions
{
    internal static XmlElement AddElement(this XmlNode element, string name)
    {
        XmlElement xmlElement = element.OwnerDocument!.CreateElement(name);
        element.AppendChild(xmlElement);
        return xmlElement;
    }
}
