using Autodesk.DataExchange.Interface;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class SessionStoreTests
    {
        [TestMethod]
        public void Capture_ThenApply_RoundTripsFolderAndDefaults()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults
            {
                StepPath = "C:\\samples\\part.stp",
                IfcPath = "C:\\samples\\model.ifc",
                ObjPath = "C:\\samples\\mesh.obj",
                BuiltInSchemaId = "schema-1",
            })
            {
                Folder = new FolderInfo("hub-1", "project-1", "folder-1", "US"),
                LastExchangeTitle = "Test Exchange",
            };

            var session = new SessionData();
            SessionStore.Capture(ctx, session);

            var restored = new SampleContext(new Mock<IClient>().Object, new Defaults());
            SessionStore.Apply(session, restored);

            Assert.AreEqual(ctx.Folder, restored.Folder);
            Assert.AreEqual("Test Exchange", restored.LastExchangeTitle);
            Assert.AreEqual("C:\\samples\\part.stp", restored.Defaults.StepPath);
            Assert.AreEqual("C:\\samples\\model.ifc", restored.Defaults.IfcPath);
            Assert.AreEqual("C:\\samples\\mesh.obj", restored.Defaults.ObjPath);
            Assert.AreEqual("schema-1", restored.Defaults.BuiltInSchemaId);
        }

        [TestMethod]
        public void Capture_PrefersLastExchangeTitle_OverLastExchangeWhenLastExchangeAbsent()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults())
            {
                LastExchange = null,
                LastExchangeTitle = "Untitled placeholder",
            };

            var session = new SessionData();
            SessionStore.Capture(ctx, session);

            Assert.AreEqual("Untitled placeholder", session.LastExchangeTitle);
        }

        [TestMethod]
        public void Apply_EmptySessionData_LeavesContextFolderNull()
        {
            var session = new SessionData();
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());

            SessionStore.Apply(session, ctx);

            Assert.IsNull(ctx.Folder);
            Assert.IsNull(ctx.LastExchange);
        }
    }
}
