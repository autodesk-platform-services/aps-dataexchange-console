using System.Threading;
using System.Threading.Tasks;
using ConsoleConnector.Driver;
using ConsoleConnector.Common;

namespace ConsoleConnector.Samples
{
    /// <summary>
    /// What you learn: Export the same exchange as STEP, IFC, and OBJ.
    /// SDK: DownloadCompleteExchangeAsSTEP/IFC/OBJ.
    /// Console plumbing: ExchangeSessionHelper + DownloadSampleHelper.
    /// Prerequisites: folder in session.
    /// </summary>
    [SampleAddress(11, 9)]
    public sealed class DownloadAllFormatsScenarioSample : ISample
    {
        public string Name => "Download All Formats";
        public string Description => "Download exchange as STEP, IFC, and OBJ";

        public async Task RunAsync(SampleContext ctx)
        {
            TerminalUi.Chat("Scenario 11.9: Load -> STEP + IFC + OBJ downloads");
            await ExchangeSessionHelper.LoadPickedExchangeAsync(ctx);

            var stepSession = await DownloadSampleHelper.BeginAsync(ctx);
            if (stepSession != null)
            {
                var path = DownloadSampleHelper.ResolveOutputPath("STEP output path", "exchange.step");
                await DownloadSampleHelper.RunExchangeDownloadAsync(
                    "STEP",
                    () => ctx.Client.DownloadCompleteExchangeAsSTEP(stepSession.Identifier, path, CancellationToken.None));
            }

            var ifcSession = await DownloadSampleHelper.BeginAsync(ctx);
            if (ifcSession != null)
            {
                var path = DownloadSampleHelper.ResolveOutputPath("IFC output path", "exchange.ifc");
                await DownloadSampleHelper.RunExchangeDownloadAsync(
                    "IFC",
                    () => ctx.Client.DownloadCompleteExchangeAsIFC(ifcSession.Identifier, path, CancellationToken.None));
            }

            var objSession = await DownloadSampleHelper.BeginAsync(ctx);
            if (objSession != null)
            {
                var path = DownloadSampleHelper.ResolveOutputPath("OBJ output folder", "exchange_obj");
                await DownloadSampleHelper.RunExchangeDownloadAsync(
                    "OBJ",
                    () => DownloadSampleHelper.DownloadObj(ctx, objSession.Identifier, path));
            }

            TerminalUi.Success("Scenario complete.");
        }
    }
}
