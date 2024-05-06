using SunamoCsproj._sunamo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SunamoCsproj._Tests;
public class SHTests
{
    [Fact]
    public void GetIndexesOfLinesStartingWithTest()
    {
        var input = SHGetLines.GetLines(@"a
c
a");

        var actual = SH.GetIndexesOfLinesStartingWith(input, s => s.StartsWith("a"));
    }
}
