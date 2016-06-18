using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace VstsBuildQueuer
{
    public class BuildQueuer : IBuildQueuer
    {
        private BuildHttpClient _buildHttpClient;
        private TeamProject _teamproject;
        private List<DefinitionReference> _buildDefinitions;
        private readonly PollListener _listener;
        private Action<string> _logAction = message => { Trace.WriteLine(message); };
        private Action<string> _buildFailedAction = buildDefinitionName =>
        {
            Trace.WriteLine($"FAILED BUILD: {buildDefinitionName}, see build log for more information. Continuing");
        };

        public BuildQueuer()
        {
            _listener = new PollListener();
        }

        /// <summary>
        /// Initializes variables and fetches build definitions from VSTS/TFS
        /// </summary>
        /// <param name="projectName">Project name in which the build definitions reside</param>
        /// <param name="vstsUrl">Url to the Team Project (VSTS) or Collection (TFS)</param>
        /// <param name="credentials">Credentials to use when authenticating with VSTS/TFS. Default value is VssBasicCredential, works for instance when authenticating via NTLM to TFS-server.</param>
        /// <param name="logAction">Trace.WriteLine is being used by default, override if wanted.</param>
        /// <param name="buildFailedAction">Trace.WriteLine is being used by default. Override if you want to for instance throw an Exception and abort.</param>
        /// <returns></returns>
        public async Task<IBuildQueuer> InitConfig(string projectName, string vstsUrl, VssCredentials credentials = null,
            Action<string> logAction = null, Action<string> buildFailedAction = null)
        {
            if (buildFailedAction != null)
            {
                _buildFailedAction = buildFailedAction;
            }

            if (logAction != null)
            {
                _logAction = logAction;
            }

            if (credentials == null)
            {
                credentials = new VssBasicCredential();
            }

            var vstsUri = new Uri(vstsUrl);
            var projectHttpClient = new ProjectHttpClient(vstsUri, credentials);
            _buildHttpClient = new BuildHttpClient(vstsUri, credentials);
            _teamproject = await projectHttpClient.GetProject(projectName);
            _buildDefinitions = await _buildHttpClient.GetDefinitionsAsync(_teamproject.Id);

            _listener.Init(_buildHttpClient, _teamproject);
            
            _logAction("Configuration is initialized");

            return this;
        }

        public async Task<IBuildQueuer> QueueBuilds(params string[] buildDefinitionNames)
        {
            await Task.WhenAll(buildDefinitionNames.Select(TriggerIndividualBuildAsync));
            return this;
        }
        
        public async Task<IBuildQueuer> QueueRemainingBuildDefinitions(string includeRegexFilter, string excludeRegexFilter = "")
        {
            var includeRegex = new Regex(includeRegexFilter);
            var excludeRegex = new Regex(excludeRegexFilter);
            var filteredBuildDefinitionNames =
                _buildDefinitions
                    .Select(bd => bd.Name)
                    .Where(bn => includeRegex.IsMatch(bn)
                                    && (string.IsNullOrWhiteSpace(excludeRegexFilter)
                                        || !excludeRegex.IsMatch(bn)))
                    .Except(_listener.NameOfBuildDefinitionsBuilt)
                    .OrderBy(s => s)
                    .ToList();

            _logAction($"Queue build definitions not processed up to this point and matching regex filter, count {filteredBuildDefinitionNames.Count}");

            await QueueBuilds(filteredBuildDefinitionNames.ToArray());

            return this;
        }

        private async Task TriggerIndividualBuildAsync(string buildDefinitionName)
        {
            Build queuedBuild;
            try
            {
                queuedBuild = await _buildHttpClient.QueueBuildAsync(new Build
                {
                    Definition = new DefinitionReference
                    {
                        Id = _buildDefinitions.Single(d => d.Name.Equals(buildDefinitionName)).Id,
                        Name = buildDefinitionName
                    },
                    Project = new TeamProjectReference
                    {
                        Id = _teamproject.Id
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Build definition {buildDefinitionName} does not exist", ex);
            }

            var buildTriggeredTime = DateTime.Now;

            _logAction($"{buildDefinitionName} is queued ({queuedBuild.Id}) {buildTriggeredTime.TimeOfDay}");
            
            var taskCompletionSource = new TaskCompletionSource<BuildResult>();

            _listener.WaitForResult(new BuildWaitingForResult
            {
                Build = queuedBuild,
                TaskCompletionSource = taskCompletionSource
            });

            var buildResult = await taskCompletionSource.Task;

            _logAction($"{buildDefinitionName} finished with result {buildResult} {(DateTime.Now - buildTriggeredTime).TotalSeconds} sec");
            if (buildResult == BuildResult.Failed)
            {
                _buildFailedAction(buildDefinitionName);
            }
        }
    }
}
