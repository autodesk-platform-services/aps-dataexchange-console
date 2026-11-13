using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach SAT geometry from a memory stream.
    /// SDK: ElementDataModel.CreateFileGeometry (SAT stream).
    /// Console plumbing: GeometrySampleHelper.AttachStreamGeometryAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 6, 2)]
    public sealed class AddSatFromSatStreamSample : ISample
    {
        public string Name => "Add SAT from SAT stream";
        public string Description => "Attach SAT geometry from a memory stream";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachStreamGeometryAsync(ctx, GeometryFormat.Sat, "SAT path", ctx.Defaults.SatPath, "box.sat");
        }
}
