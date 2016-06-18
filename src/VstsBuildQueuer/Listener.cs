using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;

namespace VstsBuildQueuer
{
    public abstract class Listener
    {
        protected readonly ConcurrentDictionary<int, BuildWaitingForResult> BuildsWaitingForResults = new ConcurrentDictionary<int, BuildWaitingForResult>();
        private readonly ConcurrentBag<string> _buildDefinitionsAlreadyBuilt = new ConcurrentBag<string>();

        public IEnumerable<string> NameOfBuildDefinitionsBuilt => _buildDefinitionsAlreadyBuilt;
        public IEnumerable<string> UnfinishedBuilds => BuildsWaitingForResults.Select(d => d.Value.Build.Definition.Name);

        public abstract void Init(BuildHttpClient buildHttpClient, TeamProject teamproject);

        protected void NotifyBuildFinished(int buildId, BuildResult buildResult)
        {
            if (!BuildsWaitingForResults.ContainsKey(buildId))
            {
                return;
            }

            BuildWaitingForResult buildWaitingForResult;
            BuildsWaitingForResults.TryRemove(buildId, out buildWaitingForResult);

            if (buildWaitingForResult != null)
            {
                buildWaitingForResult.TaskCompletionSource.SetResult(buildResult);
            }
        }

        public void WaitForResult(BuildWaitingForResult buildWaitingForResult)
        {
            _buildDefinitionsAlreadyBuilt.Add(buildWaitingForResult.Build.Definition.Name);
            BuildsWaitingForResults.TryAdd(
                buildWaitingForResult.Build.Id,
                buildWaitingForResult);
        }
    }
}
