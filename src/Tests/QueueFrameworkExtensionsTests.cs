using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VstsBuildQueuer.Extensions;

namespace VstsBuildQueuer.Tests
{
    [TestClass]
    public class QueueFrameworkExtensionsTests
    {
        private Mock<IQueueFramework> _queueFramework;

        [TestInitialize]
        public void Setup()
        {
            _queueFramework = new Mock<IQueueFramework>();
        }

        [TestMethod]
        public async Task QueueBuilds_IsBeingCalled()
        {
            var queueFrameworkTask = Task<IQueueFramework>.Factory.StartNew(() => _queueFramework.Object);
            await queueFrameworkTask.QueueBuildsOnCompleted("buildDefinitionName");

            _queueFramework.Verify(framework => framework.QueueBuilds("buildDefinitionName"), Times.Once);
        }
    }
}
