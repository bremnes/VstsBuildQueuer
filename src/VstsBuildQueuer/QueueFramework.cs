using System;
using System.Linq;
using System.Threading.Tasks;

namespace VstsBuildQueuer
{
    public class QueueFramework : IQueueFramework
    {
        private readonly Random _rnd = new Random();

        public async Task<QueueFramework> InitConfig()
        {
            Console.WriteLine("Configuration is being initialized");
            await Task.Delay(300);
            Console.WriteLine("Initialization completed");

            return this;
        }

        public async Task<QueueFramework> QueueBuilds(params string[] buildDefinitionNames)
        {
            await Task.WhenAll(buildDefinitionNames.Select(TriggerIndividualBuildAsync));
            return this;
        }

        private async Task TriggerIndividualBuildAsync(string buildDefinitionName)
        {
            var simulatedBuildTime = _rnd.Next(10, 1000);
            Console.WriteLine($"{buildDefinitionName} is being queued and finished in {simulatedBuildTime} ms");
            await Task.Delay(simulatedBuildTime);
            Console.WriteLine($"{buildDefinitionName} finished");
        }
    }
}
