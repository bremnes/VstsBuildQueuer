using System.Threading.Tasks;

namespace VstsBuildQueuer.Extensions
{
    public static class QueueFrameworkExtensions
    {
        public static async Task<IQueueFramework> QueueBuildsOnCompleted(this Task<IQueueFramework> task, params string[] buildDefinitionNames)
        {
            var queueFramework = await task;
            return await queueFramework.QueueBuilds(buildDefinitionNames);
        }
    }
}
