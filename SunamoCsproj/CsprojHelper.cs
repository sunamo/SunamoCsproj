// variables names: ok
namespace SunamoCsproj;

using System.Xml.Linq;

/// <summary>
/// Helper methods for working with csproj files.
/// </summary>
public class CsprojHelper : CsprojConsts
{
    /// <summary>
    /// Keywords that indicate class-level code elements.
    /// </summary>
    public static readonly List<string> ClassCodeElements = ["class ", "interface ", "enum ", "struct ", "delegate "];

    /// <summary>
    /// Formats XML for better readability.
    /// </summary>
    /// <param name="xml">Unformatted XML.</param>
    /// <returns>Formatted XML.</returns>
    private static string FormatXml(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            return doc.ToString();
        }
        catch (Exception)
        {
            return xml;
        }
    }
    /// <summary>
    /// EN: Must be here, works with multiple csproj files at once.
    /// CZ: Musí být zde, pracuje s více csproj najednou.
    /// </summary>
    /// <param name="csprojs">EN: List of csproj file paths. CZ: Seznam cest k csproj souborům.</param>
    /// <returns>EN: String with duplicates report. CZ: Řetězec s reportem duplicit.</returns>
    public static async Task<string> DetectDuplicatedProjectAndPackageReferences(List<string> csprojs)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in csprojs)
        {
            var csi = new CsprojInstance(item);
            var dup = await csi.DetectDuplicatedProjectAndPackageReferences();
            if (dup.HasDuplicates()) dup.AppendToSb(stringBuilder, item);
        }
        return stringBuilder.ToString();
    }
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static string CsprojPathFromName(string slnFolder, string fnwoe)
    {
        //GetCsprojsGlobal.
        var path = Path.Combine(slnFolder, fnwoe, fnwoe + ".csproj");
        if (!File.Exists(path)) ThrowEx.Custom(path + " does not exists!");
        return path;
    }
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task AddLinkToCsproj(string target, string source, string csprojPath)
    {
    }
    /// <summary>
    /// EN: Returns path to csproj file, not folder.
    /// CZ: Vrací cestu k csproj souboru, nikoliv složce.
    /// </summary>
    /// <param name="path">EN: Path to .cs file. CZ: Cesta k .cs souboru.</param>
    /// <param name="slnFolder">EN: Solution folder or null. CZ: Složka solution nebo null.</param>
    /// <returns>EN: Path to csproj file. CZ: Cesta k csproj souboru.</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static string GetCsprojFromCsPath(string path, string? slnFolder = null)
    {
        if (slnFolder != null)
        {
            if (!path.StartsWith(slnFolder)) ThrowEx.Custom(path + " Not starting with " + slnFolder);
            path = path.Replace(slnFolder, "");
            var fnwoe = SH.RemoveAfterFirst(path, "\\");
            return Path.Combine(slnFolder, fnwoe, fnwoe + ".csproj");
        }
        var pathCopy = new string(path);
        while (true)
        {
            path = Path.GetDirectoryName(path);
            var csprojs = Directory.GetFiles(path, "*.csproj");
            if (csprojs.Any()) return csprojs.First();
        }
    }
    public static async Task RemoveDuplicatedProjectAndPackageReferences(List<string> list)
    {
        foreach (var item in list)
        {
            var csi = new CsprojInstance(item);
            var xmlContent = await csi.RemoveDuplicatedProjectAndPackageReferences();
            await File.WriteAllTextAsync(item, xmlContent);
        }
    }
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task<string> ReplaceProjectReferenceForPackageReference(string pathOrContentCsproj,
        List<string> availableNugetPackages, bool isTests)
    {
        // EN: Tests should connect only assembly they test, or project with test data. However, this won't work - then million errors like: "Unable to satisfy conflicting requests for 'Diacritics'" (via project/package mix). If I connect nuget instead of project, error disappears. Trying if it would work with tests in separate sln - so far looks yes. In tests don't connect any nugets, especially not SunamoShared (has many deps, would cause mess). This way tests won't work in pipeline but will be solved later.
        // CZ: Testy mají připojovat jen assembly kterou testují, případně projekt s testovacími daty. Nicméně takto to nepůjde - pak milion chyb jako: "Unable to satisfy conflicting requests for 'Diacritics'" (mix via project/package). Pokud připojím nuget místo projektu, chyba zmizí. Zkouším zda by to fungovalo s testy ve samostatné sln - zatím vypadá že ano. V testech nepřipojovat žádné nugety, zejména ne SunamoShared (má spoustu deps, dělalo by to neplechu). Tímhle testy nebudou fungovat v pipeline ale to se dořeší později.
        if (!pathOrContentCsproj.StartsWith("<") && (pathOrContentCsproj.EndsWith("Tests.csproj") ||
                                                     pathOrContentCsproj.Contains("TestValues")))
            return await File.ReadAllTextAsync(pathOrContentCsproj);
        var xmlDocument = new XmlDocument();
        if (pathOrContentCsproj.StartsWith("<"))
        {
            if (isTests) return pathOrContentCsproj;
            xmlDocument.LoadXml(pathOrContentCsproj);
        }
        else
        {
            xmlDocument.Load(pathOrContentCsproj);
        }
        var versionEl = xmlDocument.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference);
        var csi = new CsprojInstance(xmlDocument);
        foreach (XmlNode item in versionEl)
        {
            var include = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var fnwoe = Path.GetFileNameWithoutExtension(include);
            // EN: If I already have it as nuget
            // CZ: Pokud už jej mám na nugetu
            if (availableNugetPackages.Contains(fnwoe))
            {
                var newEl = csi.CreateNewPackageReference(fnwoe, "*");
                item.ParentNode?.ReplaceChild(newEl, item);
            }
            // EN: Can't break here when I want to replace in whole file - would replace only first one
            // CZ: Tady break nemůže být když chci nahradit v celém souboru - nahradil by se mi pouze první
            //break;
        }
        // EN: TODO: make format from SunamoXml
        // CZ: TODO: z SunamoXml udělat formát
        return XHelper.FormatXmlInMemory(xmlDocument.OuterXml);
    }

    /// <summary>
    /// EN: Replaces ProjectReference with PackageReference and returns new csproj content + names of replaced projects.
    /// CZ: Nahradí ProjectReference za PackageReference a vrátí nový obsah csproj + názvy projektů, které byly nahrazeny.
    /// </summary>
    /// <param name="contentCsproj">EN: Content of csproj file. CZ: Obsah csproj souboru.</param>
    /// <param name="availableNugetPackagesS">EN: List of available NuGet packages. CZ: Seznam dostupných NuGet balíčků.</param>
    /// <param name="isTests">EN: Whether this is a test project. CZ: Zda je to testovací projekt.</param>
    /// <returns>EN: Tuple of (new csproj content, list of removed project names). CZ: Tuple (nový obsah csproj, seznam odstraněných názvů projektů).</returns>
#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task<(string, List<string>)> ReplaceProjectReferenceForPackageReferenceWithRemoved(string contentCsproj, List<string> availableNugetPackagesS, bool isTests)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var removedProjects = new List<string>();
        // EN: Original replacement logic
        // CZ: Původní logika nahrazování
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(contentCsproj);
        var projectReferences = xmlDoc.GetElementsByTagName("ProjectReference");
        var toRemove = new List<XmlNode>();
        foreach (XmlNode pr in projectReferences)
        {
            var include = pr.Attributes?["Include"]?.Value;
            if (include != null)
            {
                var projectName = Path.GetFileNameWithoutExtension(include);
                if (availableNugetPackagesS.Contains(projectName))
                {
                    removedProjects.Add(projectName);
                    toRemove.Add(pr);
                }
            }
        }
        // EN: Remove ProjectReference and optionally add PackageReference
        // CZ: Odstraň ProjectReference a případně přidej PackageReference
        foreach (var pr in toRemove)
        {
            var projectName = Path.GetFileNameWithoutExtension(pr.Attributes["Include"].Value);
            pr.ParentNode.RemoveChild(pr);
            // EN: Add PackageReference
            // CZ: Přidej PackageReference
            var itemGroup = xmlDoc.CreateElement("ItemGroup");
            var pkgRef = xmlDoc.CreateElement("PackageReference");
            var attr = xmlDoc.CreateAttribute("Include");
            attr.Value = projectName;
            pkgRef.Attributes.Append(attr);
            itemGroup.AppendChild(pkgRef);
            xmlDoc.DocumentElement.AppendChild(itemGroup);
        }

        // EN: Return formatted XML instead of unformatted
        // CZ: Vrať formátovaný XML místo neformátovaného
        return (FormatXml(xmlDoc.OuterXml), removedProjects);
    }

    /// <summary>
    ///     Use RHSE2.SetPropertyToInnerClass
    /// </summary>
    /// <param name="contentOrPath"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task ParseCsproj(string contentOrPath)
    {
        if (!contentOrPath.StartsWith("<")) contentOrPath = await File.ReadAllTextAsync(contentOrPath);
        var data = new CsprojData();
        var xValue = XDocument.Parse(contentOrPath);
        foreach (var item in xValue.Root.Descendants())
            if (item.Name == "PropertyGroup")
            {
                //RH.SetPropertyToInnerClass(data.PropertyGroup, item.Name, item.Value);
            }
    }
    /// <summary>
    /// EN: Parses namespace from .cs file. Item1 is project name, Item2 is sanitized full namespace.
    /// CZ: Parsuje jmenný prostor z .cs souboru. Item1 je název projektu, Item2 je sanitizované celé NS.
    /// </summary>
    /// <param name="content">EN: File content. CZ: Obsah souboru.</param>
    /// <param name="path">EN: File path or null. CZ: Cesta k souboru nebo null.</param>
    /// <returns>EN: Tuple (project name, full sanitized namespace). CZ: Tuple (název projektu, celé sanitizované NS).</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static (string, string) ParseNamespaceFromCsFile(string content, string? path)
    {
        // EN: Cannot contain ; SunamoDateTime is missing
        // CZ: Nemůže obsahovat ; SunamoDateTime chybí

        // EN: Find out if I have SunamoPercentCalculator here and if not, why?
        // CZ: Zjistit zda tu mám SunamoPercentCalculator a pokud ne, proč?
        var list = SHGetLines.GetLines(content);
        for (var index = 0; index < list.Count; index++)
        {
            var item = list[index];
            if (ClassCodeElements.Any(data => item.Contains(data))) break;
            if (item.StartsWith("#else"))
            {
                var line = list[index + 1].Trim();
                var firstPart = line.Split('.')[0];
                return (firstPart, line);
            }
            if (item.StartsWith("namespace "))
            {
                var data = item.Trim().TrimEnd(';').TrimEnd('{').Trim();
                data = data.Replace("namespace ", "");
                var firstPart = data.Split('.')[0];
#if DEBUG
                //if (data.Contains(";"))
                //{
                //    ThrowEx.Custom("NS can't contains ;");
                //}
                //if (data == "SunamoDateTime")
                //{
                //}
                if (data == "SunamoData")
                {
                }
                if (data == "SunamoData.Data")
                {
                }
                if (firstPart == "SunamoText" || data == "SunamoText")
                {
                }
#endif
                return (firstPart, CsprojNsHelper.SanitizeProjectName(data));
            }
        }
        return (null, null);
    }
}
#pragma warning restore