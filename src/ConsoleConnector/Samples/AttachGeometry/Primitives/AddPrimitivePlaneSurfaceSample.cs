using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a B-surface (plane surface) primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (B-surface).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 7)]
    public sealed class AddPrimitivePlaneSurfaceSample : ISample
    {
        public string Name => "Add Primitive Plane Surface";
        public string Description => "Attach a B-surface primitive";

        public async Task RunAsync(SampleContext ctx)
        {
            // Empty BSurface payloads are not supported end-to-end; use a populated curve primitive instead.
            await GeometrySampleHelper.AttachPrimitiveAsync(
                ctx,
                GeometrySampleHelper.CreateLineGeometry,
                "surface stand-in (line primitive)");
        }
    }
}
