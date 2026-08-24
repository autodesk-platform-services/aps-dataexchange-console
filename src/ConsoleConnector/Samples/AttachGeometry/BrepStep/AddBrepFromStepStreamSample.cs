using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach STEP BREP from a memory stream.
    /// SDK: ElementDataModel.CreateFileGeometry (STEP stream).
    /// Console plumbing: GeometrySampleHelper.AttachStreamGeometryAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 2)]
    public sealed class AddBrepFromStepStreamSample : ISample
    {
        public string Name => "Add BREP from STEP stream";
        public string Description => "Attach STEP geometry from a memory stream";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachStreamGeometryAsync(ctx, GeometryFormat.Step, "STEP path", ctx.Defaults.StepPath, "nist_ftc_09_asme1_rd.stp");
        }
}
