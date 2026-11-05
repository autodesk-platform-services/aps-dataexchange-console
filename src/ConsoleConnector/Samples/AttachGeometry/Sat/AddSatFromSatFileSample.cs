using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach SAT geometry from a file.
    /// SDK: ElementDataModel.CreateFileGeometry (SAT file).
    /// Console plumbing: GeometrySampleHelper.AttachFileGeometryAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 6, 1)]
    public sealed class AddSatFromSatFileSample : ISample
    {
        public string Name => "Add SAT from SAT file";
        public string Description => "Attach a SAT file as geometry";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachFileGeometryAsync(ctx, GeometryFormat.Sat, "SAT path", ctx.Defaults.SatPath, "exchange.sat");
        }
}
