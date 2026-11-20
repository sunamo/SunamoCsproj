namespace SunamoCsproj.Tests;
public class CsprojNsHelperTests
{
    [Fact]
    public async Task ParseSharpIfToFirstCodeElementTest()
    {
        var input = @"using System.Threading.Tasks;

namespace SunamoCsproj;
public class CsprojNsHelper
{
}";

        var inputLines = SHGetLines.GetLines(input);

        List<string> allNamespaces = new List<string>();
        var parseResult = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputLines, allNamespaces, false);

        // note that it removes all empty lines for me
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace SunamoCsproj;"), parseResult.AllLinesBefore);
        Assert.Equal(new List<string>(), parseResult.FoundedNamespaces);

    }

    /// <summary>
    /// It does not throw here, it is called only in WriteNew
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ParseSharpIfToFirstCodeElementTest2_MustThrowExceptionNamespaceOutsideCsprojNsHelper()
    {
        var input = @"using System.Threading.Tasks;

namespace
;

namespace SunamoCsproj;
public class CsprojNsHelper
{
}";

        var inputLines = SHGetLines.GetLines(input);

        // Everything between #if and #endif must be passed, otherwise it would not return even all the lines before
        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var parseResult = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputLines, allNamespaces, false);

        // note that it removes all empty lines for me
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace
;
namespace SunamoCsproj;
"), parseResult.AllLinesBefore);
        Assert.Equal(new List<string>(new string[] { "SunamoString", "SunamoCsproj" }), parseResult.FoundedNamespaces);
    }

    [Fact]
    public async Task ParseSharpIfToFirstCodeElementTest3()
    {
        var input = @"using System.Threading.Tasks;

namespace
;

public class CsprojNsHelper
{
}";

        var inputLines = SHGetLines.GetLines(input);

        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var parseResult = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputLines, allNamespaces, false);

        // note that it removes all empty lines for me
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace
;"), parseResult.AllLinesBefore);
        Assert.Equal(new List<string>(new string[] { "SunamoString", "SunamoCsproj" }), parseResult.FoundedNamespaces);

    }

    [Fact]
    public async Task WriteNewTest()
    {
        const string basePath = @"D:\_Test\PlatformIndependentNuGetPackages\SunamoCsproj\";
        await CsprojNsHelper.WriteNew(new List<string>(new String[] { "S1", "S2" }), basePath + @"WriteNewTest.cs", SHGetLines.GetLines(@"using System.Threading.Tasks;

namespace
;

public class CsprojNsHelper
{
}"),
// here I must add all NS that I want to add
new List<string>(new String[] { "SunamoString", "SunamoCsproj", "S1", "S2" }));


    }
}
