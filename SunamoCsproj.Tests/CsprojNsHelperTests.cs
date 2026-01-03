// variables names: ok
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

        // nutno si povšimnout že mi to dává pryč všechny prázdné řádky
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace SunamoCsproj;"), parseResult.AllLinesBefore);
        Assert.Equal(new List<string>(), parseResult.FoundedNamespaces);

    }

    /// <summary>
    /// Zde mi to nevyhodí, volá se až ve WriteNew
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ParseSharpIfToFirstCodeElementTest2_MustThrowExceptionNamespaceOutsideCsprojNsHelper()
    {
        var input = @"using System.Threading.Tasks;

namespace
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
;

namespace SunamoCsproj;
public class CsprojNsHelper
{
}";

        var inputLines = SHGetLines.GetLines(input);

        // Nutno předat vše mezi #if, #endif protože jinak by mi to nevrátilo ani všechny řádky před
        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var parseResult = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputLines, allNamespaces, false);

        // nutno si povšimnout že mi to dává pryč všechny prázdné řádky
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
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
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
;

public class CsprojNsHelper
{
}";

        var inputLines = SHGetLines.GetLines(input);

        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var parseResult = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputLines, allNamespaces, false);

        // nutno si povšimnout že mi to dává pryč všechny prázdné řádky
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
;"), parseResult.AllLinesBefore);
        Assert.Equal(new List<string>(new string[] { "SunamoString", "SunamoCsproj" }), parseResult.FoundedNamespaces);

    }

    [Fact]
    public async Task WriteNewTest()
    {
        const string basePath = @"D:\_Test\PlatformIndependentNuGetPackages\SunamoCsproj\";
        await CsprojNsHelper.WriteNew(new List<string>(new String[] { "S1", "S2" }), basePath + @"WriteNewTest.cs", SHGetLines.GetLines(@"using System.Threading.Tasks;

namespace
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
;

public class CsprojNsHelper
{
}"),
// zde musím přidat všechny NS co chci přidat
new List<string>(new String[] { "SunamoString", "SunamoCsproj", "S1", "S2" }));


    }
}
