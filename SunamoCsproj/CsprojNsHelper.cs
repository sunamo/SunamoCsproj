namespace SunamoCsproj;

public class CsprojNsHelper
{
    public static string[] KeywordsBeforeFirstCodeElementDeclaration =
        { "#if", "using ", "namespace", "#elif", "#else", "#endif", ";" };

    // EN: Writes new #elif directives to .cs file. Parameter reallyOccuredInFilesOrProjectNames is critical - must pass csproj paths, not cs paths, because linked files add elif based on physical project location.
    // CZ: Zapíše do .cs nové #elif direktivy. Parametr reallyOccuredInFilesOrProjectNames je kritický - musí předávat csproj cesty, ne cs cesty, protože linkované soubory přidávají elif podle fyzického umístění projektu.
    public static async Task WriteNew(List<string> reallyOccuredInFilesOrProjectNames, string pathCsToAppendElif,
        List<string> contentCs, List<string> AllNamespaces)
    {
        var addTo_linked = true;

        var isCsFiles = reallyOccuredInFilesOrProjectNames.First().EndsWith(".cs");
        var reallyOccuredInFiles = reallyOccuredInFilesOrProjectNames.ToList();

        var count = contentCs ?? (await FileAsync.ReadAllLinesAsync(pathCsToAppendElif)).ToList();



        var result = await ParseSharpIfToFirstCodeElement(pathCsToAppendElif, count, AllNamespaces, addTo_linked);

        var existingNamespace = result.FoundedNamespaces!;
        // EN: If #if is already introduced
        // CZ: Pokud už je #if zavedený
        if (existingNamespace.Count > 0)
        {
            var indexElse = count.IndexOf("#else");

            var stringBuilder = new StringBuilder();

            // EN: Not needed because we will add at #else position
            // CZ: Nepotřebuji, protože budu přidávat až na místo #else
            //if (addEarlierAddedToFile)
            //{
            //    reallyOccuredInFiles.AddRange(existingNamespace);
            //    reallyOccuredInFiles = reallyOccuredInFiles.Distinct().ToList();
            //}

            // EN: Critical: how will this work when class is partial - should have tokens bound to file, so ns will be added only to files where needed
            // CZ: Nejzásadnější: jak to bude pracovat když je třída partial - mělo by to mít svázané tokeny se souborem, takže ns jen do souborů kde jsou potřeba

            foreach (var item in reallyOccuredInFiles)
            {
                var projectName = isCsFiles ? ProjectNameFromCsPath(item) : item;
                if (!existingNamespace.Contains(projectName))
                {
                    stringBuilder.AppendLine("#elif " + projectName);
                    stringBuilder.AppendLine(projectName);
                }
            }

            var textToInsert = stringBuilder.ToString();
            count.InsertMultilineString(indexElse, textToInsert);

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(pathCsToAppendElif, count, AllNamespaces, addTo_linked);

            var temp = SHJoin.JoinNL(count);

            // TODO2
            await FileAsync.WriteAllTextAsync(pathCsToAppendElif, temp);
        }
        else
        {
            // EN: No #if here yet
            // CZ: Žádný #if tu ještě není

            // EN: Otherwise must write all + else. NS will be taken from what's already in file
            // CZ: V opačném případě musím zapsat všechny + else. NS vezmu z toho co už v souboru bude

            var namespaceLines = count.Where(text => text.StartsWith("namespace ") && text.Trim() != "namespace");
            string? nsToElse = null;
            if (!namespaceLines.Any())
            {
                nsToElse = GenerateNsFromPath(pathCsToAppendElif);
            }
            else
            {
                if (namespaceLines.Count() > 1) ThrowEx.Custom("Contains more than one NS outside of #if");

                nsToElse = namespaceLines.First();

                if (!nsToElse.EndsWith(";"))
                    ThrowEx.Custom(
                        "Namespace is not file scoped! For start it should be enough to open swld in VS and replace NS everywhere. For adding csproj that are not in sln I have utility.");

                nsToElse = nsToElse.Replace("namespace ", "");
                nsToElse = nsToElse.TrimEnd(';');
                nsToElse = SanitizeProjectName(nsToElse);
            }




            var first = true;
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("namespace");
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
                stringBuilder.AppendLine((first ? "#if " : "#elif ") + projectName);
                stringBuilder.AppendLine(projectName);

                first = false;
            }

            stringBuilder.AppendLine("#else");
            stringBuilder.AppendLine(nsToElse);
            stringBuilder.AppendLine("#endif");
            stringBuilder.AppendLine(";");

            var namespaceIndexes = SH.GetIndexesOfLinesStartingWith(count, line => line.StartsWith("namespace"));

            var textToInsert = stringBuilder.ToString();

            if (namespaceIndexes.Count == 0)
            {
                count.InsertMultilineString(0, textToInsert);
            }
            else
            {
                count.RemoveAt(namespaceIndexes.First());
                count.InsertMultilineString(namespaceIndexes.First(), textToInsert);
            }

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(pathCsToAppendElif, count, AllNamespaces, addTo_linked);

            var temp = SHJoin.JoinNL(count);

            await FileAsync.WriteAllTextAsync(pathCsToAppendElif, temp);
        }
    }

    public static string SanitizeProjectName(string text)
    {
        return string.Concat(text.Where(character => char.IsLetterOrDigit(character)));
    }

    public static string ProjectNameFromCsPath(string csPath)
    {
#pragma warning disable CS0618 // EN: Type or member is obsolete - internal usage allowed / CZ: Typ nebo člen je zastaralý - interní použití povoleno
        var csprojPath = CsprojHelper.GetCsprojFromCsPath(csPath);
#pragma warning restore CS0618
        var sanitized = SanitizeProjectName(Path.GetFileNameWithoutExtension(csprojPath));
        return sanitized;
    }

    private static string GenerateNsFromPath(string path)
    {
        // EN: Already handled in _5AddNamespaceByInputFolderName in CommandsToAllCsFiles.Cmd
        // CZ: Už řeším v _5AddNamespaceByInputFolderName v CommandsToAllCsFiles.Cmd

#pragma warning disable CS0618 // EN: Type or member is obsolete - internal usage allowed / CZ: Typ nebo člen je zastaralý - interní použití povoleno
        var csprojPath = CsprojHelper.GetCsprojFromCsPath(path);
#pragma warning restore CS0618
        var csprojDir = FS.WithEndBs(Path.GetDirectoryName(csprojPath)!);

        string? remain = null;

        if (path.Contains(csprojDir))
            remain = path.Replace(csprojDir, "");
        else
            ThrowEx.Custom($"{path} does not contains {csprojDir}");

        var parameter = remain!.Split('\\');

        // EN: Sanitize only the last element instead of unreachable loop
        // CZ: Sanitizovat pouze poslední element místo nedosažitelné smyčky
        var lastIndex = parameter.Length - 1;
        if (lastIndex >= 0)
        {
            parameter[lastIndex] = SanitizeProjectName(parameter[lastIndex]);
        }

        return remain.Replace("\\", ".");
    }

    private static async Task ThrowWhenThereIsNamespaceOutsideOfSharpIf(string path, List<string> count,
        List<string> allNamespaces, bool addTo_linked)
    {
        // EN: Continue here next time - find indexes of #if and #elif
        // CZ: Zde příště pokračovat - zjistím indexy #if a #elif

        var parsed = await ParseSharpIfToFirstCodeElement(path, count, allNamespaces, addTo_linked);
        var allLinesBefore = parsed.AllLinesBefore!;
        var elifIndexes = SH.GetIndexesOfLinesStartingWith(allLinesBefore, line => line.StartsWith("#elif"));
        //var namespaceIndexes = SH.GetIndexesOfLinesStartingWith(allLinesBefore, line => line.StartsWith("namespace "));
        var namespaceIndexes = SH.GetIndexesOfLinesWhichContainsAnyOfStrings(allLinesBefore, allNamespaces);

        foreach (var item in elifIndexes)
        {
            var namespaceIndex = namespaceIndexes.IndexOf(item + 1);
            if (namespaceIndex != -1)
            {
                namespaceIndexes.RemoveAt(namespaceIndex);
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

        var first = allLinesBefore.FirstOrDefault(line => line.StartsWith("#if"));
        if (first == null) throw new Exception("#if was not found");
        var ifIndex = allLinesBefore.IndexOf(first);
        if (ifIndex != -1)
        {
            if (ifIndex != -1)
                namespaceIndexes.Remove(ifIndex + 1);
            else
                ThrowEx.Custom($"On index {ifIndex + 1} is not namespace but should be after #if");
        }

        var firstElse = allLinesBefore.FirstOrDefault(line => line.StartsWith("#else"));
        if (firstElse == null)
            throw new Exception("#else was not found");
        var elseIndex = allLinesBefore.IndexOf(firstElse);
        if (elseIndex != -1)
        {
            if (elseIndex != -1)
                namespaceIndexes.Remove(elseIndex + 1);
            else
                ThrowEx.Custom($"On index {elseIndex + 1} is not namespace but should be after #else");
        }

        if (namespaceIndexes.Count != 0)
            ThrowEx.Custom(
                "All namespaces after #if or #elif were excluded. However, there are still NS at these indexes: " +
                string.Join(",", namespaceIndexes.ConvertAll(index => index.ToString())));

        //    var dxNs = parsed.allLinesBefore.Select((middle, index) => new { middle, index })
        //.Where(x => x.middle.StartsWith("#elif "))
        //.Select(x => new { x.middle })
        //.Where(x => x.dt >= czas11 && x.dt <= czas22)
        //.Select(x => x.index)
        //.ToList();
    }

    // EN: Parses #if directives to first code element. This method can be called only when SetAllNamespaces completes, so first check is OK.
    // CZ: Parsuje #if direktivy po první kódový element. Tato metoda se může volat jen když SetAllNamespaces se dokoná, proto ta první kontrola je v pohodě.
    public static async Task<ParseSharpIfToFirstCodeElementResult> ParseSharpIfToFirstCodeElement(string? pathCs,
        List<string> content, List<string> AllNamespaces, bool addTo_linked)
    {
        if (addTo_linked && AllNamespaces.Count == 0) ThrowEx.Custom("AllNamespaces is empty!");



        var result = new List<string>();
        var linesBefore = new List<string>();

        var count = content ?? (await FileAsync.ReadAllLinesAsync(pathCs!)).ToList();



        var result2 = new ParseSharpIfToFirstCodeElementResult();

        for (var index = 0; index < count.Count; index++)
        {
            count[index] = count[index].Trim();

            var line = count[index];
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (!KeywordsBeforeFirstCodeElementDeclaration.Any(keyword => line.Contains(keyword)) &&
                (addTo_linked ? !AllNamespaces!.Contains(line) : true))
                //if (line.Contains("<"))
                //{
                //    result2.IsGeneric = true;
                //}
                break;

            // EN: Another problem: NS checked if on line, can have dots if in project name or nested deeper in folder structure - means more elif, growing text by folder count. Won't look nice but won't remove folders now after organizing. Hopefully not too many #elif
            // CZ: Další problém: NS kontroluji zda jsou na řádku, můžou se tam vyskytovat i tečky pokud jsou v názvu projektu nebo jsou zanořené níže v adresářové struktuře - to znamená více elif, vzrůstající text počtem složek. Nebude vypadat hezky ale nebudu se složek zbavovat po tom co jsem do nich třídil. Snad těch #elif nebude moc

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
                "linesBefore not contains #endif, whole line iteration process was not completed. Probably something missing in AllNamespaces between #if and #endif");

        result2.FoundedNamespaces = result;
        result2.AllLinesBefore = linesBefore;

        return result2;
    }
}
