using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.DataExchange.ConsoleApp.Helper;
using Autodesk.DataExchange.Core.Enums;
using Autodesk.DataExchange.Extensions.HostingProvider;
using Autodesk.DataExchange.Interface;
using Autodesk.DataExchange.Models.Revit;
using Parameter = Autodesk.Parameters.Parameter;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class ParameterHelper_Test
    {
        private AssemblyResolver AssemblyResolver;
        private GeometryHelper GeometryHelper;
        private ParameterHelper parameterHelper;
        private Mock<IClient> _client;


        [TestInitialize]
        public void TestInit()
        {
            AssemblyResolver = new AssemblyResolver();
            _client = new Mock<IClient>();
            parameterHelper = new ParameterHelper();
            GeometryHelper = new GeometryHelper();
        }

        [TestMethod]
        public void CreateInstanceParameters_Test()
        {
            var exchangeData = RevitExchangeData.Create(_client.Object);
            var element = GeometryHelper.CreateBrep(exchangeData);
            parameterHelper.AddInstanceParameter(element, Parameter.AllModelDescription, "test", ParameterDataType.String);
            parameterHelper.AddInstanceParameter(element, Parameter.HostVolumeComputed, "300.50", ParameterDataType.Float64);
            parameterHelper.AddInstanceParameter(element, Parameter.CurtainWallPanelsHeight, "100", ParameterDataType.Int32);
            parameterHelper.AddInstanceParameter(element, Parameter.CoverTypeLength, "200", ParameterDataType.Int64);
            parameterHelper.AddInstanceParameter(element, Parameter.LevelIsStructural, "false", ParameterDataType.Bool);
            var totalParameters = element.InstanceParameters.Count;
            Assert.AreEqual(5, totalParameters);
            var parameter1 = element.InstanceParameters.FirstOrDefault(n =>
                n.ParameterDataType == ParameterDataType.String && n.Name == Parameter.AllModelDescription.ToString());
            var parameter2 = element.InstanceParameters.FirstOrDefault(n =>
                n.ParameterDataType == ParameterDataType.Float64 && n.Name == Parameter.HostVolumeComputed.ToString());
            var parameter3 = element.InstanceParameters.FirstOrDefault(n =>
                n.ParameterDataType == ParameterDataType.Int32 && n.Name == Parameter.CurtainWallPanelsHeight.ToString());
            var parameter4 = element.InstanceParameters.FirstOrDefault(n =>
                n.ParameterDataType == ParameterDataType.Int64 && n.Name == Parameter.CoverTypeLength.ToString());
            var parameter5 = element.InstanceParameters.FirstOrDefault(n =>
                n.ParameterDataType == ParameterDataType.Bool && n.Name == Parameter.LevelIsStructural.ToString());

            Assert.IsNotNull(parameter1);
            Assert.IsNotNull(parameter2);
            Assert.IsNotNull(parameter3);
            Assert.IsNotNull(parameter4);
            Assert.IsNotNull(parameter5);

            Assert.AreEqual(Convert.ToBoolean(parameter1.Value.ToString()),"test");
            Assert.AreEqual(Convert.ToDouble(parameter2.Value.ToString()), 300.50);
            Assert.AreEqual(Convert.ToInt32(parameter3.Value.ToString()), 100);
            Assert.AreEqual(Convert.ToInt64(parameter4.Value.ToString()), 200);
            Assert.AreEqual(Convert.ToBoolean(parameter5.Value.ToString()), false);
        }

        [TestMethod]
        public void CreateCustomParameters_Test()
        {
            var exchangeData = RevitExchangeData.Create(_client.Object);
            var element = GeometryHelper.CreateBrep(exchangeData);
            parameterHelper.AddCustomParameter("Test", element, "test", ParameterDataType.String);
            var totalParameters = element.InstanceParameters.Count;
            Assert.AreEqual(1, totalParameters);

            var parameter1 = element.InstanceParameters.LastOrDefault();
            Assert.IsNotNull(parameter1);
            Assert.AreEqual(parameter1.Value.ToString(),"test");
        }

    }
}
