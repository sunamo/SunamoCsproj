using SunamoCsproj._sunamo;
using SunamoCsproj.Results;
using SunamoExceptions.OnlyInSE;
using SunamoExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoCsproj;
public class CsprojNsHelper
{
    public static async Task WriteNew(List<string> reallyOccuredInFiles2, string path, List<string> content, List<string> AllNamespaces)
    {
        var reallyOccuredInFiles = reallyOccuredInFiles2.ToList();

        var c = content ?? (await File.ReadAllLinesAsync(path)).ToList();
        var result = await ParseSharpIfToFirstCodeElement(path, content, AllNamespaces);

        var existingNamespace = result.foundedNamespaces;
        // pokud už je #if zavedený
        if (existingNamespace.Count > 0)
        {
            var dx = content.IndexOf("#else");

            StringBuilder sb = new StringBuilder();

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
                if (!existingNamespace.Contains(item))
                {
                    sb.AppendLine("#elif " + item);
                    sb.AppendLine(item);
                }
            }

            var ts = sb.ToString();
            c.InsertMultilineString(dx, ts);

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(c, AllNamespaces);

            await File.WriteAllLinesAsync(path, c.ToArray());
        }
        else
        {
            // v opačném případě musím zapsat všechny + else. 
            // NS vezmu z toho co už v souboru bude

            var nss = c.Where(s => s.StartsWith("namespace ") && s.Trim() != "namespace");
            string ns = null;
            if (!ns.Any())
            {
                ns = GenerateNsFromPath(path);
            }
            else
            {
                if (nss.Count() > 1)
                {
                    throw new Exception("Contains more than one NS outside of #if");
                }

                ns = c.First();

                if (!ns.EndsWith(";"))
                {
                    throw new Exception("Namespace is not file scoped! Pro začátek by mělo stačit otevřít swld ve VS a u všech zaměnit NS. Na to aby se přidali csproj i co nejsou v sln utility mám. Hledal jsem nějaký kód v C#");
                }
            }

            bool first = true;
            StringBuilder sb = new StringBuilder();

            foreach (var item in reallyOccuredInFiles)
            {
                sb.AppendLine((first ? "#if " : "#elif ") + item);
                sb.AppendLine(item);

                first = false;
            }
            sb.AppendLine("#endif");

            var dx = SH.GetIndexesOfLinesStartingWith(c, d => d.StartsWith("namespace"));
            var ts = sb.ToString();

            if (!dx.Any())
            {
                c.InsertMultilineString(0, ts);
            }
            else
            {
                c.InsertMultilineString(dx.First(), ts);
            }

            await ThrowWhenThereIsNamespaceOutsideOfSharpIf(c, AllNamespaces);

            await File.WriteAllLinesAsync(path, c.ToArray());
        }
    }

    private static string GenerateNsFromPath(string path)
    {
        // Už řeším v _5AddNamespaceByInputFolderName v CommandsToAllCsFiles.Cmd

        var csprojPath = CsprojHelper.GetCsprojFromCsPath(path);
        var csprojDir = FS.WithEndBs(Path.GetDirectoryName(csprojPath));

        string remain = null;

        if (path.Contains(csprojDir))
        {
            remain = path.Replace(csprojDir, "");
        }
        else
        {
            throw new Exception($"{path} does not contains {csprojDir}");
        }

        return remain.Replace("\\", ".");
    }

    private static async Task ThrowWhenThereIsNamespaceOutsideOfSharpIf(List<string> c, List<string> allNamespaces)
    {
        // zde příště pokračovat
        // zjistím indexy #if a #elif

        var parsed = await ParseSharpIfToFirstCodeElement(null, c, allNamespaces);
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
                throw new Exception($"On index {item + 1} is not namespace but should be after #elif");
            }
        }

        var first = allLinesBefore.First(d => d.StartsWith("#if"));
        var dxIf = allLinesBefore.IndexOf(first);
        if (dxIf != -1)
        {
            if (dxIf != -1)
            {
                dxNs.Remove(dxIf + 1);
            }
            else
            {
                throw new Exception($"On index {dxIf + 1} is not namespace but should be after #if");
            }
        }

        var dxElse = allLinesBefore.IndexOf("#else");
        if (dxElse != -1)
        {
            if (dxElse != -1)
            {
                dxNs.Remove(dxElse + 1);
            }
            else
            {
                throw new Exception($"On index {dxElse + 1} is not namespace but should be after #else");
            }
        }

        if (dxNs.Any())
        {
            throw new Exception("Byly vyřazeny všechny namespace po #if nebo #elif. Přesto stále existují NS na těchto indexech: " + string.Join(',', dxNs.ConvertAll(d => d.ToString())));
        }

        //    var dxNs = parsed.allLinesBefore.Select((middle, index) => new { middle, index })
        //.Where(x => x.middle.StartsWith("#elif "))
        //.Select(x => new { x.middle })
        //.Where(x => x.dt >= czas11 && x.dt <= czas22)
        //.Select(x => x.index)
        //.ToList();


    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    /// <param name="AllNamespaces"></param>
    /// <returns></returns>
    public static async Task<ParseSharpIfToFirstCodeElementResult> ParseSharpIfToFirstCodeElement(string path, List<string> content, List<string> AllNamespaces)
    {
        List<string> result = new List<string>();
        List<string> linesBefore = new List<string>();

        // musí být namespace bez mezery na konci, takto se užívá v #if
        string[] keywordsBefore = new string[] { "#if", "using ", "namespace", "#elif", "#else", "#endif", ";" };

        var c = content ?? (await File.ReadAllLinesAsync(path)).ToList();
        for (int i = 0; i < c.Count; i++)
        {
            c[i] = c[i].Trim();

            var line = c[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (!keywordsBefore.Any(keyword => line.Contains(keyword)) && !AllNamespaces.Contains(line))
            {
                break;
            }

            /*
Tady je ale další problém
            NS kontroluji zda jsou an řádku, můžou se tam vyskytovat i tečky, pokud jsou v názvu projektu nebo jsou zanořené níže v adr. struktuře
            to ale znamená že budu mít více elif, vzrůstající s počtem složek
            To zase nebude vypadat tak hezky
            Ale když jsem to celou dobu třídil do složek, nebudu se jich teď zbavovat
            Snad těch #elif nebude tak hodně jak to vypadá
           
             */

            if (AllNamespaces != null && AllNamespaces.Contains(line))
            {
                result.Add(line);
            }

            linesBefore.Add(line);
        }

        if (!linesBefore.Contains("#endif"))
        {
            throw new Exception("linesBefore not contains #endif, celý proces procházení řádků nebyl dokonán. Asi chybělo v AllNamespaces něco mezi #if a #endif");
        }

        return new ParseSharpIfToFirstCodeElementResult { foundedNamespaces = result, allLinesBefore = linesBefore };
    }
}
