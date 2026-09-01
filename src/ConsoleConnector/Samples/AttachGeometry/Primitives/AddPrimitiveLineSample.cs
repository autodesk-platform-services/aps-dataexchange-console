using System.Threading.Tasks;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach a line curve primitive.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (line).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 4, 2)]
    public sealed class AddPrimitiveLineSample : ISample
    {
        public string Name => "Add Primitive Line";
        public string Description => "Attach a line curve primitive";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => GeometrySampleHelper.CreateLineGeometry(), "line primitive");
        }
}
