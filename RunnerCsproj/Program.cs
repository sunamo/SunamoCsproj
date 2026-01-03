// variables names: ok
namespace RunnerCsproj;

using SunamoCsproj.Tests;

internal class Program
{
    static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }

    static async Task MainAsync()
    {
        CsprojInstanceTests d = new();

        d.CreateOrReplaceMicrosoft_Extensions_Logging_AbstractionsTest();

        //d.PropertyGroupItemContentTest();
        //d.AddRemoveNoWarnTest2();

        //d.AddRemoveNoWarnTestWorker(@"E:\vs\Projects\LearnCsharp\LearnSwagger\LearnSwagger.csproj");

        #region MyRegion
        //var item = @"E:\vs\Projects\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj";
        //var cs = new SunamoCsproj.CsprojInstance(item);

        //cs.AddRemoveDefineConstant(false, "CA1822");

        //cs.Save(); 
        #endregion


    }
}