namespace SunamoCsproj;

using System.Xml.Linq;

public class CsprojHelper : CsprojConsts
{
    public static readonly List<string> classCodeElements = ["class ", "interface ", "enum ", "struct ", "delegate "];

    /// <summary>
    /// EN: Formats XML for better readability.
    /// CZ: Formátuje XML pro lepší čitelnost.
    /// </summary>
    /// <param name="xml">EN: Unformatted XML. CZ: Neformátovaný XML.</param>
    /// <returns>EN: Formatted XML. CZ: Formátovaný XML.</returns>
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
    ///     Musí být zde, pracuje s více csproj najednou
    /// </summary>
    /// <param name="csprojs"></param>
    /// <returns></returns>
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
    ///     Vrací cestu k csproj, nikoliv složce
    /// </summary>
    /// <param name="path"></param>
    /// <param name="slnFolder"></param>
    /// <returns></returns>
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
        /*
        Takhle to má být správné
        Testy mají připojovat jen assembly kterou testují, případně projekt se testovacími daty
        nicméně takto to nepůjde, pak mám milion chyb jako:
        Unable to satisfy conflicting requests for 'Diacritics':
        Diacritics (>= 3.3.18) (via project/SunamoShared 23.12.15.1),
        Diacritics (>= 3.3.18) (via package/SunamoShared 23.12.15.1),
        Diacritics (>= 3.3.18) (via package/SunamoShared 23.12.15.1),
        Diacritics (>= 3.3.18) (via package/SunamoShared 23.12.15.1),
        Diacritics (>= 3.3.18) (via package/SunamoShared 23.12.15.1) Framework: (.NETCoreApp,Version=v8.0)
        pokud připojím nuget místo projektu chyba hned zmizí
        zkouším zda by to fungovalo kdybych měl testy ve samostatné sln, zatím na swod vypadá že ano
        ve testech nepřipojovat žádné nugety a už vůbec ne SunamoShared, má spoustu deps, dělalo by to neplechu
        tímhle ty testy nebudou fungovat v pipeline ale to se dořeší později
        akorát jsem smazal původní sunamo.Tests.sln a teď ho musím složitě zase dovytvářet :-(
        */
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
            // Pokud už jej mám na nugetu
            if (availableNugetPackages.Contains(fnwoe))
            {
                var newEl = csi.CreateNewPackageReference(fnwoe, "*");
                item.ParentNode?.ReplaceChild(newEl, item);
            }
            // Tady break nemůže být když chci nahradit v celém souboru - nahradil by se mi pouze první
            //break;
        }
        // TODO: z SunamoXml udělat formát
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
    ///     v I1 je project name
    ///     v I2 je sanitizované celé NS
    /// </summary>
    /// <param name="content"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static (string, string) ParseNamespaceFromCsFile(string content, string? path)
    {
        /*
    Nemůže obsahovat ;
    SunamoDateTime chybí
    */
        // zjistit zda tu mám SunamoPercentCalculator a pokud ne, proč?
        var list = SHGetLines.GetLines(content);
        for (var index = 0; index < list.Count; index++)
        {
            var item = list[index];
            if (classCodeElements.Any(data => item.Contains(data))) break;
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