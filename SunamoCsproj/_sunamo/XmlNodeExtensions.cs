// variables names: ok
namespace SunamoCsproj._sunamo;

// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
internal static class XmlNodeExtensions
{
    internal static XmlElement AddElement(this XmlNode element, string name)
    {
        XmlElement xmlElement = element.OwnerDocument!.CreateElement(name);
        element.AppendChild(xmlElement);
        return xmlElement;
    }
}