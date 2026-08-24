using System.Threading.Tasks;
using Autodesk.DataExchange.Core.Enums;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: One exchange can hold mixed geometry types and params.
    /// SDK: Multiple geometry types + CreateInstanceParameterAsync + SyncExchangeDataAsync.
    /// Console plumbing: ExchangeSessionHelper + GeometrySampleHelper + ParameterSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 4)]
    public sealed class MultiGeometryMultiParameterScenarioSample : ISample
    {
        public string Name => "Multi Geometry Multi Parameter";
        public string Description => "BREP + mesh + primitive + custom params";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.4: Create -> empty model -> geometries -> params -> Sync");
            if (!await ExchangeSessionHelper.CreateAndPrepareEmptyAsync(ctx))
                return;

            try
            {
                await GeometrySampleHelper.AttachFileGeometryAsync(
                    ctx, GeometryFormat.Step, "STEP path", ctx.Defaults.StepPath, syncAfter: false, "nist_ftc_09_asme1_rd.stp");
                await GeometrySampleHelper.AttachFileGeometryAsync(
                    ctx, GeometryFormat.Obj, "OBJ path", ctx.Defaults.ObjPath, syncAfter: false, "mesh2.obj");
                await GeometrySampleHelper.AttachPrimitiveAsync(
                    ctx, () => GeometrySampleHelper.CreatePointGeometry(), "point primitive", syncAfter: false);

                var session = await ParameterSampleHelper.BeginAsync(ctx);
                var element = session == null ? null : ParameterSampleHelper.PickElement(session.Model);
                if (element != null)
                {
                    await ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync(element);
                    await ParameterSampleHelper.AddCustomInstanceParamInteractiveAsync(element);
                }

                await ExchangeSessionHelper.SyncDemoLineAsync(ctx, ctx.ScenarioExchangeTitle);
                TerminalUi.Success("Scenario complete.");
            }
            finally
            {
                ExchangeSessionHelper.EndScenario(ctx);
            }
        }
    }
}
