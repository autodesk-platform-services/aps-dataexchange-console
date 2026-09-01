using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a circle primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (circle).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 4)]
    public sealed class AddPrimitiveCircleSample : ISample
    {
        public string Name => "Add Primitive Circle";
        public string Description => "Attach a circle primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateSingleCurveGeometry(c => c.Curves.Add(new Circle())), "circle primitive");
        }
}
