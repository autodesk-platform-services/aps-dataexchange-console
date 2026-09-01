using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a cylinder profile primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (cylinder).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 9)]
    public sealed class AddPrimitiveCylinderSample : ISample
    {
        public string Name => "Add Primitive Cylinder";
        public string Description => "Attach a circle profile primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(
                ctx,
                () => GeometrySampleHelper.CreateSingleCurveGeometry(c => c.Curves.Add(new Circle())),
                "cylinder profile primitive");
        }
}
