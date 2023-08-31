using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Extensions.HostingProvider;
using Autodesk.DataExchange.Interface;
using Parameter = Autodesk.Parameters.Parameter;
using Moq;
using Autodesk.DataExchange.DataModels;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class GeometryHelper_Test
    {
        private AssemblyResolver AssemblyResolver;
        private GeometryHelper GeometryHelper;
        private Mock<IClient> _client;

        [TestInitialize]
        public void TestInit()
        {
            AssemblyResolver = new AssemblyResolver();
            GeometryHelper = new GeometryHelper();
            _client = new Mock<IClient>();
        }

        [TestMethod]
        public void CreateBrep_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.CreateBrep(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(),1);
        }

        [TestMethod]
        public void CreateIfc_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.CreateIfc(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);

        }

        [TestMethod]
        public void CreateMesh_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.CreateMesh(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);
        }

        [TestMethod]
        public void CreateLine_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.AddLine(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);
        }

        [TestMethod]
        public void CreatePoint_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.AddPoint(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);
        }

        [TestMethod]
        public void CreateCircle_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.AddCircle(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);
        }

        [TestMethod]
        public void CreatePrimitiveAll_Test()
        {
            var exchangeData = ElementDataModel.Create(_client.Object);
            var element = GeometryHelper.AddPrimitive(exchangeData);
            Assert.IsNotNull(element);
            Assert.AreEqual(exchangeData.Elements.Count(), 1);
        }
    }
}
