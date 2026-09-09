using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach an ellipse primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (ellipse).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 5)]
    public sealed class AddPrimitiveEllipseSample : ISample
    {
        public string Name => "Add Primitive Ellipse";
        public string Description => "Attach an ellipse primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateSingleCurveGeometry(() => new Ellipse()), "ellipse primitive");
        }
}
