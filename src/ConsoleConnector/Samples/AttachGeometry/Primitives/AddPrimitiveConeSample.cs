using System.Threading.Tasks;
using Autodesk.GeometryUtilities.PrimitivesAPI;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a cone profile primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (cone).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 11)]
    public sealed class AddPrimitiveConeSample : ISample
    {
        public string Name => "Add Primitive Cone";
        public string Description => "Attach a polyline profile primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(
                ctx,
                () => GeometrySampleHelper.CreateSingleCurveGeometry(c => c.Curves.Add(new Polyline())),
                "cone profile primitive");
        }
}
