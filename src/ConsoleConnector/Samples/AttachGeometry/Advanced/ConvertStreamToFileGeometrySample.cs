using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Explicit MemoryStream to CreateFileGeometry conversion.
    /// SDK: ElementDataModel.CreateFileGeometry (stream).
    /// Console plumbing: GeometrySampleHelper.AttachStreamGeometryAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 5)]
    public sealed class ConvertStreamToFileGeometrySample : ISample
    {
        public string Name => "Convert Stream to File Geometry";
        public string Description => "Load STEP from stream via CreateFileGeometry(MemoryStream)";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachStreamGeometryAsync(
                ctx,
                GeometryFormat.Step,
                "STEP path",
                ctx.Defaults.StepPath,
                "nist_ftc_09_asme1_rd.stp");
        }
}
