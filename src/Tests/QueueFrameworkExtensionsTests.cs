using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VstsBuildQueuer.Extensions;

namespace VstsBuildQueuer.Tests
{
    [TestClass]
    public class QueueFrameworkExtensionsTests
    {
        [TestMethod]
        public async Task TriggerBuild_IsBeingCalled()
        {
            var task = Task<IQueueFramework>.Factory.StartNew(() => new QueueFramework());
            await task.TriggerBuilds("buildDefinitionName");
        }
    }
}
