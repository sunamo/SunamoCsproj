namespace SunamoCsproj;

public class CsprojNsHelper
{
    // musí být namespace bez mezery na konci, takto se užívá v #if
    public static string[] keywordsBeforeFirstCodeElementDeclaration =
        { "#if", "using ", "namespace", "#elif", "#else", "#endif", ";" };

    /// <summary>
    ///     Zapíše mi do .cs nové #elif
    ///     A1 je zde velmi důležité. Je třeba tam předávat fnwoe csproj než cs cesty. Tím jak přilinkovávám soubory, přidávají
    ///     se mi elif podle projektu kde je soubor fyzicky.
    /// </summary>
    /// <param name="reallyOccuredInFilesOrProjectNames"></param>
    /// <param name="pathCsToAppendElif"></param>
    /// <param name="contentCs"></param>
    /// <param name="AllNamespaces"></param>
    /// <returns></returns>
    public static async Task WriteNew(List<string> reallyOccuredInFilesOrProjectNames, string pathCsToAppendElif,
        List<string> contentCs, List<string> AllNamespaces)
    {
        var addTo_linked = true;

        var isCsFiles = reallyOccuredInFilesOrProjectNames.First().EndsWith(".cs");
        var reallyOccuredInFiles = reallyOccuredInFilesOrProjectNames.ToList();

        var c = contentCs ?? (await File.ReadAllLinesAsync(pathCsToAppendElif)).ToList();



        var result = await ParseSharpIfToFirstCodeElement(pathCsToAppendElif, contentCs, AllNamespaces, addTo_linked);

        var existingNamespace = result.foundedNamespaces;
        // pokud už je #if zavedený
        if (existingNamespace.Count > 0)
        {
            var dx = contentCs.IndexOf("#else");

            var sb = new StringBuilder();

            // nepotřebuji, protože budu přidávat až na místo #else
            //if (addEarlierAddedToFile)
            //{
            //    reallyOccuredInFiles.AddRange(existingNamespace);
            //    reallyOccuredInFiles = reallyOccuredInFiles.Distinct().ToList();
            //}

            /*
            teď je nejzásadnější jak to bude pracovat když je třída partial
            mělo by to mít svázané tokeny se souborem
            takže mi bude přidávat ns jen do těch souborů kde jsou potřeba
            na první pohled vše je zalité sluncem, uvidíme jak potom
             */

            foreach (var item in reallyOccuredInFiles)
            {
                var projectName = isCsFiles ? ProjectNameFromCsPath(item) : item;
                if (!existingNamespace.Contains(projectName))
                {
                    sb.AppendLine("#elif " + projectName);
                    sb.AppendLine(projectName);
                }
            }

            var ts = sb.ToString();
            c.InsertMultilineString(dx, ts);

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(pathCsToAppendElif, c, AllNamespaces, addTo_linked);

            var t = SHJoin.JoinNL(c);

            // TODO2
            await File.WriteAllTextAsync(pathCsToAppendElif, t);
        }
        else
        {
            // žádný #if tu ještě není

            // v opačném případě musím zapsat všechny + else. 
            // NS vezmu z toho co už v souboru bude

            var nss = c.Where(s => s.StartsWith("namespace ") && s.Trim() != "namespace");
            string nsToElse = null;
            if (!nss.Any())
            {
                nsToElse = GenerateNsFromPath(pathCsToAppendElif);
            }
            else
            {
                if (nss.Count() > 1) ThrowEx.Custom("Contains more than one NS outside of #if");

                nsToElse = nss.First();

                if (!nsToElse.EndsWith(";"))
                    ThrowEx.Custom(
                        "Namespace is not file scoped! Pro začátek by mělo stačit otevřít swld ve VS a u všech zaměnit NS. Na to aby se přidali csproj i co nejsou v sln utility mám. Hledal jsem nějaký kód v C#");

                nsToElse = nsToElse.Replace("namespace ", "");
                nsToElse = nsToElse.TrimEnd(';');
                nsToElse = SanitizeProjectName(nsToElse);
            }




            var first = true;
            var sb = new StringBuilder();

            sb.AppendLine("namespace");
            var projectNames = new List<string>();
            foreach (var item in reallyOccuredInFiles)
            {
                var projectName = isCsFiles ? ProjectNameFromCsPath(item) : item;
                if (projectName == nsToElse) continue;

                if (!projectNames.Contains(projectName)) projectNames.Add(projectName);
            }

            if (projectNames.Count == 0) return;

            foreach (var projectName in projectNames)
            {
                sb.AppendLine((first ? "#if " : "#elif ") + projectName);
                sb.AppendLine(projectName);

                first = false;
            }

            sb.AppendLine("#else");
            sb.AppendLine(nsToElse);
            sb.AppendLine("#endif");
            sb.AppendLine(";");

            var dx = SH.GetIndexesOfLinesStartingWith(c, d => d.StartsWith("namespace"));

            var ts = sb.ToString();

            if (dx.Count == 0)
            {
                c.InsertMultilineString(0, ts);
            }
            else
            {
                c.RemoveAt(dx.First());
                c.InsertMultilineString(dx.First(), ts);
            }

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(pathCsToAppendElif, c, AllNamespaces, addTo_linked);

            var t = SHJoin.JoinNL(c);

            await File.WriteAllTextAsync(pathCsToAppendElif, t);
        }
    }

    /// <summary>
    ///     Nechám jen písmena nebo čísla
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SanitizeProjectName(string s)
    {
        return string.Concat(s.Where(d => char.IsLetterOrDigit(d)));
    }

    public static string ProjectNameFromCsPath(string csPath)
    {
        var csprojPath = CsprojHelper.GetCsprojFromCsPath(csPath);
        var sanitized = SanitizeProjectName(Path.GetFileNameWithoutExtension(csprojPath));
        return sanitized;
    }

    private static string GenerateNsFromPath(string path)
    {
        // Už řeším v _5AddNamespaceByInputFolderName v CommandsToAllCsFiles.Cmd

        var csprojPath = CsprojHelper.GetCsprojFromCsPath(path);
        var csprojDir = FS.WithEndBs(Path.GetDirectoryName(csprojPath));

        string remain = null;

        if (path.Contains(csprojDir))
            remain = path.Replace(csprojDir, "");
        else
            ThrowEx.Custom($"{path} does not contains {csprojDir}");

        var p = remain.Split('\\');

        for (var i = p.Length - 1; i >= 0; i--)
        {
            p[i] = SanitizeProjectName(p[i]);
            break;
        }

        return remain.Replace("\\", ".");
    }

    private static async Task ThrowWhenThereIsNamespaceOutsideOfSharpIf(string path, List<string> c,
        List<string> allNamespaces, bool addTo_linked)
    {
        // zde příště pokračovat
        // zjistím indexy #if a #elif

        var parsed = await ParseSharpIfToFirstCodeElement(path, c, allNamespaces, addTo_linked);
        var allLinesBefore = parsed.allLinesBefore;
        var dxElif = SH.GetIndexesOfLinesStartingWith(allLinesBefore, d => d.StartsWith("#elif"));
        //var dxNs = SH.GetIndexesOfLinesStartingWith(allLinesBefore, d => d.StartsWith("namespace "));
        var dxNs = SH.GetIndexesOfLinesWhichContainsAnyOfStrings(allLinesBefore, allNamespaces);

        foreach (var item in dxElif)
        {
            var dx = dxNs.IndexOf(item + 1);
            if (dx != -1)
            {
                dxNs.RemoveAt(dx);
            }
            else
            {
                var lastLine = allLinesBefore[allLinesBefore.Count - 1];
                var lastLineTrimmed = lastLine.Replace("#elif ", "");


                ThrowEx.Custom(
                    $"On index {item + 1} is not namespace but should be after #elif, maybe will be enough insert {lastLineTrimmed} to virtualNamespace");
                Debugger.Break();
            }
        }

        var first = allLinesBefore.FirstOrDefault(d => d.StartsWith("#if"));
        if (first == null) throw new Exception("#if was not found");
        var dxIf = allLinesBefore.IndexOf(first);
        if (dxIf != -1)
        {
            if (dxIf != -1)
                dxNs.Remove(dxIf + 1);
            else
                ThrowEx.Custom($"On index {dxIf + 1} is not namespace but should be after #if");
        }

        var firstElse = allLinesBefore.FirstOrDefault(d => d.StartsWith("#else"));
        if (firstElse == null)
            throw new Exception("#else was not found");
        var dxElse = allLinesBefore.IndexOf(firstElse);
        if (dxElse != -1)
        {
            if (dxElse != -1)
                dxNs.Remove(dxElse + 1);
            else
                ThrowEx.Custom($"On index {dxElse + 1} is not namespace but should be after #else");
        }

        if (dxNs.Count != 0)
            ThrowEx.Custom(
                "Byly vyřazeny všechny namespace po #if nebo #elif. Přesto stále existují NS na těchto indexech: " +
                string.Join(',', dxNs.ConvertAll(d => d.ToString())));

        //    var dxNs = parsed.allLinesBefore.Select((middle, index) => new { middle, index })
        //.Where(x => x.middle.StartsWith("#elif "))
        //.Select(x => new { x.middle })
        //.Where(x => x.dt >= czas11 && x.dt <= czas22)
        //.Select(x => x.index)
        //.ToList();
    }

    /// <summary>
    ///     Tato metoda se může volat jen když SetAllNamespaces se dokoná
    ///     Proto ta první kontrola je v pohodě
    /// </summary>
    /// <param name="pathCs"></param>
    /// <param name="content"></param>
    /// <param name="AllNamespaces"></param>
    /// <returns></returns>
    public static async Task<ParseSharpIfToFirstCodeElementResult> ParseSharpIfToFirstCodeElement(string pathCs,
        List<string> content, List<string> AllNamespaces, bool addTo_linked)
    {
        if (addTo_linked && AllNamespaces.Count == 0) ThrowEx.Custom("AllNamespaces is empty!");



        var result = new List<string>();
        var linesBefore = new List<string>();

        var c = content ?? (await File.ReadAllLinesAsync(pathCs)).ToList();



        var result2 = new ParseSharpIfToFirstCodeElementResult();

        for (var i = 0; i < c.Count; i++)
        {
            c[i] = c[i].Trim();

            var line = c[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (!keywordsBeforeFirstCodeElementDeclaration.Any(keyword => line.Contains(keyword)) &&
                (addTo_linked ? !AllNamespaces.Contains(line) : true))
                //if (line.Contains("<"))
                //{
                //    result2.IsGeneric = true;
                //}
                break;

            /*
Tady je ale další problém
            NS kontroluji zda jsou an řádku, můžou se tam vyskytovat i tečky, pokud jsou v názvu projektu nebo jsou zanořené níže v adr. struktuře
            to ale znamená že budu mít více elif, vzrůstající s počtem složek
            To zase nebude vypadat tak hezky
            Ale když jsem to celou dobu třídil do složek, nebudu se jich teď zbavovat
            Snad těch #elif nebude tak hodně jak to vypadá

             */

            if (addTo_linked)
            {
                if (AllNamespaces != null && AllNamespaces.Contains(line)) result.Add(line);
            }
            else
            {
                result.Add(line);
            }

            linesBefore.Add(line);
        }

        if (!linesBefore.Contains("#endif") && linesBefore.Contains("#if"))
            ThrowEx.Custom(
                "linesBefore not contains #endif, celý proces procházení řádků nebyl dokonán. Asi chybělo v AllNamespaces něco mezi #if a #endif");

        result2.foundedNamespaces = result;
        result2.allLinesBefore = linesBefore;

        return result2;
    }
}