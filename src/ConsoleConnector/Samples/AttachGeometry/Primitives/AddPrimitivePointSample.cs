using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a design point primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (point).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 1)]
    public sealed class AddPrimitivePointSample : ISample
    {
        public string Name => "Add Primitive Point";
        public string Description => "Attach a design point primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreatePointGeometry(), "point primitive");
        }
}
