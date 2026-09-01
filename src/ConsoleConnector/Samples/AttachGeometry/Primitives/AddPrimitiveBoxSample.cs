using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a box outline (composite curve) primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (box).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 8)]
    public sealed class AddPrimitiveBoxSample : ISample
    {
        public string Name => "Add Primitive Box";
        public string Description => "Attach a composite-curve box outline primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(
                ctx,
                () => GeometrySampleHelper.CreateSingleCurveGeometry(() => new CompositeCurve()),
                "box outline primitive");
        }
}
