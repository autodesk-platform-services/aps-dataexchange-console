using Autodesk.DataExchange.Interface;
using ConsoleConnector.Common;
using ConsoleConnector.Driver;
using ConsoleConnector.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConsoleConnector_Test
{
    [TestClass]
    public class GeometrySampleHelperTests
    {
        // ResolvePath falls through to Prompt.AskString, which reads the console unless
        // BatchMode is enabled — keep it on for these tests so they never block on stdin.
        [TestInitialize]
        public void Init() => BatchMode.Enabled = true;

        [TestCleanup]
        public void Cleanup() => BatchMode.Enabled = false;

        private static SampleContext NewContext() =>
            new(new Mock<IClient>().Object, new Defaults());

        [TestMethod]
        public void ResolvePath_StepLabel_UpdatesStepPathDefault()
        {
            var ctx = NewContext();

            var path = GeometrySampleHelper.ResolvePath(ctx, "STEP path", "C:\\samples\\part.stp", "part.stp");

            Assert.AreEqual("C:\\samples\\part.stp", path);
            Assert.AreEqual("C:\\samples\\part.stp", ctx.Defaults.StepPath);
            Assert.IsNull(ctx.Defaults.IfcPath);
            Assert.IsNull(ctx.Defaults.ObjPath);
        }

        [TestMethod]
        public void ResolvePath_IfcLabel_UpdatesIfcPathDefault()
        {
            var ctx = NewContext();

            GeometrySampleHelper.ResolvePath(ctx, "IFC path", "C:\\samples\\model.ifc", "model.ifc");

            Assert.AreEqual("C:\\samples\\model.ifc", ctx.Defaults.IfcPath);
            Assert.IsNull(ctx.Defaults.StepPath);
        }

        [TestMethod]
        public void ResolvePath_ObjLabel_UpdatesObjPathDefault()
        {
            var ctx = NewContext();

            GeometrySampleHelper.ResolvePath(ctx, "OBJ path", "C:\\samples\\mesh.obj", "mesh.obj");

            Assert.AreEqual("C:\\samples\\mesh.obj", ctx.Defaults.ObjPath);
        }

        [TestMethod]
        public void ResolvePath_LabelComparisonIsCaseInsensitive()
        {
            var ctx = NewContext();

            GeometrySampleHelper.ResolvePath(ctx, "step path (lowercase)", "C:\\samples\\part.stp", "part.stp");

            Assert.AreEqual("C:\\samples\\part.stp", ctx.Defaults.StepPath);
        }
    }
}
