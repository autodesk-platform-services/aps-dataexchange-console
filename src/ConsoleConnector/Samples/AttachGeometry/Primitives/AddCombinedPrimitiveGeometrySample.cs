using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach multiple primitives in one container.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (combined).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 12)]
    public sealed class AddCombinedPrimitiveGeometrySample : ISample
    {
        public string Name => "Add Combined Primitive Geometry";
        public string Description => "Attach multiple primitives in one container";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateCombinedPrimitiveGeometry(), "combined primitive geometry");
        }
}
