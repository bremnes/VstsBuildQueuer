using System.Threading.Tasks;

namespace VstsBuildQueuer
{
    public static class BuildQueuerExtensions
    {
        public static async Task<IBuildQueuer> QueueBuilds(this Task<IBuildQueuer> task, params string[] buildDefinitionNames)
        {
            var buildQueuer = await task;
            return await buildQueuer.QueueBuilds(buildDefinitionNames);
        }

        /// <summary>
        /// Queue the remaining builds that haven't been queued in earlier steps
        /// </summary>
        /// <param name="task"></param>
        /// <param name="includeRegexFilter">Regular expression matching the build definition names you want to include</param>
        /// <param name="excludeRegexFilter">Regular expression matching the build definition names you want to exclude</param>
        /// <returns></returns>
        public static async Task<IBuildQueuer> QueueRemainingBuildDefinitions(this Task<IBuildQueuer> task, string includeRegexFilter, string excludeRegexFilter = "")
        {
            var buildQueuer = await task;
            return await buildQueuer.QueueRemainingBuildDefinitions(includeRegexFilter, excludeRegexFilter);
        }
    }
}
