using Autodesk.DataExchange.Interface;
using ConsoleConnector.Common;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class NavigationHelperTests
    {
        private static SampleContext NewContext(FolderInfo? folder)
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());
            ctx.Folder = folder;
            return ctx;
        }

        [TestMethod]
        public void EnsureFullFolder_NoFolder_ReturnsFalse()
        {
            var ctx = NewContext(null);
            Assert.IsFalse(NavigationHelper.EnsureFullFolder(ctx));
        }

        [TestMethod]
        public void EnsureFullFolder_MissingFolderUrn_ReturnsFalse()
        {
            var ctx = NewContext(new FolderInfo("hub-1", "project-1", "", "US"));
            Assert.IsFalse(NavigationHelper.EnsureFullFolder(ctx));
        }

        [TestMethod]
        public void EnsureFullFolder_AllFieldsPresent_ReturnsTrue()
        {
            var ctx = NewContext(new FolderInfo("hub-1", "project-1", "folder-1", "US"));
            Assert.IsTrue(NavigationHelper.EnsureFullFolder(ctx));
        }

        [TestMethod]
        public void EnsureHub_NoFolder_ReturnsFalse()
        {
            var ctx = NewContext(null);
            Assert.IsFalse(NavigationHelper.EnsureHub(ctx));
        }

        [TestMethod]
        public void EnsureHub_HubIdPresent_ReturnsTrue()
        {
            var ctx = NewContext(new FolderInfo("hub-1", "", "", "US"));
            Assert.IsTrue(NavigationHelper.EnsureHub(ctx));
        }

        [TestMethod]
        public void EnsureProjectFolder_MissingProjectUrn_ReturnsFalse()
        {
            var ctx = NewContext(new FolderInfo("hub-1", "", "", "US"));
            Assert.IsFalse(NavigationHelper.EnsureProjectFolder(ctx));
        }

        [TestMethod]
        public void LoadFolderFromSession_IncompleteFolder_ReturnsNull()
        {
            var ctx = NewContext(new FolderInfo("hub-1", "project-1", "", "US"));
            Assert.IsNull(NavigationHelper.LoadFolderFromSession(ctx));
        }

        [TestMethod]
        public void LoadFolderFromSession_CompleteFolder_ReturnsFolder()
        {
            var folder = new FolderInfo("hub-1", "project-1", "folder-1", "US");
            var ctx = NewContext(folder);
            Assert.AreEqual(folder, NavigationHelper.LoadFolderFromSession(ctx));
        }
    }
}
