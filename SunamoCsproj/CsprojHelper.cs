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
    /// Must be here, works with multiple csproj files at once.
    /// </summary>
    /// <param name="csprojs">List of csproj file paths.</param>
    /// <returns>String with duplicates report.</returns>
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
    /// <summary>
    /// Csproj path from name.
    /// </summary>
    /// <param name="slnFolder">The sln folder.</param>
    /// <param name="fileNameWithoutExtension">The file name without extension.</param>
    /// <returns>The csproj path from name result.</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static string CsprojPathFromName(string slnFolder, string fileNameWithoutExtension)
    {
        //GetCsprojsGlobal.
        var path = Path.Combine(slnFolder, fileNameWithoutExtension, fileNameWithoutExtension + ".csproj");
        if (!File.Exists(path)) ThrowEx.Custom(path + " does not exists!");
        return path;
    }
    /// <summary>
    /// Add link to csproj.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="source">The source.</param>
    /// <param name="csprojPath">The csproj path.</param>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task AddLinkToCsproj(string target, string source, string csprojPath)
    {
    }
    /// <summary>
    /// Returns path to csproj file, not folder.
    /// </summary>
    /// <param name="path">Path to .cs file.</param>
    /// <param name="slnFolder">Solution folder or null.</param>
    /// <returns>Path to csproj file.</returns>
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
    /// <summary>
    /// Remove duplicated project and package references.
    /// </summary>
    /// <param name="list">The list.</param>
    public static async Task RemoveDuplicatedProjectAndPackageReferences(List<string> list)
    {
        foreach (var item in list)
        {
            var csprojInstance = new CsprojInstance(item);
#pragma warning disable CS0618 // Type or member is obsolete - internal usage allowed /
            var xmlContent = await csprojInstance.RemoveDuplicatedProjectAndPackageReferences();
#pragma warning restore CS0618
            await FileAsync.WriteAllTextAsync(item, xmlContent);
        }
    }
    /// <summary>
    /// Replace project reference for package reference.
    /// </summary>
    /// <param name="pathOrContentCsproj">The path or content csproj.</param>
    /// <param name="availableNugetPackages">The available nuget packages.</param>
    /// <param name="isTests">The is tests.</param>
    /// <returns>The replace project reference for package reference result.</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task<string> ReplaceProjectReferenceForPackageReference(string pathOrContentCsproj,
        List<string> availableNugetPackages, bool isTests)
    {
        // Tests should connect only assembly they test, or project with test data. However, this won't work - then million errors like: "Unable to satisfy conflicting requests for 'Diacritics'" (via project/package mix). If I connect nuget instead of project, error disappears. Trying if it would work with tests in separate sln - so far looks yes. In tests don't connect any nugets, especially not SunamoShared (has many deps, would cause mess). This way tests won't work in pipeline but will be solved later.
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
            // If I already have it as nuget
            if (availableNugetPackages.Contains(projectName))
            {
                var newPackageReference = csprojInstance.CreateNewPackageReference(projectName, "*");
                item.ParentNode?.ReplaceChild(newPackageReference, item);
            }
            // Can't break here when I want to replace in whole file - would replace only first one
        }
        // TODO: make format from SunamoXml
        return XHelper.FormatXmlInMemory(xmlDocument.OuterXml);
    }

    /// <summary>
    /// Replaces ProjectReference with PackageReference and returns new csproj content + names of replaced projects.
    /// </summary>
    /// <param name="contentCsproj">Content of csproj file.</param>
    /// <param name="availableNugetPackagesS">List of available NuGet packages.</param>
    /// <param name="isTests">Whether this is a test project.</param>
    /// <returns>Tuple of (new csproj content, list of removed project names).</returns>
#pragma warning disable IDE0060 // Remove unused parameter
    public static async Task<(string, List<string>)> ReplaceProjectReferenceForPackageReferenceWithRemoved(string contentCsproj, List<string> availableNugetPackagesS, bool isTests)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var removedProjects = new List<string>();
        // Original replacement logic
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
        // Remove ProjectReference and optionally add PackageReference
        foreach (var projectReference in toRemove)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectReference.Attributes!["Include"]!.Value);
            projectReference.ParentNode!.RemoveChild(projectReference);
            // Add PackageReference
            var itemGroup = xmlDocument.CreateElement("ItemGroup");
            var packageReference = xmlDocument.CreateElement("PackageReference");
            var includeAttribute = xmlDocument.CreateAttribute("Include");
            includeAttribute.Value = projectName;
            packageReference.Attributes.Append(includeAttribute);
            itemGroup.AppendChild(packageReference);
            xmlDocument.DocumentElement!.AppendChild(itemGroup);
        }

        // Return formatted XML instead of unformatted
        return (FormatXml(xmlDocument.OuterXml), removedProjects);
    }

    /// <summary>
    ///     Use RHSE2.SetPropertyToInnerClass
    /// </summary>
    /// <param name="contentOrPath"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task ParseCsproj(string contentOrPath)
    {
        if (!contentOrPath.StartsWith("<")) contentOrPath = await FileAsync.ReadAllTextAsync(contentOrPath);
        var data = new CsprojData();
        var xDocument = XDocument.Parse(contentOrPath);
        foreach (var item in xDocument.Root!.Descendants())
            if (item.Name == "PropertyGroup")
            {
            }
    }
    /// <summary>
    /// Parses namespace from .cs file. Item1 is project name, Item2 is sanitized full namespace.
    /// </summary>
    /// <param name="content">File content.</param>
    /// <param name="path">File path or null.</param>
    /// <returns>Tuple (project name, full sanitized namespace).</returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static (string, string) ParseNamespaceFromCsFile(string content, string? path)
    {
        // Cannot contain ; SunamoDateTime is missing

        // Find out if I have SunamoPercentCalculator here and if not, why?
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