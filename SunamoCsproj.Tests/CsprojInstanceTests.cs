// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

using System.IO.Compression;

namespace SunamoCsproj.Tests;
public class CsprojInstanceTests : SwdRepoNames
{
    [Fact]
    public async Task RemoveSingleItemGroupTest()
    {
        CsprojInstance csp = new CsprojInstance(await File.ReadAllTextAsync(@"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoAsync\SunamoAsync.csproj"));

        csp.RemoveSingleItemGroup("SunamoArgs", Items.ItemGroupTagName.PackageReference);
    }

    //

    [Fact]
    public void CreateOrReplaceMicrosoft_Extensions_Logging_AbstractionsTest()
    {
        CsprojInstance csi = new CsprojInstance(@"E:\vs\Projects\_ut2\PlatformIndependentNuGetPackages.Tests\SunamoCsproj.Tests\SunamoCsproj.Tests.csproj");
        csi.CreateOrReplaceMicrosoft_Extensions_Logging_Abstractions();



        csi.Save();
    }

    [Fact]
    public void AddRemoveNoWarnTest()
    {
        var pathFile = @"D:\_Test\sunamo\SunamoCsproj\CsProjInstance\Original.zip";
        ZipFile.ExtractToDirectory(pathFile, Path.GetDirectoryName(pathFile)!, true);

        var csprojPath = @"D:\_Test\sunamo\SunamoCsproj\CsProjInstance\Original\Example.csproj";
        AddRemoveNoWarnTestWorker(csprojPath);
    }

    [Fact]
    public void AddRemoveNoWarnTest2()
    {
        var zipFile = @"E:\vs\Projects\sunamo.notmine\UAParser\UAParser.zip";
        ZipFile.ExtractToDirectory(zipFile, Path.GetDirectoryName(zipFile), true);

        AddRemoveNoWarnTestWorker(@"E:\vs\Projects\sunamo.notmine\UAParser\UAParser.csproj");
    }

    public void AddRemoveNoWarnTestWorker(string csprojPath)
    {
        CsprojInstance csi = new(csprojPath);
        csi.AddRemoveNoWarn(true, "CA1822");

        csi.Save();
    }

    //

    [Fact]
    public void ItemsInItemGroupTest()
    {
        CsprojInstance csi = new(@"E:\vs\Projects\_WhenNeedToEditAllCorruptedSlns\CommandsToAllCsprojs.Cmd\CommandsToAllCsprojs.Cmd\CommandsToAllCsprojs.Cmd.csproj");

        var data = csi.ItemsInItemGroup(ItemGroupTagName.PackageReference);
        var d2 = csi.ItemsInItemGroup(ItemGroupTagName.ProjectReference);

    }

    [Fact]
    public async Task RemoveDuplicatesInItemGroupTest()
    {
        CsprojInstance csi = new(@"D:\_Test\PlatformIndependentNuGetPackages\SunamoCsproj\DetectDuplicatedNugetPackages.csproj");

        var newCsprojContent = await csi.RemoveDuplicatedProjectAndPackageReferences(null);
        await File.WriteAllTextAsync(@"E:\vs\Projects\_tests\CompareTwoFiles\CompareTwoFiles\xml\1.xml", newCsprojContent);
    }


    [Fact]
    public void PropertyGroupItemContentTest()
    {
        CsprojInstance csi = new(@"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\SunamoCsproj.csproj");

        //var data = await CsprojHelper.PropertyGroupItemContent(@"E:\vs\Projects\_ut2\PlatformIndependentNuGetPackages.Tests\SunamoCsproj.Tests\SunamoCsproj.Tests.csproj", "Description");
        var d2 = csi.PropertyGroupItemContent("Description");
    }

    [Fact]
    public void AddSunamoSharedPlusOtherAndThenAddAnother_EveryMustBeUnique()
    {
        // Arrange

        XmlDocument xd = new XmlDocument();
        xd.LoadXml("<Project Sdk=\"Microsoft.NET.Sdk\"></Project>");

        const string SunamoShared = "SunamoShared";

        var csi = new CsprojInstance(xd);
        csi.CreateNewPackageReference(SunamoShared, "*");
        csi.CreateNewPackageReference(SunamoInterfaces, "*");

        var xml = xd.OuterXml;
        Console.WriteLine(xml);
        Debugger.Break();

        foreach (var item in new string[] { SunamoArgs, SunamoInterfaces })
        {
            csi.CreateNewPackageReference(item, "1");
        }

        var xml2 = xd.OuterXml;
        Console.WriteLine(xml2);
        Debugger.Break();
        // Act

        // Assert
    }

    [Fact]
    public void AddTagsForNugetReadmeFile()
    {
        //var csi = new CsprojInstance();
    }
}
