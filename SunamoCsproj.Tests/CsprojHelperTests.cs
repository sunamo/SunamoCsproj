// variables names: ok
namespace SunamoCsproj.Tests.csproj;

public class CsprojHelperTests
{


    [Fact]
    public void ParseNamespaceFromCsFileTest()
    {
#pragma warning disable CS0618 // EN: Type or member is obsolete - internal testing allowed / CZ: Typ nebo člen je zastaralý - interní testování povoleno
        var braceNamespaceResult = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c {
}", null);

        var fileScopedNamespaceResult = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c;

class A{}", null);

        Assert.Equal("c", braceNamespaceResult.Item2);
        Assert.Equal("c", fileScopedNamespaceResult.Item2);
#pragma warning restore CS0618
    }




}
