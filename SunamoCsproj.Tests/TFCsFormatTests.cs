//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SunamoDevCode.Tests;
//public class TFCsFormatTests
//{
//    [Fact]
//    public async Task WriteAllLinesTest()
//    {
//        const string path = @"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoGetFiles\_sunamo\XlfKeys.cs";
//        var l = await File.ReadAllLinesAsync(path);
//        l[0] = "namespace SunamoGetFiles._sunamo;";
//        await TFCsFormat.WriteAllLines(path, l);


//    }
//}
