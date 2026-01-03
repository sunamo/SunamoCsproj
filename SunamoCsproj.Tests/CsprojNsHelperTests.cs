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

        var inputL = SHGetLines.GetLines(input);

        List<string> allNamespaces = new List<string>();
        var actual = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputL, allNamespaces, false);

        // nutno si povšimnout že mi to dává pryč všechny prázdné řádky
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace SunamoCsproj;"), actual.AllLinesBefore);
        Assert.Equal(new List<string>(), actual.FoundedNamespaces);

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

        var inputL = SHGetLines.GetLines(input);

        // Nutno předat vše mezi #if, #endif protože jinak by mi to nevrátilo ani všechny řádky před
        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var actual = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputL, allNamespaces, false);

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
"), actual.AllLinesBefore);
        Assert.Equal(new List<string>(new string[] { "SunamoString", "SunamoCsproj" }), actual.FoundedNamespaces);
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

        var inputL = SHGetLines.GetLines(input);

        List<string> allNamespaces = new List<string>(new string[] { "SunamoString", "SunamoCsproj" });
        var actual = await CsprojNsHelper.ParseSharpIfToFirstCodeElement(null, inputL, allNamespaces, false);

        // nutno si povšimnout že mi to dává pryč všechny prázdné řádky
        Assert.Equal(SHGetLines.GetLines(@"using System.Threading.Tasks;
namespace
#if SunamoString
SunamoString
#else
SunamoCsproj
#endif
;"), actual.AllLinesBefore);
        Assert.Equal(new List<string>(new string[] { "SunamoString", "SunamoCsproj" }), actual.FoundedNamespaces);

    }

    [Fact]
    public async Task WriteNewTest()
    {
        const string bp = @"D:\_Test\PlatformIndependentNuGetPackages\SunamoCsproj\";
        await CsprojNsHelper.WriteNew(new List<string>(new String[] { "S1", "S2" }), bp + @"WriteNewTest.cs", SHGetLines.GetLines(@"using System.Threading.Tasks;
 
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
