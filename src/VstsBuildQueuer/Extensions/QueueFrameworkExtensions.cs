using System.Threading.Tasks;

namespace VstsBuildQueuer.Extensions
{
    public static class QueueFrameworkExtensions
    {
        public static async Task<IQueueFramework> TriggerBuilds(this Task<IQueueFramework> task, params string[] buildDefinitionNames)
        {
            var queueFramework = await task;
            return await queueFramework.TriggerBuilds(buildDefinitionNames);
        }
    }
}
