using System.Threading.Tasks;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class SampleRunnerTests
    {
        [TestInitialize]
        public void Init() => BatchMode.Enabled = true;

        [TestCleanup]
        public void Cleanup() => BatchMode.Enabled = false;

        // Against a bare Mock<IClient> every registered sample is expected to fail (no real SDK
        // backend), but RunAllAsync must still cover every discovered sample exactly once and
        // never let one sample's exception escape and abort the run.
        [TestMethod]
        public async Task RunAllAsync_CoversEveryDiscoveredSample_WithoutThrowing()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            var session = new SessionData();
            var expectedCount = SampleDiscovery.DiscoverSamples().Count;

            var result = await SampleRunner.RunAllAsync(ctx, session);

            Assert.AreEqual(expectedCount, result.Results.Count);
            Assert.AreEqual(result.Passed + result.Failed, result.Results.Count);
        }
    }
}
