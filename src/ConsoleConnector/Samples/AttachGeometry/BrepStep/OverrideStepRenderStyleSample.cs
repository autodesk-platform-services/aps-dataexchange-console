using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: How to make your RenderStyle win over the colours stored inside a STEP file.
    /// SDK: GeometryConfiguration.AllowUserRenderStyleOverride + ElementDataModel.CreateFileGeometry (renderStyle).
    /// Console plumbing: GeometrySampleHelper.RequireGeometryConfiguration, AttachFileGeometryAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 1, 6)]
    public sealed class OverrideStepRenderStyleSample : ISample
    {
        public string Name => "Override STEP Render Style";
        public string Description => "Let the connector RenderStyle override file-derived BRep body styles";

        public async Task RunAsync(SampleContext ctx)
        {
            var config = GeometrySampleHelper.RequireGeometryConfiguration(ctx);
            var previous = config.AllowUserRenderStyleOverride;
            TerminalUi.Info("By default, body styles read from the STEP file win and the RenderStyle you pass is ignored for BRep.");
            TerminalUi.Info("AllowUserRenderStyleOverride = true makes the RenderStyle passed to CreateFileGeometry apply instead.");
            TerminalUi.Chat($"Current AllowUserRenderStyleOverride: {previous}");

            // SDK: global switch read when the geometry is converted during sync
            config.AllowUserRenderStyleOverride = true;
            try
            {
                var style = GeometrySampleHelper.DefaultRenderStyle;
                var rgba = style.RGBA;
                TerminalUi.Chat($"Attaching STEP with RenderStyle '{style.Name}' (RGBA {rgba.Red}, {rgba.Green}, {rgba.Blue}, {rgba.Alpha}).");
                await GeometrySampleHelper.AttachFileGeometryAsync(
                    ctx, GeometryFormat.Step, "STEP path", ctx.Defaults.StepPath, syncAfter: true, "nist_ftc_09_asme1_rd.stp");
            }
            finally
            {
                config.AllowUserRenderStyleOverride = previous;
                TerminalUi.Dim($"Restored AllowUserRenderStyleOverride to {previous}.");
            }
        }
    }
}
