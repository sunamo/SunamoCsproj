namespace SunamoCsproj;


/// <summary>
/// Zde jsou ty co nepoužívají xd
/// </summary>
[Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
public class CsprojHelper : CsprojConsts
{
    /// <summary>
    /// Musí být zde, pracuje s více csproj najednou
    /// </summary>
    /// <param name="csprojs"></param>
    /// <returns></returns>
    public static async Task<string> DetectDuplicatedProjectAndPackageReferences(List<string> csprojs)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in csprojs)
        {
            var csi = new CsprojInstance(item);
            var dup = await csi.DetectDuplicatedProjectAndPackageReferences();
            if (dup.HasDuplicates())
            {
                dup.AppendToSb(sb, item);
            }
        }

        return sb.ToString();
    }


    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
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

    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static async Task AddLinkToCsproj(string target, string source, string csprojPath)
    {

    }

    /// <summary>
    /// Vrací cestu k csproj, nikoliv složce
    /// </summary>
    /// <param name="path"></param>
    /// <param name="slnFolder"></param>
    /// <returns></returns>
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
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




    public static async Task RemoveDuplicatedProjectAndPackageReferences(List<string> l)
    {
        foreach (var item in l)
        {
            CsprojInstance csi = new CsprojInstance(item);
            var xmlContent = await csi.RemoveDuplicatedProjectAndPackageReferences();
            await File.WriteAllTextAsync(item, xmlContent);
        }
    }











    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
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
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
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
    [Obsolete("everything from here will be converted to CsprojInstance. Don't add a single method here!")]
    public static (string, string) ParseNamespaceFromCsFile(string content, string path)
    {
        /*
    Nemůže obsahovat ;
    SunamoDateTime chybí
    */
        // zjistit zda tu mám SunamoPercentCalculator a pokud ne, proč?
        var l = SHGetLines.GetLines(content);

#if DEBUG
        if (path == @"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoPercentCalculator\PercentCalculator.cs")
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




}
