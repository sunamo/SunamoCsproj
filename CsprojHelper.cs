namespace SunamoCsproj;


/// <summary>
/// Zde jsou ty co nepoužívají xd
/// </summary>
public class CsprojHelper : CsprojConsts
{
    public static async Task<string?> PropertyGroupItemContent(string path, string tag)
    {
        var c = await File.ReadAllTextAsync(path);
        var xd = new XmlDocument();
        xd.LoadXml(c);

        var s = xd.SelectSingleNode("/Project/PropertyGroup/" + tag);
        if (s == null)
        {
            return null;
        }
        return s.InnerText;
    }

    public static string CsprojPathFromName(string slnFolder, string fnwoe)
    {
        //GetCsprojsGlobal.
        var path = Path.Combine(slnFolder, fnwoe, fnwoe + ".csproj");

        if (!File.Exists(path))
        {
            ThrowEx.Custom(path + " does not exists!");
        }

        return path;
    }

    public static async Task AddLinkToCsproj(string target, string source, string csprojPath)
    {

    }

    /// <summary>
    /// Vrací cestu k csproj, nikoliv složce
    /// </summary>
    /// <param name="path"></param>
    /// <param name="slnFolder"></param>
    /// <returns></returns>
    public static string GetCsprojFromCsPath(string path, string slnFolder = null)
    {
        if (slnFolder != null)
        {
            if (!path.StartsWith(slnFolder))
            {
                ThrowEx.Custom(path + " Not starting with " + slnFolder);
            }

            path = path.Replace(slnFolder, "");
            var fnwoe = SH.RemoveAfterFirst(path, "\\");

            return Path.Combine(slnFolder, fnwoe, fnwoe + ".csproj");
        }
        else
        {
            string pathCopy = new string(path);

            while (true)
            {
                path = Path.GetDirectoryName(path);

                var csprojs = Directory.GetFiles(path, "*.csproj");
                if (csprojs.Any())
                {
                    return csprojs.First();
                }
            }
        }
    }

    public static async Task<DuplicatesInItemGroup> DetectDuplicatedProjectAndPackageReferences(string pathOrContentCsproj)
    {
        if (!pathOrContentCsproj.StartsWith("<"))
        {
            pathOrContentCsproj = await File.ReadAllTextAsync(pathOrContentCsproj);
        }

        var packages = ItemsInItemGroup(ItemGroupTagName.PackageReference, pathOrContentCsproj);
        var projects = ItemsInItemGroup(ItemGroupTagName.ProjectReference, pathOrContentCsproj);

        var packagesNames = packages.Select(d => d.Include).ToList();
        var projectsNames = projects.Select(d => Path.GetFileNameWithoutExtension(d.Include)).ToList();

        var duplicatedPackages = CAG.GetDuplicities<string>(packagesNames);
        var duplicatedProjects = CAG.GetDuplicities<string>(projectsNames);

        var both = packagesNames.Intersect(projectsNames).ToList();

        var r = new DuplicatesInItemGroup { DuplicatedPackages = duplicatedPackages, DuplicatedProjects = duplicatedProjects, ExistsInPackageAndProjectReferences = both };
        var dd = r.HasDuplicates();
        return r;
    }

    /// <summary>
    /// Return always content, even if into A1 is passed path
    /// </summary>
    /// <param name="pathOrContentCsproj"></param>
    /// <returns></returns>
    public static async Task<string> RemoveDuplicatedProjectAndPackageReferences(string pathOrContentCsproj)
    {
        var d = await DetectDuplicatedProjectAndPackageReferences(pathOrContentCsproj);

        if (d.HasDuplicates())
        {
            var result = await RemoveDuplicatedProjectAndPackageReferences(pathOrContentCsproj, d);
            return result;
        }

        if (pathOrContentCsproj.StartsWith("<"))
        {
            return pathOrContentCsproj;
        }
        else
        {
            return await File.ReadAllTextAsync(pathOrContentCsproj);
        }
    }

    public static async Task RemoveDuplicatedProjectAndPackageReferences(List<string> l)
    {
        foreach (var item in l)
        {
            var xmlContent = await RemoveDuplicatedProjectAndPackageReferences(item);
            await File.WriteAllTextAsync(item, xmlContent);
        }
    }

    public static async Task<string> RemoveDuplicatedProjectAndPackageReferences(string pathOrContentCsproj, DuplicatesInItemGroup d)
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

        if (d == null)
        {
            d = await DetectDuplicatedProjectAndPackageReferences(xd.OuterXml);
        }

        var nodes = xd.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference.ToString());

        Dictionary<string, string> csprojNameToRelativePath = new Dictionary<string, string>();

        foreach (XmlNode item in nodes)
        {
            var v = XmlHelper.GetAttrValueOrInnerElement(item, Include);
            var key = Path.GetFileName(v).Replace(".csproj", string.Empty);
#if DEBUG
            if (!csprojNameToRelativePath.ContainsKey(key))
            {
                csprojNameToRelativePath.Add(key, v);
            }
#else
csprojNameToRelativePath.Add(key, v);
#endif
        }

        List<string> alreadyProcessedPackages = new List<string>();
        List<string> alreadyProcessedProjects = new List<string>();

        CsprojInstance csi = new CsprojInstance(xd);


        foreach (var item in d.DuplicatedPackages)
        {
            if (!alreadyProcessedPackages.Contains(item))
            {
                alreadyProcessedPackages.Add(item);
            }
            else
            {
                csi.RemoveSingleItemGroup(item, ItemGroupTagName.PackageReference);
            }
        }

        foreach (var item in d.DuplicatedProjects)
        {
            if (!alreadyProcessedProjects.Contains(item))
            {
                alreadyProcessedProjects.Add(item);
            }
            else
            {
                csi.RemoveSingleItemGroup(csprojNameToRelativePath[item], ItemGroupTagName.ProjectReference);
            }
        }

        return xd.OuterXml;
    }

    /// <summary>
    /// Nepotřebuji tu vracet XmlDocument, je v každém vráceném prvku.OwnerDocument
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="pathOrContentCsproj"></param>
    /// <returns></returns>
    public static List<ItemGroupElement> ItemsInItemGroup(ItemGroupTagName tagName, string pathOrContentCsproj)
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

        var itemsInItemGroup = xd.SelectNodes("/Project/ItemGroup/" + tagName);

        List<ItemGroupElement> result = new List<ItemGroupElement>();

        foreach (XmlNode item in itemsInItemGroup)
        {
            ItemGroupElement p = ItemGroupElement.Parse(item);

            result.Add(p);
        }

        return result;
    }

    public static async Task<string> DetectDuplicatedProjectAndPackageReferences(List<string> csprojs)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in csprojs)
        {
            var dup = await DetectDuplicatedProjectAndPackageReferences(item);
            if (dup.HasDuplicates())
            {
                dup.AppendToSb(sb, item);
            }
        }

        return sb.ToString();
    }

    public static async Task ReplacePackageReferenceForProjectReference(string pathCsproj, string pathSlnFolder)
    {
        //pathSlnFolder = pathSlnFolder.TrimEnd('\\') + "\\";

        CsprojInstance csp = new CsprojInstance(pathCsproj);

        var packagesRef = ItemsInItemGroup(ItemGroupTagName.PackageReference, pathCsproj);

        foreach (var item in packagesRef)
        {
            //var fnwoe = Path.GetFileNameWithoutExtension(item.Include);
            csp.RemoveSingleItemGroup(item.Include, ItemGroupTagName.PackageReference);
            csp.CreateNewItemGroupElement(ItemGroupTagName.ProjectReference, "..\\" + item.Include + "\\" + item.Include + ".csproj", null);


        }

        csp.Save();
    }

    public static async Task<string> ReplaceProjectReferenceForPackageReference(string pathOrContentCsproj, List<string> availableNugetPackages, bool isTests)
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

        if (!pathOrContentCsproj.StartsWith("<") && (pathOrContentCsproj.EndsWith("Tests.csproj") || pathOrContentCsproj.Contains("TestValues")))
        {
            return await File.ReadAllTextAsync(pathOrContentCsproj);
        }
        XmlDocument xd = new XmlDocument();
        if (pathOrContentCsproj.StartsWith("<"))
        {
            if (isTests)
            {
                return pathOrContentCsproj;
            }
            xd.LoadXml(pathOrContentCsproj);
        }
        else
        {
            xd.Load(pathOrContentCsproj);
        }

        var versionEl = xd.SelectNodes("/Project/ItemGroup/" + ItemGroupTagName.ProjectReference.ToString());

        var csi = new CsprojInstance(xd);

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

        return XHelper.FormatXmlInMemory(xd.OuterXml);
    }

    /// <summary>
    /// Use RHSE2.SetPropertyToInnerClass
    /// </summary>
    /// <param name="contentOrPath"></param>
    /// <returns></returns>
    public static async Task ParseCsproj(string contentOrPath)
    {
        if (!contentOrPath.StartsWith("<"))
        {
            contentOrPath = await File.ReadAllTextAsync(contentOrPath);
        }

        CsprojData d = new CsprojData();

        XDocument x = XDocument.Parse(contentOrPath);
        foreach (var item in x.Root.Descendants())
        {
            if (item.Name == "PropertyGroup")
            {
                //RH.SetPropertyToInnerClass(d.PropertyGroup, item.Name, item.Value);
            }
        }
    }

    public static readonly List<string> classCodeElements = new List<string>() { "class ", "interface ", "enum ", "struct ", "delegate " };

    /// <summary>
    /// v I1 je project name
    /// v I2 je sanitizované celé NS
    /// </summary>
    /// <param name="content"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static (string, string) ParseNamespaceFromCsFile(string content, string path)
    {
        /*
    Nemůže obsahovat ;
    SunamoDateTime chybí
    */
        // zjistit zda tu mám SunamoPercentCalculator a pokud ne, proč?
        var l = SHGetLines.GetLines(content);

#if DEBUG
        if (path == @"E:\vs\Projects\sunamoWithoutLocalDep\SunamoPercentCalculator\PercentCalculator.cs")
        {

        }
#endif

        for (int i = 0; i < l.Count; i++)
        {
            var item = l[i];
            if (classCodeElements.Any(d => item.Contains(d)))
            {
                break;
            }

            if (item.StartsWith("#else"))
            {
                var line = l[i + 1].Trim();
                var firstPart = line.Split('.')[0];
                return (firstPart, line);
            }

            if (item.StartsWith("namespace "))
            {
                var d = item.Trim().TrimEnd(';').TrimEnd('{').Trim();
                d = d.Replace("namespace ", "");

                var firstPart = d.Split('.')[0];

#if DEBUG
                //if (d.Contains(";"))
                //{
                //    ThrowEx.Custom("NS can't contains ;");
                //}
                //if (d == "SunamoDateTime")
                //{
                //}

                if (d == "SunamoData")
                {

                }
                if (d == "SunamoData.Data")
                {

                }
                if (firstPart == "SunamoText" || d == "SunamoText")
                {

                }
#endif

                return (firstPart, CsprojNsHelper.SanitizeProjectName(d));
            }
        }

        return (null, null);
    }

    /// <summary>
    /// Protože mám často null v hodnotách kde mi čisté where selže, je tu tato metdoa
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="attr"></param>
    /// <param name="mustContains"></param>
    /// <param name="pathCsproj"></param>
    /// <returns></returns>
    public static List<ItemGroupElement> GetAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr, string mustContains, string pathCsproj)
    {
        var items = ItemsInItemGroup(tagName, pathCsproj);
        items = FilterByAttrAndContains(items, attr, mustContains);
        return items;
    }

    public static List<ItemGroupElement> FilterByAttrAndContains(List<ItemGroupElement> l, string attr, string mustContains)
    {
        return l.Where(d => (attr == "Link" ? d.Link : (attr == "Include" ? d.Include : throw new Exception($"{nameof(attr)} is {attr}, must be Link or Include"))).ContainsNullAllow(mustContains)).ToList();
    }

    public static void RemoveAllItemsInItemGroupWhichContainsInInclude(ItemGroupTagName tagName, string attr, string mustContains, string pathCsproj)
    {
        var items = ItemsInItemGroup(tagName, pathCsproj);
        items = FilterByAttrAndContains(items, attr, mustContains);

        if (items.Any())
        {
            foreach (var item in items)
            {
                item.XmlNode.ParentNode.RemoveChild(item.XmlNode);
            }

            items[0].XmlNode.OwnerDocument.Save(pathCsproj);
        }
    }
}
