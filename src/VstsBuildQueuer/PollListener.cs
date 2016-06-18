using System.Linq;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;

namespace VstsBuildQueuer
{
    class PollListener : Listener
    {
        private BuildHttpClient _buildHttpClient;
        private TeamProject _teamProject;

        public override void Init(BuildHttpClient buildHttpClient, TeamProject teamproject)
        {
            _teamProject = teamproject;
            _buildHttpClient = buildHttpClient;

            var pollForCompletedBuildsTimer = new System.Timers.Timer(5000);
            pollForCompletedBuildsTimer.Elapsed += PollForCompletedBuildsTimer_Elapsed;
            pollForCompletedBuildsTimer.Enabled = true;
        }

        private async void PollForCompletedBuildsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var waitingBuildDefinitionIds = BuildsWaitingForResults.Select(b => b.Value.Build.Definition.Id).ToList();
            var waitingBuildIds = BuildsWaitingForResults.Select(b => b.Value.Build.Id).ToList();

            var builds = await _buildHttpClient.GetBuildsAsync(_teamProject.Id.ToString(), waitingBuildDefinitionIds);

            var finishedBuilds = builds.Where(b => b.FinishTime.HasValue && waitingBuildIds.Contains(b.Id)).ToList();

            if (!finishedBuilds.Any())
            {
                return;
            }

            foreach (var finishedBuild in finishedBuilds)
            {
                if (finishedBuild.Result.HasValue)
                {
                    NotifyBuildFinished(finishedBuild.Id, finishedBuild.Result.Value);
                }
            }
        }
    }
}
