using Autodesk.DataExchange.DataModels;
using Autodesk.DataExchange.Interface;
using ConsoleConnector.Common;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class ElementSampleHelperTests
    {
        [TestMethod]
        public void EnsureFolder_NoFolderInSession_ReturnsFalse()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults());

            Assert.IsFalse(ElementSampleHelper.EnsureFolder(ctx));
        }

        [TestMethod]
        public void EnsureFolder_FullFolderInSession_ReturnsTrue()
        {
            var ctx = new SampleContext(new Mock<IClient>().Object, new Defaults())
            {
                Folder = new FolderInfo("hub-1", "project-1", "folder-1", "US"),
            };

            Assert.IsTrue(ElementSampleHelper.EnsureFolder(ctx));
        }

        [TestMethod]
        public void ClassifyGeneric_SetsCategoryFamilyAndType()
        {
            var client = new Mock<IClient>().Object;
            var model = ElementDataModel.Create(client);
            var element = model.AddElement("element-1", "Test Element");

            ElementSampleHelper.ClassifyGeneric(model, element);

            Assert.AreEqual("Generics", element.Category);
            Assert.AreEqual("Generic", element.Family);
            Assert.IsNotNull(element.Type);
        }
    }
}
