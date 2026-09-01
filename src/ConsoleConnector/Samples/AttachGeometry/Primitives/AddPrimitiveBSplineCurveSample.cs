using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a B-spline curve primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (B-spline).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 6)]
    public sealed class AddPrimitiveBSplineCurveSample : ISample
    {
        public string Name => "Add Primitive BSpline Curve";
        public string Description => "Attach a B-spline curve primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateSingleCurveGeometry(() => new BCurve()), "B-spline primitive");
        }
}
