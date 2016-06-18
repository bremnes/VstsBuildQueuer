using System.Threading.Tasks;

namespace VstsBuildQueuer
{
    public interface IQueueFramework
    {
        Task<QueueFramework> TriggerBuilds(params string[] buildDefinitionNames);
    }
}