using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;

namespace VstsBuildQueuer
{
    public class BuildWaitingForResult
    {
        public Build Build { get; set; }

        public TaskCompletionSource<BuildResult> TaskCompletionSource { get; set; }
    }
}
