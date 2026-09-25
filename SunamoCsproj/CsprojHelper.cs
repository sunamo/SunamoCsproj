namespace SunamoCsproj;

using System.Xml.Linq;

public class CsprojHelper : CsprojConsts
{
    public static readonly List<string> ClassCodeElements = ["class ", "interface ", "enum ", "struct ", "delegate "];

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

    // EN: Must be here, works with multiple csproj files at once.
    // CZ: Musí být zde, pracuje s více csproj najednou.
    public static async Task<string> DetectDuplicatedProjectAndPackageReferences(List<string> csprojs)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in csprojs)
        {
            var csprojInstance = new CsprojInstance(item);
            var duplicates = await csprojInstance.DetectDuplicatedProjectAndPackageReferences();
            if (duplicates.HasDuplicates()) duplicates.AppendToSb(stringBuilder, item);
        }
        return stringBuilder.ToString();
    }
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static string CsprojPathFromName(string slnFolder, string fileNameWithoutExtension)
    {
        //GetCsprojsGlobal.
        var path = Path.Combine(slnFolder, fileNameWithoutExtension, fileNameWithoutExtension + ".csproj");
        if (!File.Exists(path)) ThrowEx.Custom(path + " does not exists!");
        return path;
    }
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task AddLinkToCsproj(string target, string source, string csprojPath)
    {
    }

    // EN: Returns path to csproj file, not folder.
    // CZ: Vrací cestu k csproj souboru, nikoliv složce.
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static string GetCsprojFromCsPath(string path, string? slnFolder = null)
    {
        if (slnFolder != null)
        {
            if (!path.StartsWith(slnFolder)) ThrowEx.Custom(path + " Not starting with " + slnFolder);
            path = path.Replace(slnFolder, "");
            var projectName = SH.RemoveAfterFirst(path, "\\");
            return Path.Combine(slnFolder, projectName, projectName + ".csproj");
        }
        var pathCopy = path;
        while (true)
        {
            path = Path.GetDirectoryName(path)!;
            var csprojs = Directory.GetFiles(path, "*.csproj");
            if (csprojs.Any()) return csprojs.First();
        }
    }
    public static async Task RemoveDuplicatedProjectAndPackageReferences(List<string> list)
    {
        foreach (var item in list)
        {
            var csprojInstance = new CsprojInstance(item);
#pragma warning disable CS0618 // EN: Type or member is obsolete - internal usage allowed / CZ: Typ nebo člen je zastaralý - interní použití povoleno
            var xmlContent = await csprojInstance.RemoveDuplicatedProjectAndPackageReferences();
#pragma warning restore CS0618
            await FileAsync.WriteAllTextAsync(item, xmlContent);
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
            return await FileAsync.ReadAllTextAsync(pathOrContentCsproj);
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
        var projectReferenceNodes = xmlDocument.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference);
        var csprojInstance = new CsprojInstance(xmlDocument);
        foreach (XmlNode item in projectReferenceNodes!)
        {
            var include = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var projectName = Path.GetFileNameWithoutExtension(include)!;
            // EN: If I already have it as nuget
            // CZ: Pokud už jej mám na nugetu
            if (availableNugetPackages.Contains(projectName))
            {
                var newPackageReference = csprojInstance.CreateNewPackageReference(projectName, "*");
                item.ParentNode?.ReplaceChild(newPackageReference, item);
            }
            // EN: Can't break here when I want to replace in whole file - would replace only first one
            // CZ: Tady break nemůže být když chci nahradit v celém souboru - nahradil by se mi pouze první
            //break;
        }
        // EN: TODO: make format from SunamoXml
        // CZ: TODO: z SunamoXml udělat formát
        return XHelper.FormatXmlInMemory(xmlDocument.OuterXml);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task<(string, List<string>)> ReplaceProjectReferenceForPackageReferenceWithRemoved(string contentCsproj, List<string> availableNugetPackagesS, bool isTests)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var removedProjects = new List<string>();
        // EN: Original replacement logic
        // CZ: Původní logika nahrazování
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(contentCsproj);
        var projectReferences = xmlDocument.GetElementsByTagName("ProjectReference");
        var toRemove = new List<XmlNode>();
        foreach (XmlNode projectReference in projectReferences)
        {
            var include = projectReference.Attributes?["Include"]?.Value;
            if (include != null)
            {
                var projectName = Path.GetFileNameWithoutExtension(include);
                if (availableNugetPackagesS.Contains(projectName))
                {
                    removedProjects.Add(projectName);
                    toRemove.Add(projectReference);
                }
            }
        }
        // EN: Remove ProjectReference and optionally add PackageReference
        // CZ: Odstraň ProjectReference a případně přidej PackageReference
        foreach (var projectReference in toRemove)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectReference.Attributes!["Include"]!.Value);
            projectReference.ParentNode!.RemoveChild(projectReference);
            // EN: Add PackageReference
            // CZ: Přidej PackageReference
            var itemGroup = xmlDocument.CreateElement("ItemGroup");
            var packageReference = xmlDocument.CreateElement("PackageReference");
            var includeAttribute = xmlDocument.CreateAttribute("Include");
            includeAttribute.Value = projectName;
            packageReference.Attributes.Append(includeAttribute);
            itemGroup.AppendChild(packageReference);
            xmlDocument.DocumentElement!.AppendChild(itemGroup);
        }

        // EN: Return formatted XML instead of unformatted
        // CZ: Vrať formátovaný XML místo neformátovaného
        return (FormatXml(xmlDocument.OuterXml), removedProjects);
    }

    // Use RHSE2.SetPropertyToInnerClass
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task ParseCsproj(string contentOrPath)
    {
        if (!contentOrPath.StartsWith("<")) contentOrPath = await FileAsync.ReadAllTextAsync(contentOrPath);
        var data = new CsprojData();
        var xDocument = XDocument.Parse(contentOrPath);
        foreach (var item in xDocument.Root!.Descendants())
            if (item.Name == "PropertyGroup")
            {
                //RH.SetPropertyToInnerClass(data.PropertyGroup, item.Name, item.Value);
            }
    }

    // EN: Parses namespace from .cs file. Item1 is project name, Item2 is sanitized full namespace.
    // CZ: Parsuje jmenný prostor z .cs souboru. Item1 je název projektu, Item2 je sanitizované celé NS.
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
            if (ClassCodeElements.Any(codeElement => item.Contains(codeElement))) break;
            if (item.StartsWith("#else"))
            {
                var line = list[index + 1].Trim();
                var firstPart = line.Split('.')[0];
                return (firstPart, line);
            }
            if (item.StartsWith("namespace "))
            {
                var namespaceLine = item.Trim().TrimEnd(';').TrimEnd('{').Trim();
                namespaceLine = namespaceLine.Replace("namespace ", "");
                var firstPart = namespaceLine.Split('.')[0];
                return (firstPart, CsprojNsHelper.SanitizeProjectName(namespaceLine));
            }
        }
        return (null!, null!);
    }
}
#pragma warning restore
