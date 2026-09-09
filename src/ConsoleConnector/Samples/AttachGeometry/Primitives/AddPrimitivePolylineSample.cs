using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a polyline primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (polyline).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 3)]
    public sealed class AddPrimitivePolylineSample : ISample
    {
        public string Name => "Add Primitive Polyline";
        public string Description => "Attach a polyline primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateSingleCurveGeometry(c => c.Curves.Add(new Polyline())), "polyline primitive");
        }
}
