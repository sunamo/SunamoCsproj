namespace SunamoCsproj;

/// <summary>
/// Zde jsou ty co nepoužívají xd
/// </summary>
public class CsprojHelper : CsprojConsts
{
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

        //var duplicatedPackages = packagesNames.GroupBy(d => d.Key).Where(d => d.Count() > 1);
        //var duplicatedProjects = packagesNames.GroupBy(d => d).Where(d => d.Count() > 1);

        var duplicatedPackages = SunamoCsproj._sunamo.CA.GetDuplicities<string>(packagesNames);
        var duplicatedProjects = SunamoCsproj._sunamo.CA.GetDuplicities<string>(projectsNames);

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
            await TFSE.WriteAllText(item, xmlContent);
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

        CsprojInstance csi = new CsprojInstance() { xd = xd };


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
            return await TFSE.ReadAllText(pathOrContentCsproj);
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

        var csi = new CsprojInstance() { xd = xd };


        foreach (XmlNode item in versionEl)
        {
            var include = XmlHelper.GetAttrValueOrInnerElement(item, Include);

            var fnwoe = Path.GetFileNameWithoutExtension(include);

            // Pokud už jej mám na nugetu
            if (availableNugetPackages.Contains(fnwoe))
            {
                var newEl = csi.CreateNewPackageReference(Include, "*");

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
                RHSE2.SetPropertyToInnerClass(d.PropertyGroup, item.Name, item.Value);
            }
        }
    }
}
