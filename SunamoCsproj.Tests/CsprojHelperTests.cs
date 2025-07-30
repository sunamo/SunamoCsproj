namespace SunamoCsproj.Tests.csproj;

public class CsprojHelperTests
{


    [Fact]
    public void ParseNamespaceFromCsFileTest()
    {
        var actual1 = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c {
}", null);

        var actual2 = CsprojHelper.ParseNamespaceFromCsFile(@"using a;

namespace c;

class A{}", null);

        Assert.Equal("c", actual1.Item2);
        Assert.Equal("c", actual2.Item2);
    }




}
