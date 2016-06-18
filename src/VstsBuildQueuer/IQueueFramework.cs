using System.Threading.Tasks;

namespace VstsBuildQueuer
{
    public interface IQueueFramework
    {
        Task<QueueFramework> QueueBuilds(params string[] buildDefinitionNames);
    }
}