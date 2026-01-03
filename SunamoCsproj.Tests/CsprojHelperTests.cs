// variables names: ok
namespace SunamoCsproj.Tests.csproj;

public class CsprojHelperTests
{


    [Fact]
    public void ParseNamespaceFromCsFileTest()
    {
        var braceNamespaceResult = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c {
}", null);

        var fileScopedNamespaceResult = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c;

class A{}", null);

        Assert.Equal("c", braceNamespaceResult.Item2);
        Assert.Equal("c", fileScopedNamespaceResult.Item2);
    }




}
