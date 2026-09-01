using System.Threading.Tasks;
using Autodesk.DataExchange.DataModels;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Geometry can carry custom color and opacity via RenderStyle.
    /// SDK: ElementDataModel.CreatePrimitiveGeometry (custom RenderStyle).
    /// Console plumbing: GeometrySampleHelper.AttachPrimitiveAsync.
    /// Prerequisites: 2.3 Load Exchange.
    /// </summary>
    [SampleAddress(4, 5, 1)]
    public sealed class AddCustomRenderStyleSample : ISample
    {
        public string Name => "Add Custom Render Style";
        public string Description => "Attach primitive geometry with a custom RenderStyle";

        public async Task RunAsync(SampleContext ctx)
        {
            var style = new RenderStyle("ConsoleConnector Custom", new RGBA(255, 128, 0, 255), 0.5);
            var geometry = ElementDataModel.CreatePrimitiveGeometry(
                GeometrySampleHelper.CreateLineGeometry().Geometry,
                style,
                GeometrySampleHelper.DefaultUnits);
            await GeometrySampleHelper.AttachPrimitiveAsync(ctx, () => geometry, "custom-styled primitive");
        }
    }
}
