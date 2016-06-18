using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace VstsBuildQueuer.Tests
{
    [TestClass]
    public class BuildQueuerExtensionsTests
    {
        private Mock<IBuildQueuer> _buildQueuerMock;

        [TestInitialize]
        public void Setup()
        {
            _buildQueuerMock = new Mock<IBuildQueuer>();
        }

        [TestMethod]
        public async Task QueueBuilds_IsBeingCalled()
        {
            var buildQueuerTask = Task<IBuildQueuer>.Factory.StartNew(() => _buildQueuerMock.Object);
            await buildQueuerTask.QueueBuilds("buildDefinitionName");

            _buildQueuerMock.Verify(queuer => queuer.QueueBuilds("buildDefinitionName"), Times.Once);
        }
    }
}
