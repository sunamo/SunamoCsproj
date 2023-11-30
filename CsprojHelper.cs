
using System.Xml;

namespace SunamoCsproj;

public class CsprojHelper
{
    const string ProjectReference = "ProjectReference";
    const string PackageReference = "PackageReference";
    const string Include = "Include";
    const string Version = "Version";


    public static string ReplaceProjectReferenceForPackageReference(string pathOrContentCsproj, List<string> availableNugetPackages)
    {
        XmlDocument xd = new XmlDocument();
        if (pathOrContentCsproj.StartsWith("<"))
        {
            xd.LoadXml(pathOrContentCsproj);
        }
        else
        {
            xd.Load(pathOrContentCsproj);
        }

        var versionEl = xd.SelectNodes("/Project/ItemGroup/" + ProjectReference);

        foreach (XmlNode item in versionEl)
        {
            var include = XmlHelper.GetAttrValueOrInnerElement(item, Include);

            var fnwoe = Path.GetFileNameWithoutExtension(include);

            // Pokud už jej mám na nugetu
            if (availableNugetPackages.Contains(fnwoe))
            {
                var newEl = xd.CreateElement(PackageReference);
                var attr = CreateAttr(newEl, Include, fnwoe, xd);
                newEl.Attributes.Append(attr);

                var attrVersion = CreateAttr(newEl, Version, "*", xd);
                newEl.Attributes.Append(attrVersion);

                item.ParentNode?.ReplaceChild(newEl, item);
            }

            // Tady break nemůže být když chci nahradit v celém souboru - nahradil by se mi pouze první
            //break;
        }

        return XHDuo.FormatXml(xd.OuterXml);
    }

    private static XmlAttribute CreateAttr(XmlElement newEl, string include, string fnwoe, XmlDocument xd)
    {
        var attr = xd.CreateAttribute(include);
        attr.Value = fnwoe;

        var newAttr = newEl.Attributes.Append(attr);
        return newAttr;
    }
}
