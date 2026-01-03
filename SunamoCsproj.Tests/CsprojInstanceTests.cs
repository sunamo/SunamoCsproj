// variables names: ok
using System.IO.Compression;

namespace SunamoCsproj.Tests;
public class CsprojInstanceTests : SwdRepoNames
{
    [Fact]
    public async Task RemoveSingleItemGroupTest()
    {
        CsprojInstance csprojInstance = new CsprojInstance(await File.ReadAllTextAsync(@"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoAsync\SunamoAsync.csproj"));

        csprojInstance.RemoveSingleItemGroup("SunamoArgs", Items.ItemGroupTagName.PackageReference);
    }

    //

    [Fact]
    public void CreateOrReplaceMicrosoft_Extensions_Logging_AbstractionsTest()
    {
        CsprojInstance csprojInstance = new CsprojInstance(@"E:\vs\Projects\_ut2\PlatformIndependentNuGetPackages.Tests\SunamoCsproj.Tests\SunamoCsproj.Tests.csproj");
        csprojInstance.CreateOrReplaceMicrosoft_Extensions_Logging_Abstractions();



        csprojInstance.Save();
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
        CsprojInstance csprojInstance = new(csprojPath);
        csprojInstance.AddRemoveNoWarn(true, "CA1822");

        csprojInstance.Save();
    }

    //

    [Fact]
    public void ItemsInItemGroupTest()
    {
        CsprojInstance csprojInstance = new(@"E:\vs\Projects\_WhenNeedToEditAllCorruptedSlns\CommandsToAllCsprojs.Cmd\CommandsToAllCsprojs.Cmd\CommandsToAllCsprojs.Cmd.csproj");

        var packageReferences = csprojInstance.ItemsInItemGroup(ItemGroupTagName.PackageReference);
        var projectReferences = csprojInstance.ItemsInItemGroup(ItemGroupTagName.ProjectReference);

    }

    [Fact]
    public async Task RemoveDuplicatesInItemGroupTest()
    {
        CsprojInstance csprojInstance = new(@"D:\_Test\PlatformIndependentNuGetPackages\SunamoCsproj\DetectDuplicatedNugetPackages.csproj");

        var newCsprojContent = await csprojInstance.RemoveDuplicatedProjectAndPackageReferences(null);
        await File.WriteAllTextAsync(@"E:\vs\Projects\_tests\CompareTwoFiles\CompareTwoFiles\xml\1.xml", newCsprojContent);
    }


    [Fact]
    public void PropertyGroupItemContentTest()
    {
        CsprojInstance csprojInstance = new(@"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCsproj\SunamoCsproj.csproj");

        //var data = await CsprojHelper.PropertyGroupItemContent(@"E:\vs\Projects\_ut2\PlatformIndependentNuGetPackages.Tests\SunamoCsproj.Tests\SunamoCsproj.Tests.csproj", "Description");
        var description = csprojInstance.PropertyGroupItemContent("Description");
    }

    [Fact]
    public void AddSunamoSharedPlusOtherAndThenAddAnother_EveryMustBeUnique()
    {
        // Arrange

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml("<Project Sdk=\"Microsoft.NET.Sdk\"></Project>");

        const string SunamoShared = "SunamoShared";

        var csprojInstance = new CsprojInstance(xmlDocument);
        csprojInstance.CreateNewPackageReference(SunamoShared, "*");
        csprojInstance.CreateNewPackageReference(SunamoInterfaces, "*");

        var xmlAfterFirstPackages = xmlDocument.OuterXml;
        Console.WriteLine(xmlAfterFirstPackages);
        Debugger.Break();

        foreach (var item in new string[] { SunamoArgs, SunamoInterfaces })
        {
            csprojInstance.CreateNewPackageReference(item, "1");
        }

        var xmlAfterSecondPackages = xmlDocument.OuterXml;
        Console.WriteLine(xmlAfterSecondPackages);
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
