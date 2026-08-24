using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a sphere marker (design point) primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (sphere marker).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 10)]
    public sealed class AddPrimitiveSphereSample : ISample
    {
        public string Name => "Add Primitive Sphere";
        public string Description => "Attach a design point marker primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreatePointGeometry(), "sphere marker primitive");
        }
}
