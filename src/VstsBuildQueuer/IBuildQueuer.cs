using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;

namespace VstsBuildQueuer
{
    public interface IBuildQueuer
    {
        Task<IBuildQueuer> QueueBuilds(params string[] buildDefinitionNames);

        /// <summary>
        /// Queues the remaining builds that haven't been queued in earlier steps
        /// </summary>
        /// <param name="includeRegexFilter">Regular expression matching the build definition names you want to include</param>
        /// <param name="excludeRegexFilter">Regular expression matching the build definition names you want to exclude</param>
        /// <returns></returns>
        Task<IBuildQueuer> QueueRemainingBuildDefinitions(string includeRegexFilter, string excludeRegexFilter = "");

        /// <summary>
        /// Initializes variables and fetches build definitions from VSTS/TFS
        /// </summary>
        /// <param name="projectName">Project name in which the build definitions reside</param>
        /// <param name="vstsUrl">Url to the VSTS-account or Collection (TFS)</param>
        /// <param name="credentials">Credentials to use when authenticating with VSTS/TFS. Default value is VssBasicCredential, works for instance when authenticating via NTLM to TFS-server.</param>
        /// <param name="logAction">Trace.WriteLine is being used by default, override if wanted.</param>
        /// <param name="buildFailedAction">Trace.WriteLine is being used by default. Override if you want to for instance throw an Exception and abort.</param>
        /// <returns></returns>
        Task<IBuildQueuer> InitConfig(string projectName, string vstsUrl, VssCredentials credentials = null,
            Action<string> logAction = null, Action<string> buildFailedAction = null);
    }
}