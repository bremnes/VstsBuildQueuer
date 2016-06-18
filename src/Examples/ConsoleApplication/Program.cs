using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;

namespace VstsBuildQueuer.Examples.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            try
            {
                IBuildQueuer queue = new BuildQueuer();
                await queue
                    .InitConfig(
                        "MyProject",
                        "https://myaccount.visualstudio.com",
                        new VssBasicCredential(string.Empty, "3ucqyyxfsadasv4ffjsj6mp3c6abg7kez3xwxdikijgnh21d4kta"), // this is the personal access token key
                        Console.WriteLine,
                        buildDefinitionName =>
                        {
                            Console.WriteLine($"FAILED BUILD: {buildDefinitionName}, see build log for more information. Fix build manually and press [Enter] if you want to continue with the rest of the builds");
                            Console.ReadLine();
                            Console.WriteLine($"Continuing past failed build {buildDefinitionName}..");
                        })
                    .QueueBuilds("CommonUtils") // queues and wait for result before continuing
                    .QueueBuilds("SharedClasses", "BusinessLogic") // queue both in parallel, continue when both are completed
                    .QueueRemainingBuildDefinitions("", "Integration"); // queue remaining build definitions that haven't been built up to this point and doesn't include "Integration" in their name
                
                Console.WriteLine("BuildQueuer completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured {ex}");
            }

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();
        }
    }
}
