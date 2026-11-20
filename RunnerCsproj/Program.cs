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



        #region MyRegion


        #endregion


    }
}