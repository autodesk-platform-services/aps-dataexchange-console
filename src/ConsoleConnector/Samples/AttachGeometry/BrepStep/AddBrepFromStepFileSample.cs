using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to attach BREP geometry from a STEP file on disk.
    /// SDK: ElementDataModel.CreateFileGeometry (STEP file).
    /// Console plumbing: GeometrySampleHelper.AttachFileGeometryAsync.
    /// Prerequisites: 2.2 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 1)]
    public sealed class AddBrepFromStepFileSample : ISample
    {
        public string Name => "Add BREP from STEP file";
        public string Description => "Attach a STEP file as BREP geometry";

        public Task RunAsync(SampleContext ctx) =>
            GeometrySampleHelper.AttachFileGeometryAsync(
                ctx,
                GeometryFormat.Step,
                "STEP path",
                ctx.Defaults.StepPath,
                syncAfter: true,
                "nist_ftc_09_asme1_rd.stp");
    }
}
